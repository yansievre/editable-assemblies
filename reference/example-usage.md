# Example Usage

You can see an example script below. Keep in mind that this example will run constantly, locking your editor in a state of constantly refreshing. It's only meant to show how the class is used.

```csharp
using OpenLootBox.OpenAssembly.Scripts;
using UnityEditor;

namespace OpenLootBox.OpenAssembly.Example
{
    [InitializeOnLoad]
    public class ExampleScript
    {
        static ExampleScript()
        {
            EditableAssembly editableAssembly = EditableAssembly.FromAssemblyName("ExampleAssembly");
            if (editableAssembly == null) return;
            editableAssembly.SetIncludePlatforms(Platforms.Editor);
            editableAssembly.SetAssemblyReferences();
            editableAssembly.AllowUnsafeCode = false;
            editableAssembly.Save();
        }
    }
}
```
