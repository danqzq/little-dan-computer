using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Danqzq.Workshops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static Danqzq.PostOperator;

namespace Danqzq
{
    public class Assembler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CPUManager _cpuManager;
        [SerializeField] private MemoryManager _memoryManager;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private OutputManager _outputManager;
        
        [Header("UI")]
        [SerializeField] private TMP_InputField _codeInput;
        [SerializeField] private GameObject _codeInputScrollbar;
        [SerializeField] private Button _assembleButton;
        [SerializeField] private TMP_Text _assemblyOutput;
        [SerializeField] private Animator _assemblyOutputAnimator;
        [SerializeField] private GameObject _haltButton;

        [Header("Workshops")]
        [SerializeField] private TMP_Text _workshopCompletionText;
        [SerializeField] private GameObject _workshopCompleteMenu;

        private const string CURRENT_COMMAND_POINTER = "<color=red><</color>";

        private Dictionary<string, short> _variables;
        private Dictionary<string, short> _labels;

        private bool _isExecuting;
        private bool _workshopModeEnabled;
        
        private static readonly int IsOpenAnimatorBoolean = Animator.StringToHash("isOpen");
        
        public bool IsAssemblyEdited { get; set; }

        public string[] GetAssemblyLines() => _codeInput.text.Split('\n');
        
        public void SetAssemblyLines(string[] lines) => _codeInput.text = string.Join('\n', lines);

        private void Start()
        {
            IsAssemblyEdited = false;
            _codeInput.onValueChanged.AddListener(OnEditCode);

            var workshop = WorkshopManager.CurrentWorkshop;
            if (!(_workshopModeEnabled = workshop != null))
            {
                return;
            }
            
            var savedCode = SaveManager.Load(Globals.WORKSHOP_CODE_SAVE_KEY + workshop.ID);
            _codeInput.text = string.IsNullOrEmpty(savedCode) ? workshop.BaseCode : savedCode;
        }

        private void OnEditCode(string newCode)
        {
            IsAssemblyEdited = true;
            const int codeInputLinesThreshold = 15;
            if (newCode.Split('\n').Length > codeInputLinesThreshold)
            {
                _codeInputScrollbar.SetActive(true);
            }
            else if (_codeInputScrollbar.activeSelf)
            {
                _codeInputScrollbar.SetActive(false);
            }
        }

        public void ToggleAssemblyOutput()
        {
            var isAssemblyOutputVisible = _assemblyOutputAnimator.GetBool(IsOpenAnimatorBoolean);
            _assemblyOutputAnimator.SetBool(IsOpenAnimatorBoolean, !isAssemblyOutputVisible);
        }

        public void Assemble()
        {
            _variables = _labels = new Dictionary<string, short>();
            
            var lines = _codeInput.text.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
            if (lines.Length == 0)
            {
                Logger.Send("No commands to assemble", Logger.MsgType.Error);
                return;
            }
            
            _cpuManager.ResetElements();
            _assemblyOutput.text = "";
            
            short memoryPointer = 0;
            AddVariables(ref memoryPointer, lines);
            if (lines.Length - memoryPointer > _memoryManager.Size)
            {
                Logger.Send("Not enough memory to store the program", Logger.MsgType.Error);
                return;
            }
            
            ParseInstructions(memoryPointer, lines);
        }
        
        public void Execute()
        {
            if (_isExecuting)
            {
                return;
            }
            
            _isExecuting = true;
            _assembleButton.interactable = false;
            _memoryManager.DeselectAll();
            _inputManager.Clear();
            _haltButton.SetActive(true);
            ValueDisplay.IsHighlightsEnabled = true;

            if (_workshopModeEnabled)
            {
                _inputManager.SetInput(WorkshopManager.CurrentWorkshop.Input);
            }
            
            var lines = _assemblyOutput.text.Split('\n');
            IEnumerator ExecuteCoroutine()
            {
                _cpuManager.ProgramCounter.Value = (short) _variables.Count;
                
                short instruction;
                while ((instruction = _memoryManager.Read(_cpuManager.ProgramCounter.Value)) != 0)
                {
                    _cpuManager.InstructionRegister.Value = instruction;
                    
                    var pc = _cpuManager.ProgramCounter.Value;
                    var (op, operand) = ReadInstruction(instruction);
                    lines[pc] += CURRENT_COMMAND_POINTER;
                    _assemblyOutput.text = string.Join('\n', lines);
                    yield return _cpuManager.ProcessCommand(op, operand);
                    lines[pc] = lines[pc].Replace(CURRENT_COMMAND_POINTER, "");
                    _assemblyOutput.text = string.Join('\n', lines);
                    _cpuManager.ProgramCounter.Value++;
                }
                
                _cpuManager.ProgramCounter.Value = 0;
                StopExecution();
                Logger.Send("Execution completed");
                yield return null;
            }
            
            StartCoroutine(ExecuteCoroutine());
        }
        
        public void Halt()
        {
            StopAllCoroutines();
            StopExecution();
            Logger.Send("Execution halted", Logger.MsgType.Warning);
        }

        private void StopExecution()
        {
            _isExecuting = false;
            _assembleButton.interactable = true;
            _assemblyOutput.text = _assemblyOutput.text.Replace(CURRENT_COMMAND_POINTER, "");
            _haltButton.SetActive(false);
            _inputManager.Clear();
            ValueDisplay.IsHighlightsEnabled = false;
            
            if (_workshopModeEnabled)
            {
                CheckWorkshop();
            }
        }

        private void CheckWorkshop()
        {
            var output = _outputManager.GetOutputText();
            
            var workshop = WorkshopManager.CurrentWorkshop;
            var completion = workshop.GetCompletionPercentage(_codeInput.text, output, _variables.Count, 
                out var partialSolution);
            WorkshopManager.UpdateProgress(workshop.ID, completion, _codeInput.text);
            _workshopCompletionText.text = $"Workshop completion: {completion}%";

            if (!partialSolution && completion < 100)
            {
                return;
            }
                
            ProjectListManager.CurrentMode = ProjectListManager.Mode.Workshops;
            _workshopCompleteMenu.SetActive(true);
        }

        private void AddVariables(ref short memoryPointer, IReadOnlyList<string> lines)
        {
            var variableAddresses = new short?[lines.Count];
            short count = 0;
            for (short i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                var parts = line.Split(' ');
                
                switch (parts.Length)
                {
                    case 0:
                        continue;
                    case > 3:
                        Logger.Send($"Invalid command: {line} at line {count}", Logger.MsgType.Error);
                        break;
                }

                if (System.Enum.TryParse(parts[0], out Operator _))
                {
                    if (parts.Length > 2)
                    {
                        Logger.Send($"Invalid command: {line} at line {count}", Logger.MsgType.Error);
                        break;
                    }
                    count++;
                    continue;
                }
                
                if (parts.Length == 1)
                {
                    Logger.Send($"Invalid command: {line} at line {count}", Logger.MsgType.Error);
                    break;
                }
                
                switch (parts[1])
                {
                    case DAT:
                        if (!_variables.TryAdd(parts[0], memoryPointer))
                        {
                            Logger.Send($"Repeated definition: {parts[0]} at line {count}", Logger.MsgType.Error);
                            break;
                        }
                        _assemblyOutput.text += $"{memoryPointer:00} {parts[0]} {DAT}\n";
                        if (parts.Length == 3)
                        {
                            if (TryParseOperand(parts[2], out var value, true))
                            {
                                _memoryManager.Write(memoryPointer, value);
                            }
                            else
                            {
                                Logger.Send($"Invalid DAT value at line {count}", Logger.MsgType.Error);
                                break;
                            }
                        }
                        memoryPointer++;
                        break;
                    case FN when _labels.TryAdd(parts[0], memoryPointer):
                        _assemblyOutput.text += $"{memoryPointer:00} {parts[0]} {FN}\n";
                        variableAddresses[memoryPointer] = count;
                        memoryPointer++;
                        break;
                    case FN:
                        Logger.Send($"Repeated definition: {parts[0]} at line {count}", Logger.MsgType.Error);
                        break;
                    default:
                        Logger.Send($"Invalid command: {line} at line {count}", Logger.MsgType.Error);
                        break;
                }
            }
            
            for (short i = 0; i < memoryPointer; i++)
            {
                if (variableAddresses[i] == null)
                {
                    continue;
                } 
                _memoryManager.Write(i, (short)(variableAddresses[i].Value + memoryPointer));
            }
        }

        private void ParseInstructions(short memoryPointer, IReadOnlyList<string> lines)
        {
            short count = 0;
            for (short i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var parts = line.Split(' ');
                if (parts.Length == 0)
                {
                    continue;
                }

                if (!System.Enum.TryParse(parts[0], out Operator op))
                {
                    if (parts[1] is not DAT and not FN)
                    {
                        Logger.Send($"Invalid operator: {parts[0]} at line {count}", Logger.MsgType.Error);
                        break;
                    }
                    continue;
                }

                if (parts.Length > 2)
                {
                    Logger.Send($"Invalid command: {line} at line {count}", Logger.MsgType.Error);
                    break;
                }
                
                short operand = 0;
                if (parts.Length == 2)
                {
                    if (TryParseOperand(parts[1], out operand))
                    {
                        if (op is not Operator.LDD)
                        {
                            Logger.Send($"No memory address for {op} at line {count}", Logger.MsgType.Error);
                            break;
                        }
                        line = $"{parts[0]} {operand}";
                    }
                    else if (_variables.TryGetValue(parts[1], out var value) || _labels.TryGetValue(parts[1], out value))
                    {
                        operand = value;
                    }
                    else
                    {
                        Logger.Send($"Invalid operand at line {count}", Logger.MsgType.Error);
                        break;
                    }
                }

                var instruction = GetInstruction(op, operand);
                var address = (short)(memoryPointer + count);
                _memoryManager.Write(address, instruction);
                
                _assemblyOutput.text += $"{address:00} {line}\n";
                count++;
            }

            _assemblyOutputAnimator.SetBool(IsOpenAnimatorBoolean, true);
        }

        private bool TryParseOperand(string operandString, out short operand, bool is16Bit = false)
        {
            var binaryLimit = is16Bit ? 17 : 9;
            var hexLimit = is16Bit ? 5 : 3;
            
            if (operandString.StartsWith('b') && operandString.Length <= binaryLimit)
            {
                operandString = operandString[1..];
                var binary = 0;
                for (var j = 0; j < operandString.Length; j++)
                {
                    if (operandString[j] == '1')
                    {
                        binary += 1 << operandString.Length - j - 1;
                    }
                }
                operand = (short) binary;
                return true;
            }

            if (operandString.StartsWith('x') && operandString.Length <= hexLimit)
            {
                return short.TryParse(operandString[1..], System.Globalization.NumberStyles.HexNumber, null, out operand);
            }

            return short.TryParse(operandString, out operand);
        }
        
        private static short GetInstruction(Operator op, short operand)
        {
            // first 2 decimal digits are the operator
            // last 3 decimal digits are the operand
            // e. g ADD 5 -> 1005
            // SUB 10 -> 2010
            // LDA 15 -> 6015
            // STA -20 -> -8020

            if (operand is < -999 or > 999)
            {
                Logger.Send($"Operand {operand} is out of range [-999; 999]", Logger.MsgType.Error);
                return 0;
            }

            var isNegative = operand < 0;
            if (isNegative) operand *= -1;
            var instruction = (short)((byte)op * 1000 + operand);
            return isNegative ? (short)(instruction * -1) : instruction;
        }
        
        private static (Operator, short) ReadInstruction(short instruction)
        {
            var isNegative = instruction < 0;
            if (isNegative) instruction *= -1;
            var op = (Operator)(instruction / 1000);
            if (instruction < 1000) op = 0;
            var operand = (short)(instruction % 1000);
            return (op, isNegative ? (short)(operand * -1) : operand);
        }
    }
}