using Fusion;
using UnityEngine;

public class NetworkPlayerSpawnPoint : NetworkBehaviour
{
    public Vector3 spawnPoint;

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void AssignSpawnPointRpc(Vector3 position, [RpcTarget] PlayerRef player)
    {
        spawnPoint = position;
        Invoke(nameof(AssingSpawnPointDelayed), 1);
    }

    void AssingSpawnPointDelayed()
    {
        SpawnpointManager.Instance.AssignSpawnPoint(spawnPoint);
    }
}
