using OpenLootBox.OpenAssembly.Scripts;
using UnityEditor;

namespace OpenLootBox.OpenAssembly.Example
{
    [InitializeOnLoad]
    public class ExampleScript
    {
        static ExampleScript()
        {
            /* This will run constantly, locking your editor in a state of constantly refreshing.
               So don't uncomment this
             
            EditableAssembly editableAssembly = EditableAssembly.FromAssemblyName("ExampleAssembly");
            if (editableAssembly == null) return;
            editableAssembly.SetIncludePlatforms(Platforms.Editor);
            editableAssembly.SetAssemblyReferences();
            editableAssembly.AllowUnsafeCode = false;
            editableAssembly.Save();
            */
        }
    }
}
