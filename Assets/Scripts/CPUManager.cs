using System.Collections;
using UnityEngine;
using static Danqzq.Operator;

namespace Danqzq
{
    public class CPUManager : MonoBehaviour
    {
        [Header("Registers")]
        [field: SerializeField] public Register AddressRegister { get; private set; }
        [field: SerializeField] public Register BufferRegister { get; private set; }
        [field: SerializeField] public Register InstructionRegister { get; private set; }
        [field: SerializeField] public Register ProgramCounter { get; private set; }

        [Header("Other Components")]
        [SerializeField] private Accumulator _accumulator;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private MemoryManager _memoryManager;
        [SerializeField] private OutputManager _outputManager;
        
        [Header("Execution Speed")]
        [SerializeField] private bool _isRunWithDelay;
        [SerializeField] private float _delay;
        
        private bool _isClearMemoryOnReset = true;
        
        public void SetDelay(string delay)
        {
            if (!float.TryParse(delay, out var v))
            {
                return;
            }
            
            _delay = v;
            if (v != 0)
            {
                ValueDisplay.HighlightDelay = v;
            }
        }
        
        private void Start()
        {
            ResetElements();
        }

        public void ResetElements()
        {
            AddressRegister.Init(0);
            BufferRegister.Init(0);
            InstructionRegister.Init(0);
            ProgramCounter.Init(0);
            
            _accumulator.Clear();
            _inputManager.Clear();
            _outputManager.Clear();

            if (_isClearMemoryOnReset)
            {
                _memoryManager.Clear();
            }
        }

        public IEnumerator ProcessCommand(Operator op, short operand)
        {
            yield return _isRunWithDelay ? new WaitForSeconds(_delay) : null;
            switch (op)
            {
                case LDA:
                    AddressRegister.Value = operand;
                    _accumulator.Load(_memoryManager.Read(operand));
                    break;
                case LDD:
                    _accumulator.Load(operand);
                    break;
                case ADD:
                    _accumulator.Add(_memoryManager.Read(operand));
                    break;
                case SUB:
                    _accumulator.Subtract(_memoryManager.Read(operand));
                    break;
                case MUL:
                    _accumulator.Multiply(_memoryManager.Read(operand));
                    break;
                case DIV:
                    _accumulator.Divide(_memoryManager.Read(operand));
                    break;
                case MOD:
                    _accumulator.Modulus(_memoryManager.Read(operand));
                    break;
                case STA:
                    AddressRegister.Value = operand;
                    BufferRegister.Value = _accumulator.Value;
                    _memoryManager.Write(AddressRegister.Value, BufferRegister.Value);
                    break;
                case JMP:
                    ProgramCounter.Value = _memoryManager.Read(operand);
                    ProgramCounter.Value--;
                    break;
                case JEQ when _accumulator.Value == 0:
                    ProgramCounter.Value = _memoryManager.Read(operand);
                    ProgramCounter.Value--;
                    break;
                case JNE when _accumulator.Value != 0:
                    ProgramCounter.Value = _memoryManager.Read(operand);
                    ProgramCounter.Value--;
                    break;
                case JGT when _accumulator.Value > 0:
                    ProgramCounter.Value = _memoryManager.Read(operand);
                    ProgramCounter.Value--;
                    break;
                case JLT when _accumulator.Value < 0:
                    ProgramCounter.Value = _memoryManager.Read(operand);
                    ProgramCounter.Value--;
                    break;
                case INP:
                    yield return _inputManager.Read();
                    _accumulator.Load(_inputManager.Value);
                    break;
                case OUT:
                    _outputManager.Output(_accumulator.Value);
                    break;
                case OTC:
                    _outputManager.OutputChar(_accumulator.Value);
                    break;
                case CLR:
                    _outputManager.Clear();
                    break;
                case HLT:
                default:
                    break;
            }
        }
    }
}