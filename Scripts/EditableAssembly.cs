using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace OpenLootBox.OpenAssembly.Scripts
{
    public enum AssemblyReferenceMode
    {
        GUID,
        AssemblyName
    }

    [Serializable]
    public struct MockVersionDefine
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private string expression;
        [SerializeField]
        private string define;

        public MockVersionDefine(string name, string expression, string define)
        {
            this.name = name;
            this.expression = expression;
            this.define = define;
        }

        public string Define => define;

        public string Expression => expression;

        public string Name => name;
    }
    [Serializable]
    class MockAssembly
    {
        [SerializeField]
        internal string name;
        [SerializeField]
        internal string rootNamespace;
        [SerializeField]
        internal List<string> references;
        [SerializeField]
        internal List<string> includePlatforms;
        [SerializeField]
        internal List<string> excludePlatforms;
        [SerializeField]
        internal bool allowUnsafeCode;
        [SerializeField]
        internal bool overrideReferences;
        [SerializeField]
        internal List<string> precompiledReferences;
        [SerializeField]
        internal bool autoReferenced;
        [SerializeField]
        internal List<string> defineConstraints;
        [SerializeField]
        internal List<string> versionDefines;
        [SerializeField]
        internal bool noEngineReferences;

        public bool NoEngineReferences => noEngineReferences;

        public string[] VersionDefines => versionDefines.ToArray();

        public string[] DefineConstraints => defineConstraints.ToArray();

        public bool AutoReferenced => autoReferenced;

        public string[] PrecompiledReferences => precompiledReferences.ToArray();

        public bool OverrideReferences => overrideReferences;

        public bool AllowUnsafeCode => allowUnsafeCode;

        public string[] ExcludePlatforms => excludePlatforms.ToArray();

        public string[] IncludePlatforms => includePlatforms.ToArray();

        public string[] References => references.ToArray();

        public string RootNamespace => rootNamespace;

        public string Name => name;
    }
    
    public class EditableAssembly
    {
        public static EditableAssembly FromAsset([NotNull] AssemblyDefinitionAsset assemblyDefinitionAsset)
        {
            return new EditableAssembly(assemblyDefinitionAsset);
        }
        /// <summary>
        /// Will return null if assembly not found
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static EditableAssembly FromAssemblyName(string assemblyName)
        {
            try
            {
                return new EditableAssembly(assemblyName);
            }
            catch (Exception e)
            {
                // ignored
            }

            return null;
        }
        private static Regex _guidRegex = new Regex("(GUID:.*)");
        private MockAssembly _mockAssembly;
        public bool NoEngineReferences
        {
            get => _mockAssembly.noEngineReferences;
            set => _mockAssembly.noEngineReferences = value;
        }

        public bool AutoReferenced
        {
            get => _mockAssembly.autoReferenced;
            set => _mockAssembly.autoReferenced = value;
        }

        public bool OverrideReferences
        {
            get => _mockAssembly.overrideReferences;
            set => _mockAssembly.overrideReferences = value;
        }

        public bool AllowUnsafeCode
        {
            get => _mockAssembly.allowUnsafeCode;
            set => _mockAssembly.allowUnsafeCode = value;
        }

        public string RootNamespace
        {
            get => _mockAssembly.rootNamespace;
            set => _mockAssembly.rootNamespace = value;
        }

        public string Name
        {
            get => _mockAssembly.name;
            set => _mockAssembly.name = value;
        }

        public List<string> VersionDefines => _mockAssembly.versionDefines;

        public List<string> DefineConstraints => _mockAssembly.defineConstraints;

        public List<string> PrecompiledReferences => _mockAssembly.precompiledReferences;

        public List<string> ExcludePlatforms => _mockAssembly.excludePlatforms;

        public List<string> IncludePlatforms => _mockAssembly.includePlatforms;

        public List<string> References => _mockAssembly.references;

        private readonly string _assemblyPath;
        
        public EditableAssembly(string definitionAssetContents, string definitionAssetPath)
        {
            _mockAssembly = JsonUtility.FromJson<MockAssembly>(definitionAssetContents);
            _assemblyPath = definitionAssetPath;
        }

        private EditableAssembly(AssemblyDefinitionAsset definitionAsset) : this(definitionAsset.text,AssetDatabase.GetAssetPath(definitionAsset))
        {
        }
        
      
        private EditableAssembly(string definitionAssetName)
        {
            _assemblyPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(definitionAssetName);
            var asset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(_assemblyPath);
            _mockAssembly = JsonUtility.FromJson<MockAssembly>(asset.text);
        }
        

        public bool CanUseGuidForReference()
        {
            if (References.Count == 0) return true;
            var firstReference = References[0];
            //Checks if the first entry is a guid because UseGuids is not saved in the asset
            if (_guidRegex.IsMatch(firstReference))
            {
                return true;
            }

            return false;
        }
        
        public bool CanUseAssemblyNameForReference()
        {
            if (References.Count == 0) return true;
            return !CanUseGuidForReference();
        }
        
        /// <summary>
        /// Call CanUseAssemblyNameForReference() to make sure you can use this function.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public bool AddAssemblyToReferencesFromAssemblyName(string assemblyName)
        {
            if (!CanUseAssemblyNameForReference()) return false;
            bool isGuid = true;
            
            if (!_mockAssembly.references.Contains(assemblyName))
            {
                _mockAssembly.references.Add(assemblyName);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the GUID is in valid format.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private bool ValidateGuid(string guid)
        {
            return _guidRegex.IsMatch(guid);
        }

        /// <summary>
        /// Prepends "GUID:" to the beginning of the guid if possible
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public (bool success, string result) FixGuid(string guid)
        {
            bool isValid =  _guidRegex.IsMatch(guid);
            if (!isValid)
            {
                if (Guid.TryParse(guid,out var _))
                {
                    return (true,"GUID:"+guid);
                }
                return (false, guid);
            }

            return (true, guid);
        }
        
        /// <summary>
        /// Call CanUseAssemblyNameForReference() to make sure you can call this function
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool AddAssemblyToReferencesFromAssemblyDefinitionGuid(string guid)
        {
            if (!CanUseGuidForReference()) return false;
            if (!ValidateGuid(guid))
            {
                var res = FixGuid(guid);
                if (!res.success) return false;
                guid = res.result;
            }
            
            if (!_mockAssembly.references.Contains(guid))
            {
                _mockAssembly.references.Add(guid);
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Attempts to add an assembly definition to references
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool AddAssemblyToReferences(AssemblyDefinitionAsset asset)
        {
            if (CanUseGuidForReference())
            {
                var path = AssetDatabase.GetAssetPath(asset);
                var guid = AssetDatabase.AssetPathToGUID(path);
                guid = "GUID:" + guid.ToString();
                return AddAssemblyToReferencesFromAssemblyDefinitionGuid(guid);
            }
            var assemblyName = JsonUtility.FromJson<MockAssembly>(asset.text).Name;
            return AddAssemblyToReferencesFromAssemblyName(assemblyName);
        }

        /// <summary>
        /// Sets all the assembly references
        /// </summary>
        /// <param name="assets"></param>
        public void SetAssemblyReferences(params AssemblyDefinitionAsset[] assets)
        {
            SetAssemblyReferences(AssemblyReferenceMode.GUID, assets);
        }

        /// <summary>
        /// Sets all the assembly references
        /// </summary>
        /// <param name="referenceMode"></param>
        /// <param name="assets"></param>
        public void SetAssemblyReferences(AssemblyReferenceMode referenceMode, params AssemblyDefinitionAsset[] assets)
        {
            if (assets.Length == 0) return;
            _mockAssembly.references.Clear();
            if (referenceMode==AssemblyReferenceMode.AssemblyName)
            {
                var assemblyName = JsonUtility.FromJson<MockAssembly>(assets[0].text).Name;
                AddAssemblyToReferencesFromAssemblyName(assemblyName);
                int i = 0;
                //Remove the first one
                assets = assets.SkipWhile(x =>
                {
                    if (i == 0)
                    {
                        i = 1;
                        return true;
                    }
                    return false;
                }).ToArray();
            }
            foreach (var asset in assets)
            {
                AddAssemblyToReferences(asset);
            }
        }

        /// <summary>
        /// Only the provided platforms will be included
        /// </summary>
        /// <param name="platforms"></param>
        public void SetIncludePlatforms(Platforms platforms)
        {
            ExcludePlatforms.Clear();
            IncludePlatforms.Clear();
            foreach (Enum value in Enum.GetValues(typeof(Platforms)))
                if (platforms.HasFlag(value))
                    IncludePlatforms.Add(value.ToString());
        }

        /// <summary>
        /// Everything other than the provided platforms will be included
        /// </summary>
        /// <param name="platforms"></param>
        public void SetExcludePlatforms(Platforms platforms)
        {
            ExcludePlatforms.Clear();
            IncludePlatforms.Clear();
            foreach (Enum value in Enum.GetValues(typeof(Platforms)))
                if (platforms.HasFlag(value))
                    ExcludePlatforms.Add(value.ToString());
        }

        public void AddVersionDefine(MockVersionDefine versionDefine)
        {
            VersionDefines.Add(JsonUtility.ToJson(versionDefine));
        }

        public void RemoveVersionDefine(string resource)
        {
            VersionDefines.RemoveAll(x => resource == JsonUtility.FromJson<MockVersionDefine>(x).Name);
        }

        public MockVersionDefine[] GetReadableVersionDefines()
        {
            return VersionDefines.Select(JsonUtility.FromJson<MockVersionDefine>).ToArray();
        }
        public bool ArePlatformsIncluded(Platforms platforms)
        {
            if (IncludePlatforms.Count == 0 && ExcludePlatforms.Count == 0) return true;
            if (IncludePlatforms.Count > 0)
            {
                foreach (Enum value in Enum.GetValues(typeof(Platforms)))
                    if (platforms.HasFlag(value) && (!IncludePlatforms.Contains(value.ToString())))
                        return false;
                return true;
            }
            if (ExcludePlatforms.Count > 0)
            {
                foreach (Enum value in Enum.GetValues(typeof(Platforms)))
                    if (platforms.HasFlag(value) && (ExcludePlatforms.Contains(value.ToString())))
                        return false;
                return true;
            }
            Debug.LogError("Exception: Both 'excludePlatforms' and 'includePlatforms' are set.");
            return false;
        }

        public string GetAssemblyText()
        {
            return JsonUtility.ToJson(_mockAssembly);
        }
        
        public void Save(bool reimport = true)
        {
            using (StreamWriter sw = new StreamWriter(_assemblyPath))
            {
                sw.Write(JsonUtility.ToJson(_mockAssembly));
            }
            if(reimport)
                AssetDatabase.ImportAsset(_assemblyPath);
        }
    }
}
