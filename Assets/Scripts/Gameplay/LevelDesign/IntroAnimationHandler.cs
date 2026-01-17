using UnityEngine;

public class IntroAnimationHandler : MonoBehaviour
{
    private static readonly int _ANGRY = Animator.StringToHash("Angry");

    public AudioSource  flybyAudioSource;
    public IntroManager introManager;
    public CameraShake  cameraShake;

    public Animator    bossAnimator;
    public AudioSource bossAngryAudioSource;

    public void PlayFlyBySound()
    {
        flybyAudioSource?.Play();
    }

    public void ShakeCamera()
    {
        cameraShake?.Shake();
    }

    public void PlayBossAngry()
    {
        bossAnimator?.SetTrigger(_ANGRY);
        bossAngryAudioSource?.Play();
    }

    public void EndIntroAnimation()
    {
        introManager?.StartGameTransition();
    }
}