using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using UnityEditorInternal;
using UnityEngine;
using WorkflowToolkit.EditableAssemblies;

namespace WorkflowToolkit.Editor.EditableAssemblyTest
{
 
    [Serializable]
    class MockAssembly
    {
        [SerializeField]
        internal string name;
        [SerializeField]
        internal string rootNamespace;
        [SerializeField]
        internal List<string> references = new List<string>();
        [SerializeField]
        internal List<string> includePlatforms = new List<string>();
        [SerializeField]
        internal List<string> excludePlatforms = new List<string>();
        [SerializeField]
        internal bool allowUnsafeCode;
        [SerializeField]
        internal bool overrideReferences;
        [SerializeField]
        internal List<string> precompiledReferences = new List<string>();
        [SerializeField]
        internal bool autoReferenced;
        [SerializeField]
        internal List<string> defineConstraints = new List<string>();
        [SerializeField]
        internal List<string> versionDefines = new List<string>();
        [SerializeField]
        internal bool noEngineReferences;
        public MockAssembly()
        {
        }

        public MockAssembly(string json)
        {
            JsonUtility.FromJsonOverwrite(json,this);
        }

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

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
    public class EditableAssemblyTests
    {
        private const string GenericGUID = "GUID:27619889b8ba8c24980f49ee34dbb44a";
        [SetUp]
        public void Setup()
        {
            
        }
        
        [Test]
        public void NameChange()
        {
            var mockAssembly = new MockAssembly()
            {
                name = "Name1"
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            edit.Name = "Name2";
            var newAssembly = new MockAssembly(edit.GetAssemblyText());
            Assert.That(newAssembly.Name == "Name2");
        }

        [Test]
        public void NamespaceChange()
        {
            var mockAssembly = new MockAssembly()
            {
                rootNamespace = "Name1"
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            edit.RootNamespace = "Name2";
            var newAssembly = new MockAssembly(edit.GetAssemblyText());
            Assert.That(newAssembly.rootNamespace == "Name2");
        }
        
        
        [Test]
        public void AllowUnsafeCode()
        {
            var mockAssembly = new MockAssembly()
            {
                allowUnsafeCode = false
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            edit.AllowUnsafeCode = true;
            var newAssembly = new MockAssembly(edit.GetAssemblyText());
            Assert.That(newAssembly.allowUnsafeCode == true);
        }
        
        [Test]
        public void OverrideReferences()
        {
            var mockAssembly = new MockAssembly()
            {
                overrideReferences = false
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            edit.OverrideReferences = true;
            var newAssembly = new MockAssembly(edit.GetAssemblyText());
            Assert.That(newAssembly.overrideReferences == true);
        }
        
        [Test]
        public void AutoReferenced()
        {
            var mockAssembly = new MockAssembly()
            {
                autoReferenced = false
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            edit.AutoReferenced = true;
            var newAssembly = new MockAssembly(edit.GetAssemblyText());
            Assert.That(newAssembly.autoReferenced == true);
        }
        
        [Test]
        public void NoEngineReferences()
        {
            var mockAssembly = new MockAssembly()
            {
                noEngineReferences = false
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            edit.NoEngineReferences = true;
            var newAssembly = new MockAssembly(edit.GetAssemblyText());
            Assert.That(newAssembly.noEngineReferences == true);
        }
        
     
        [Test]
        public void CanAddGuidTest()
        {
            var mockAssembly = new MockAssembly()
            {
                
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            Assert.That(edit.CanUseGuidForReference);
            edit.AddAssemblyToReferencesFromAssemblyDefinitionGuid(GenericGUID);
            Assert.That(edit.CanUseGuidForReference);
            //Assembly name usage should be disabled after using guid
            Assert.That(!edit.CanUseAssemblyNameForReference());
        }
        [Test]
        public void CanAddAssemblyNameTest()
        {
            var mockAssembly = new MockAssembly()
            {
                
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            Assert.That(edit.CanUseAssemblyNameForReference);
            edit.AddAssemblyToReferencesFromAssemblyName("AssemblyName");
            Assert.That(!edit.CanUseGuidForReference());
            //Assembly name usage should be disabled after using guid
            Assert.That(edit.CanUseAssemblyNameForReference());
        }
        [Test]
        public void InvalidGuidTest()
        {
            var mockAssembly = new MockAssembly()
            {
                
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            
            var result = edit.AddAssemblyToReferencesFromAssemblyDefinitionGuid("GUD:27619889b8ba8c24980f49ee34dbb44a");
            Assert.That(!result);
            result = edit.AddAssemblyToReferencesFromAssemblyDefinitionGuid("24980f49ee34dbb44a");
            Assert.That(!result);
            result = edit.AddAssemblyToReferencesFromAssemblyDefinitionGuid("GUID");
            Assert.That(!result);
        }
        
        [Test]
        public void AddReferencesTest()
        {
            var mockAssembly = new MockAssembly()
            {
                
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            edit.AddAssemblyToReferencesFromAssemblyName("Assembly1");
            var newAssembly = new MockAssembly(edit.GetAssemblyText());
            Assert.That(newAssembly.References.Contains("Assembly1"));
            
            edit = new EditableAssembly(newAssembly.ToString(),"");
            edit.AddAssemblyToReferencesFromAssemblyName("Assembly2");
            edit.AddAssemblyToReferencesFromAssemblyName("Assembly3");
            newAssembly = new MockAssembly(edit.GetAssemblyText());
            Assert.That(newAssembly.References.Contains("Assembly1"));
            Assert.That(newAssembly.References.Contains("Assembly2"));
            Assert.That(newAssembly.References.Contains("Assembly3"));
        }

        [Test]
        public void PlatformsIncludedTest()
        {
            var mockAssembly = new MockAssembly()
            {
                includePlatforms = new List<string>()
                {
                    Platforms.Android.ToString(),
                    Platforms.iOS.ToString(),
                    Platforms.Lumin.ToString(),
                    Platforms.Stadia.ToString(),
                }
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            Assert.That(edit.ArePlatformsIncluded(Platforms.Android));
            Assert.That(edit.ArePlatformsIncluded(Platforms.iOS));
            Assert.That(edit.ArePlatformsIncluded(Platforms.Lumin));
            Assert.That(edit.ArePlatformsIncluded(Platforms.Stadia));
            
            
            Assert.That(edit.ArePlatformsIncluded(Platforms.Android | Platforms.iOS));
            Assert.That(edit.ArePlatformsIncluded(Platforms.Lumin | Platforms.Stadia));
            Assert.That(edit.ArePlatformsIncluded(Platforms.Android | Platforms.iOS | Platforms.Stadia | Platforms.Lumin));
            
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Switch));
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Switch | Platforms.PS4));
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Switch | Platforms.Android));
        }
        
        [Test]
        public void PlatformsExcludedTest()
        {
            var mockAssembly = new MockAssembly()
            {
                excludePlatforms = new List<string>()
                {
                    Platforms.Android.ToString(),
                    Platforms.iOS.ToString(),
                    Platforms.Lumin.ToString(),
                    Platforms.Stadia.ToString(),
                }
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Android));
            Assert.That(!edit.ArePlatformsIncluded(Platforms.iOS));
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Lumin));
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Stadia));
            
            
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Android | Platforms.iOS));
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Lumin | Platforms.Stadia));
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Android | Platforms.iOS | Platforms.Stadia | Platforms.Lumin));
            
            Assert.That(edit.ArePlatformsIncluded(Platforms.Switch));
            Assert.That(edit.ArePlatformsIncluded(Platforms.Switch | Platforms.PS4));
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Switch | Platforms.Android));
        }
        
        
        [Test]
        public void IncludePlatformsTest()
        {
            var mockAssembly = new MockAssembly()
            {
               
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            edit.SetIncludePlatforms(Platforms.Android | Platforms.iOS);
            
            
            Assert.That(edit.ArePlatformsIncluded(Platforms.Android));
            Assert.That(edit.ArePlatformsIncluded(Platforms.iOS));
            Assert.That(edit.ArePlatformsIncluded(Platforms.iOS | Platforms.Android));
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Switch | Platforms.Android));
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Switch));
        }
        
        [Test]
        public void ExcludePlatformsTest()
        {
            var mockAssembly = new MockAssembly()
            {
               
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            edit.SetExcludePlatforms(Platforms.Android | Platforms.iOS);
            
            
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Android));
            Assert.That(!edit.ArePlatformsIncluded(Platforms.iOS));
            Assert.That(!edit.ArePlatformsIncluded(Platforms.iOS | Platforms.Android));
            Assert.That(!edit.ArePlatformsIncluded(Platforms.Switch | Platforms.Android));
            Assert.That(edit.ArePlatformsIncluded(Platforms.Switch));
            Assert.That(edit.ArePlatformsIncluded(Platforms.Stadia));
            Assert.That(edit.ArePlatformsIncluded(Platforms.Stadia | Platforms.LinuxStandalone64));
        }

        [Test]
        public void VersionDefineTest()
        {
            var mockAssembly = new MockAssembly()
            {
               
            };
            var edit = new EditableAssembly(mockAssembly.ToString(),"");
            edit.AddVersionDefine(new MockVersionDefine("testName","",""));
            edit.AddVersionDefine(new MockVersionDefine("testName2","",""));
            var defines = edit.GetReadableVersionDefines();
            Assert.That(defines.Any(x=>x.Name=="testName"));
            Assert.That(defines.Any(x=>x.Name=="testName2"));
            var st = edit.GetAssemblyText();
            var newEdit = new EditableAssembly(st, "");
            
            defines = newEdit.GetReadableVersionDefines();
            Assert.That(defines.Any(x=>x.Name=="testName"));
            Assert.That(defines.Any(x=>x.Name=="testName2"));
            newEdit.RemoveVersionDefine("testName");
            defines = newEdit.GetReadableVersionDefines();
            Assert.That(defines.All(x => x.Name != "testName"));

        }
    }
    
    
}
