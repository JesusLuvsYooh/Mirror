using Mirror;
using Mirror.Weaver.Tests.Extra;

namespace MirrorTest
{
    public class NetworkBehaviourUsingAnotherAssemblyWithWriter : NetworkBehaviour
    {
        [ClientRpc]
        public void RpcDoSomething(SomeDataWithWriter data)
        {

        }
    }
}