using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum SelectionResult
    {
        None,
        Pass,
        UpgradeA,
        UpgradeB,
    }

    [FormerlySerializedAs("TitleText")]  public GameObject titleText;
    [FormerlySerializedAs("SelectText")] public GameObject selectText;

    [FormerlySerializedAs("NoUpgradeText")]
    public GameObject noUpgradeText;

    [FormerlySerializedAs("SelectWeaponNameText")]
    public TextMeshProUGUI selectWeaponNameText;

    [FormerlySerializedAs("UpgradeButtonA")]
    public Button upgradeButtonA;

    [FormerlySerializedAs("UpgradeButtonAText")]
    public TextMeshProUGUI upgradeButtonAText;

    [FormerlySerializedAs("UpgradeButtonB")]
    public Button upgradeButtonB;

    [FormerlySerializedAs("UpgradeButtonBText")]
    public TextMeshProUGUI upgradeButtonBText;

    [FormerlySerializedAs("PassButton")] public Button passButton;

    [FormerlySerializedAs("DamageOverlayImage")]
    public Image damageOverlayImage;

    [FormerlySerializedAs("HealthUIComponent")]
    public HealthUIComponent healthUIComponent;

    [FormerlySerializedAs("BossReactionComponent")]
    public BossReactionComponent bossReactionComponent;

    [FormerlySerializedAs("Selection")] public SelectionResult selection;

    private Canvas _canvas;

    private void Awake()
    {
        Game.UI = this;
    }

    public void Start()
    {
        _canvas = FindFirstObjectByType<Canvas>();
        HideAll();
    }

    public void HideAll()
    {
        titleText.SetActive(false);
        selectText.SetActive(false);
        selectWeaponNameText.gameObject.SetActive(false);
        noUpgradeText.SetActive(false);
        upgradeButtonA.gameObject.SetActive(false);
        upgradeButtonB.gameObject.SetActive(false);
        passButton.gameObject.SetActive(false);
        damageOverlayImage.gameObject.SetActive(false);
        healthUIComponent.gameObject.SetActive(false);
        // BossReactionComponent.gameObject.SetActive(false);

        selection = SelectionResult.None;
    }

    public void ShowBossReaction(BossReactionData data)
    {
        bossReactionComponent.gameObject.SetActive(true);
        bossReactionComponent.PlayBossReaction(data);
    }

    public void ShowHealth(float currentHealth, float maxHealth)
    {
        healthUIComponent.gameObject.SetActive(true);
        healthUIComponent.UpdateHealth(currentHealth, maxHealth);
    }

    public void ShowDamageOverlay()
    {
        StartCoroutine(DamageOverlayAnimationCoroutine());
    }

    private IEnumerator DamageOverlayAnimationCoroutine()
    {
        damageOverlayImage.gameObject.SetActive(true);
        damageOverlayImage.DOFade(0.8f, 0.1f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.1f);
        damageOverlayImage.DOFade(0f, 0.5f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(0.5f);
        damageOverlayImage.gameObject.SetActive(false);
    }

    public void ShowTitle()
    {
        titleText.gameObject.SetActive(true);
    }

    public void ShowSelectText(string weaponName)
    {
        selectText.gameObject.SetActive(true);
        selectWeaponNameText.gameObject.SetActive(true);
        selectWeaponNameText.text = weaponName;
    }

    public void ShowNoUpgradeText()
    {
        noUpgradeText.gameObject.SetActive(false);
    }

    public void ShowButtons(string upgradeAText, string upgradeBText)
    {
        upgradeButtonA.gameObject.SetActive(true);
        upgradeButtonAText.text = upgradeAText;
        upgradeButtonB.gameObject.SetActive(true);
        upgradeButtonBText.text = upgradeBText;
        passButton.gameObject.SetActive(true);
    }

    public void OnUpgradeA()
    {
        selection = SelectionResult.UpgradeA;
    }

    public void OnUpgradeB()
    {
        selection = SelectionResult.UpgradeB;
    }

    public void OnPass()
    {
        selection = SelectionResult.Pass;
    }
}