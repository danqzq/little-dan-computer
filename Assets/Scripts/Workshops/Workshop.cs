using UnityEngine;

namespace Danqzq.Workshops
{
    [CreateAssetMenu(fileName = "Workshop", menuName = "New Workshop", order = 0)]
    public class Workshop : ScriptableObject
    {
        public int ID { get; set; }
        
        [field: SerializeField] public string Name { get; private set; }
        
        [field: TextArea(3, 5), SerializeField] public string Description { get; private set; }
        [field: TextArea(3, 5), SerializeField] public string BaseCode { get; private set; }
        [field: SerializeField] public string[] Input { get; private set; }
        
        [field: SerializeField] public string[] Hints { get; private set; }

        [TextArea(3, 5), SerializeField] private string _expectedOutput;
        
        [System.Serializable]
        public struct Tasks
        {
            public int variableRequirement;
            public int branchingRequirement;
            public string formattedOutputRequirement;
        }
        
        [field: SerializeField] public Tasks BonusTasks { get; private set; }
        
        public byte GetCompletionPercentage(string code, string output, int varCount, out bool partialSolution)
        {
            partialSolution = false;
            
            var satisfiedRequirements = 0;
            var totalRequirements = 1;
            if (string.IsNullOrEmpty(BonusTasks.formattedOutputRequirement))
            {
                if (output == _expectedOutput)
                {
                    satisfiedRequirements++;
                    partialSolution = true;
                }
            }
            else
            {
                totalRequirements++;
                if (output == BonusTasks.formattedOutputRequirement)
                {
                    satisfiedRequirements += 2;
                } 
                else if (output == _expectedOutput)
                {
                    satisfiedRequirements++;
                    partialSolution = true;
                }
            }
            
            if (BonusTasks.variableRequirement > 0)
            {
                totalRequirements++;
                if (varCount <= BonusTasks.variableRequirement)
                {
                    satisfiedRequirements++;
                }
            }
            
            if (BonusTasks.branchingRequirement > 0)
            {
                totalRequirements++;
                var branchingOperators = new[] { "JMP", "JEQ", "JNE", "JGT", "JLT" };
                var branchingCount = code.Split(branchingOperators, System.StringSplitOptions.None).Length - 1;
                if (branchingCount <= BonusTasks.branchingRequirement)
                {
                    satisfiedRequirements++;
                }
            }
            
            return (byte) Mathf.RoundToInt((float) satisfiedRequirements / totalRequirements * 100);
        }
    }
}