using Mirror;
using Mirror.Weaver.Tests.Extra;

namespace MirrorTest
{
    public class NetworkBehaviourUsingAnotherAssembly : NetworkBehaviour
    {
        [ClientRpc]
        public void RpcDoSomething(SomeData data)
        {

        }
    }
}