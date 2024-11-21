using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpeedEffectsManager : MonoBehaviour
{
    [Header("Post-Processing Volume")]
    public Volume postProcessingVolume;

    [Header("Effect Curves")]
    public AnimationCurve motionBlurCurve;       // Curve for Motion Blur intensity
    public AnimationCurve chromaticAberrationCurve; // Curve for Chromatic Aberration intensity
    public AnimationCurve vignetteCurve;         // Curve for Vignette intensity

    private float maxSpeed;      // Speed at which effects are at full intensity

    private PlayerController playerController;

    private MotionBlur motionBlur;
    private ChromaticAberration chromaticAberration;
    private Vignette vignette;

    void Start()
    {
        AssignPlayer();
        maxSpeed = TypingManager.Instance.fastestCarSpeed / 2;

        // Cache post-processing effect components
        if (postProcessingVolume != null)
        {
            postProcessingVolume.profile.TryGet(out motionBlur);
            postProcessingVolume.profile.TryGet(out chromaticAberration);
            postProcessingVolume.profile.TryGet(out vignette);
        }
        else
        {
            Debug.LogError("Post-Processing Volume is not assigned!");
        }
    }

    void Update()
    {
        if (postProcessingVolume == null || playerController == null) return;

        // Get the current speed
        float speed = playerController.speed;

        // Normalize speed (0 to 1)
        float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);

        // Update Motion Blur intensity
        if (motionBlur != null)
        {
            motionBlur.intensity.value = motionBlurCurve.Evaluate(normalizedSpeed);
        }

        // Update Chromatic Aberration intensity
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = chromaticAberrationCurve.Evaluate(normalizedSpeed);
        }

        // Update Vignette intensity
        if (vignette != null)
        {
            vignette.intensity.value = vignetteCurve.Evaluate(normalizedSpeed);
        }
    }

    void AssignPlayer()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null) { Invoke(nameof(AssignPlayer), 1); }
    }
}
