# EditableAssembly

## Creation

To edit an assembly, you need to create an instance of the EditableAssembly class. There are two main ways of doing this.

```csharp
EditableAssembly.FromAsset([NotNull] AssemblyDefinitionAsset assemblyDefinitionAsset)
```

```
 EditableAssembly.FromAssemblyName(string assemblyName)
```

## Fields

It's recommended that you use methods instead of the fields for modification of lists.

* **bool** NoEngineReferences
* **bool** AutoReferenced
* **bool** OverrideReferences
* **bool** AllowUnsafeCode
* **string** RootNamespace
* **string** Name
* **List\<string>** VersionDefines
* **List\<string>** DefineConstraints
* **List\<string>** PrecompiledReferences
* **List\<string>** ExcludePlatforms&#x20;
* **List\<string>** IncludePlatforms
* **List\<string>** References

## Methods

<details>

<summary><mark style="color:yellow;"><strong>bool</strong></mark> <mark style="color:purple;">CanUseGuidForReference</mark>()</summary>

Checks if the **GUID** of an assembly definition asset can currently be used to add it as a reference to the assembly being edited.

</details>

<details>

<summary><mark style="color:yellow;"><strong>bool</strong></mark> <mark style="color:purple;">CanUseAssemblyNameForReference</mark>()</summary>

Checks if the **assembly name** of an assembly definition asset can currently be used to add it as a reference to the assembly being edited.

</details>

<details>

<summary><mark style="color:yellow;">bool</mark> <mark style="color:purple;">AddAssemblyToReferences</mark>(<mark style="color:orange;">AssemblyDefinitionAsset</mark> asset)</summary>

Will add the given assembly to the references, GUID takes priority. If the current elements of the reference list use assembly names, then the assembly name will be used. Otherwise GUID will be used.

</details>

<details>

<summary><mark style="color:yellow;">void</mark> <mark style="color:purple;">SetAssemblyReferences</mark>(<mark style="color:orange;">AssemblyReferenceMode</mark> referenceMode, <mark style="color:red;">params</mark> <mark style="color:orange;">AssemblyDefinitionAsset[]</mark> assets)</summary>

Clears the current references and adds the provided assets. The reference mode is to determine if GUID's or AssemblyName's will be used.&#x20;

</details>

<details>

<summary><mark style="color:yellow;">void</mark> <mark style="color:purple;">SetAssemblyReferences</mark>(<mark style="color:red;">params</mark> <mark style="color:orange;">AssemblyDefinitionAsset[]</mark> assets)</summary>

Shorthand for SetAssemblyReferences(AssemblyReferenceMode.GUID,assets);

</details>

<details>

<summary><mark style="color:yellow;">void</mark> <mark style="color:purple;">SetIncludePlatforms</mark>(<mark style="color:orange;">Platforms</mark> platforms)</summary>

Clears any previous settings. Sets the included platforms to the provided enum as bitflag.&#x20;

You can input multiple ones with the syntax:&#x20;

Platforms.iOS | Platforms.Editor

</details>

<details>

<summary><mark style="color:yellow;">void</mark> <mark style="color:purple;">SetExcludePlatforms</mark>(<mark style="color:orange;">Platforms</mark> platforms)</summary>

Clears any previous settings. Sets the excluded platforms to the provided enum as bitflag.&#x20;

You can input multiple ones with the syntax:&#x20;

Platforms.iOS | Platforms.Editor

</details>

<details>

<summary><mark style="color:yellow;">void</mark> <mark style="color:purple;">AddVersionDefine</mark>(<mark style="color:orange;">MockVersionDefine</mark> versionDefine)</summary>

Adds a version define from a mock class.

</details>

<details>

<summary><mark style="color:yellow;">void</mark> <mark style="color:purple;">RemoveVersionDefine</mark>(<mark style="color:orange;">string</mark> resource)</summary>

Removes a version define by finding it through its resource variable.

</details>

<details>

<summary><mark style="color:yellow;">MockVersionDefine[]</mark> <mark style="color:purple;">GetReadableVersionDefines</mark>()</summary>

Returns the current version defines as a new array.

</details>

<details>

<summary><mark style="color:yellow;">bool</mark> <mark style="color:purple;">ArePlatformsIncluded</mark>(<mark style="color:orange;">Platforms</mark> platforms)</summary>

Returns true if **every** platform in the given bitflag is included.

</details>

<details>

<summary><mark style="color:yellow;">string</mark> <mark style="color:purple;">GetAssemblyText</mark>()</summary>

Returns the JSON of the editable assembly in its current state.

</details>

<details>

<summary><mark style="color:yellow;">void</mark> <mark style="color:purple;">Save</mark>(<mark style="color:orange;">bool</mark> reimport <mark style="color:red;">= true</mark>)</summary>

Overwrites the assembly asset with the new values. If _reimport_ is set to true, it will force the **AssetDatabase** to reimport the assembly asset.

</details>

