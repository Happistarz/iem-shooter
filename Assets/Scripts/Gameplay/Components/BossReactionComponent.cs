using DG.Tweening;
using TMPro;
using UnityEngine;

public class BossReactionComponent : MonoBehaviour
{
    private static readonly int _EXTERNAL_GLITCH_ACTIVE = Shader.PropertyToID("_ExternalGlitchActive");
    private static readonly int _DISSOLVE_AMOUNT        = Shader.PropertyToID("_DissolveAmount");
    private static readonly int _COLOR                  = Shader.PropertyToID("_MainTint");
    private static readonly int _VERTICAL_FADE          = Shader.PropertyToID("_VerticalFade");

    private static readonly int _IDLE = Animator.StringToHash("Idle");

    [Header("References")]
    public Material bossMaterial;

    private Animator _bossAnimator;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("UI Elements")]
    public FadeMoveUIComponent topRectFadeMove;

    public FadeMoveUIComponent bottomRectFadeMove;
    public FadeMoveUIComponent renderTextureFadeMove;
    public FadeMoveUIComponent reactionTextFadeMove;
    public TextMeshProUGUI     reactionText;

    private bool _canQuit;

    public enum BossReactionType
    {
        Laugh,
        Angry
    }

    private void Start()
    {
        var bossObject                = GameObject.FindWithTag("BossHologram");
        if (bossObject) _bossAnimator = bossObject.GetComponent<Animator>();

        ResetReactionState();
        
        // DEBUG: auto start reaction for testing
        var debugReactionData = new BossReactionData
        {
            reactionType  = BossReactionType.Laugh,
            reactionClip  = null,
            reactionColor = Color.red,
            doGlitch      = true
        };
        PlayBossReaction(debugReactionData);
    }

    private void Update()
    {
        // press any key to fade out
        if (!Input.anyKeyDown || !_canQuit) return;

        FadeOutBossReaction();
        _canQuit = false;
    }

    private void ResetReactionState()
    {
        _canQuit = false;
        if (!bossMaterial) return;

        if (_bossAnimator)
        {
            // reset triggers
            foreach (var parameter in _bossAnimator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    _bossAnimator.ResetTrigger(parameter.name);
                }
            }

            _bossAnimator.SetTrigger(_IDLE);
        }

        bossMaterial.SetFloat(_EXTERNAL_GLITCH_ACTIVE, 0.0f);
        bossMaterial.SetFloat(_DISSOLVE_AMOUNT,        1.0f);
        bossMaterial.SetFloat(_VERTICAL_FADE,          0.0f);
        bossMaterial.SetColor(_COLOR, Color.white);
    }

    public void PlayBossReaction(BossReactionData reactionData)
    {
        if (!_bossAnimator) return;

        ResetReactionState();

        topRectFadeMove.FadeIn();
        bottomRectFadeMove.FadeIn();
        renderTextureFadeMove.FadeIn();
        reactionTextFadeMove.FadeIn();
        reactionText.DOFade(1.0f, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.6f);

        if (!bossMaterial) return;

        bossMaterial.DOFloat(0.0f, _DISSOLVE_AMOUNT, 0.3f).SetEase(Ease.OutQuad);
        bossMaterial.DOFloat(1.0f, _VERTICAL_FADE, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.2f)
                    .OnComplete(() => { PlayReaction(reactionData); });
    }

    private void PlayReaction(BossReactionData reactionData)
    {
        if (_bossAnimator) _bossAnimator.ResetTrigger(_IDLE);

        audioSource.PlayOneShot(reactionData.reactionClip);
        if (reactionData.doGlitch) GlitchEffect();
        ChangeColorEffect(reactionData.reactionColor);
        _bossAnimator.SetTrigger(reactionData.reactionType.ToString());

        DOVirtual.DelayedCall(1.5f, () => { _canQuit = true; });
    }

    public void FadeOutBossReaction()
    {
        topRectFadeMove.FadeOut();
        bottomRectFadeMove.FadeOut();
        renderTextureFadeMove.FadeOut();
        reactionTextFadeMove.FadeOut();
        reactionText.DOFade(0.0f, 0.2f).SetEase(Ease.OutQuad).SetDelay(0.0f);

        // Start dissolve effect and vertical fade
        if (!bossMaterial) return;

        bossMaterial.DOFloat(1.0f, _DISSOLVE_AMOUNT, 0.7f).SetEase(Ease.InQuad);
        bossMaterial.DOFloat(0.0f, _VERTICAL_FADE, 0.7f).SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        if (_bossAnimator) _bossAnimator.SetTrigger(_IDLE);
                        gameObject.SetActive(false);
                    });
    }

    private void GlitchEffect()
    {
        if (!bossMaterial) return;

        DOTween.To(
            () => bossMaterial.GetFloat(_EXTERNAL_GLITCH_ACTIVE),
            x => bossMaterial.SetFloat(_EXTERNAL_GLITCH_ACTIVE, x),
            1.0f,
            0.5f).SetLoops(2, LoopType.Yoyo);
    }

    private void ChangeColorEffect(Color targetColor)
    {
        if (!bossMaterial) return;

        bossMaterial.DOColor(targetColor, _COLOR, 0.5f);
    }

    private void OnDisable()
    {
        // visible reset to avoid leaking material changes in the editor
        if (!bossMaterial) return;
        bossMaterial.SetFloat(_EXTERNAL_GLITCH_ACTIVE, 0.0f);
        bossMaterial.SetFloat(_DISSOLVE_AMOUNT,        0.0f);
        bossMaterial.SetFloat(_VERTICAL_FADE,          1.0f);
    }
}