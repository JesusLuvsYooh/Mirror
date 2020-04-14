using NUnit.Framework;

namespace Mirror.Weaver.Tests
{
    public class WeaverTestMutlipleAssemblies : WeaverTests
    {
        [SetUp]
        public void TestSetup()
        {
            WeaverAssembler.AddReferencesByAssemblyName(new string[] { "WeaverTestExtraAssembly.dll" });
        }

        [Test]
        public void WeaverCanUseTypesFromSameAssembly()
        {
            BuildAndWeaveTestAssembly("NetworkBehaviourUsingSameAssembly");

            Assert.That(CompilationFinishedHook.WeaveFailed, Is.False);
            Assert.That(weaverErrors, Is.Empty);
        }

        [Test]
        [Ignore("Not Implemented")]
        public void WeaverDoesSomethingUnityTypes()
        {
            // Need to decide if scriptableObjects are allowed
            // eg RpcDoSomething(DataScriptableObject data)
        }


        [Test]
        public void WeaverCanUseTypesFromDifferentAssemblies()
        {
            BuildAndWeaveTestAssembly("NetworkBehaviourUsingAnotherAssembly");

            Assert.That(CompilationFinishedHook.WeaveFailed, Is.False);
            Assert.That(weaverErrors, Is.Empty);
        }

        [Test]
        public void WeaverCanUseTypesFromDifferentAssembliesWithCustomReadWrite()
        {
            BuildAndWeaveTestAssembly("NetworkBehaviourUsingAnotherAssemblyWithWriter");

            Assert.That(CompilationFinishedHook.WeaveFailed, Is.False);
            Assert.That(weaverErrors, Is.Empty);
        }
    }
}
