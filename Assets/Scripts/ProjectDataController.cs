using Danqzq.Models;
using UnityEngine;

namespace Danqzq
{
    public class ProjectDataController : MonoBehaviour
    {
        [SerializeField] private Assembler _assembler;
        [SerializeField] private MemoryManager _memoryManager;

        [SerializeField] private DeviceComponent[] _deviceComponents;
        
        private void Start()
        {
            if (ProjectManager.CurrentProject != null && !string.IsNullOrEmpty(ProjectManager.CurrentProject.projectData))
            {
                var projectData = JsonUtility.FromJson<ProjectData>(ProjectManager.CurrentProject.projectData);
                Import(projectData);
            }
        }

        public void Import(ProjectData projectData)
        {
            _assembler.SetAssemblyLines(projectData.assemblyLines);
            _memoryManager.Clear();
            for (int i = 0; i < projectData.memory.Length; i++)
            {
                _memoryManager.Write((short)i, (short)projectData.memory[i]);
            }
            if (projectData.deviceComponents == null)
            {
                return;
            }
            for (int i = 0; i < projectData.deviceComponents.Length; i++)
            {
                _deviceComponents[i].Import(projectData.deviceComponents[i]);
            }
        }
        
        public ProjectData Export()
        {
            var projectData = new ProjectData
            {
                assemblyLines = _assembler.GetAssemblyLines(),
                memory = new int[_memoryManager.Size]
            };
            
            for (int i = 0; i < projectData.memory.Length; i++)
            {
                projectData.memory[i] = _memoryManager.Read((short)i);
            }
            
            projectData.deviceComponents = new DeviceComponentData[_deviceComponents.Length];
            for (int i = 0; i < projectData.deviceComponents.Length; i++)
            {
                projectData.deviceComponents[i] = _deviceComponents[i].Export();
            }
            
            return projectData;
        }
        
        public string ExportToJson()
        {
            return JsonUtility.ToJson(Export());
        }
    }
}