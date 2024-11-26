using Fusion;
using UnityEngine;

public class CustomNetworkTransform : NetworkBehaviour, IStateAuthorityChanged
{
    [Networked] public Vector3 Position { get; set; }
    [Networked] public Quaternion Rotation { get; set; }
    [Networked] public Vector3 Velocity { get; set; }

    [Header("Interpolation Settings")]
    public float LerpSpeed = 5f; // Interpolation speed for smooth movement
    public float SnapThreshold = 1f; // Distance to teleport instead of interpolate
     public float ExtrapolationTime = 0.2f; // Aggressiveness of extrapolation
    public int VelocitySmoothingFrames = 5; // Number of frames for rolling average

    private bool _isMine; // Cached ownership state
    private bool _isSpawned; // Ensures logic only runs after spawning
    private Vector3[] velocityBuffer; // Buffer for rolling average
    private int bufferIndex = 0;

    public override void Spawned()
    {
        base.Spawned();
        UpdateIsMine();
        _isSpawned = true;

        // Initialize velocity buffer
        velocityBuffer = new Vector3[VelocitySmoothingFrames];
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
        // Update velocity with a rolling average
        //Vector3 currentVelocity = (transform.position - Position);
        //AddToVelocityBuffer(currentVelocity);
        //Velocity = GetRollingAverageVelocity();

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

        // Predict where the car should be
        //Vector3 predictedPosition = Position + (Velocity * ExtrapolationTime);

        // Teleport if the object is significantly out of sync
        if (distance > SnapThreshold)
        {
            transform.position = Position;
            transform.rotation = Rotation;
        }
        else
        {
            // Interpolate position and rotation for smooth synchronization
            //transform.position = Vector3.Lerp(transform.position, predictedPosition, Time.deltaTime * LerpSpeed);
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

    private void AddToVelocityBuffer(Vector3 velocity)
    {
        velocityBuffer[bufferIndex] = velocity;
        bufferIndex = (bufferIndex + 1) % VelocitySmoothingFrames;
    }

    private Vector3 GetRollingAverageVelocity()
    {
        Vector3 average = Vector3.zero;
        for (int i = 0; i < VelocitySmoothingFrames; i++)
        {
            average += velocityBuffer[i];
        }
        return average / VelocitySmoothingFrames;
    }
}
