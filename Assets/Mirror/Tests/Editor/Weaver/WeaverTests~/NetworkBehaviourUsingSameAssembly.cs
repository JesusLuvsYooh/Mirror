using Mirror;
using Mirror.Weaver.Tests.Extra;

namespace MirrorTest
{
    public class NetworkBehaviourUsingSameAssembly : NetworkBehaviour
    {
        [ClientRpc]
        public void RpcDoSomething(SomeOtherData data)
        {

        }
    }

    public struct SomeOtherData
    {
        public int usefulNumber;
    }
}
