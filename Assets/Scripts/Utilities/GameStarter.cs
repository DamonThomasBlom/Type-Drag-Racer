using Fusion;
using UnityEngine;

public class GameStarter : NetworkBehaviour
{
    public GameObject startButton;

    public override void Spawned()
    {
        base.Spawned();

        if (HasStateAuthority)
        {
            startButton.SetActive(true);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void StartGameRpc()
    {
        TypingManager.Instance.gameStarted = true;
    }
}
