using Mirror;

public class NetworkBehaviourUsingAnotherAssembly : NetworkBehaviour
{
    [ClientRpc]
    public void RpcDoSomething(SomeDataWithWriter data)
    {

    }
}
