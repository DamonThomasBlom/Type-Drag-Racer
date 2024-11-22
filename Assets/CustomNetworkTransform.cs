using Fusion;
using UnityEngine;

public class CustomNetworkTransform : NetworkBehaviour, IStateAuthorityChanged
{
    [Networked] public Vector3 Position { get; set; }
    [Networked] public Quaternion Rotation { get; set; }

    [Header("Interpolation Settings")]
    public float LerpSpeed = 5f; // Interpolation speed for smooth movement
    public float SnapThreshold = 1f; // Distance to teleport instead of interpolate

    private bool _isMine; // Cached ownership state
    private bool _isSpawned; // Ensures logic only runs after spawning

    public override void Spawned()
    {
        base.Spawned();
        UpdateIsMine();
        _isSpawned = true;
    }

    private void Update()
    {
        if (!_isSpawned) return;

        if (_isMine)
        {
            HandleLocalAuthority();
        }
        else
        {
            HandleRemoteUpdates();
        }
    }

    private void HandleLocalAuthority()
    {
        // Update networked properties only when changes occur
        if (Vector3.Distance(Position, transform.position) > Mathf.Epsilon)
        {
            Position = transform.position;
        }

        if (Quaternion.Angle(Rotation, transform.rotation) > Mathf.Epsilon)
        {
            Rotation = transform.rotation;
        }
    }

    private void HandleRemoteUpdates()
    {
        float distance = Vector3.Distance(transform.position, Position);

        // Teleport if the object is significantly out of sync
        if (distance > SnapThreshold)
        {
            transform.position = Position;
            transform.rotation = Rotation;
        }
        else
        {
            // Interpolate position and rotation for smooth synchronization
            transform.position = Vector3.Lerp(transform.position, Position, Time.deltaTime * LerpSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Rotation, Time.deltaTime * LerpSpeed);
        }
    }

    public void StateAuthorityChanged()
    {
        UpdateIsMine();
    }

    void UpdateIsMine()
    {
        _isMine = Object != null && Object == Object.HasStateAuthority;
    }
}
