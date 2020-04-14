using Mirror;
using Mirror.Weaver.Tests.Extra;

namespace MirrorTest
{
    public class NetworkBehaviourUsingSameAssembly : NetworkBehaviour
    {
        [ClientRpc]
        public void RpcDoSomething(DataScriptableObject data)
        {

        }
    }

    public class DataScriptableObject :ScriptableObject
    {
        public int usefulNumber;
    }
}
