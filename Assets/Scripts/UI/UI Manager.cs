using System;
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

    public TextMeshProUGUI SelectWeaponNameText;

    public Button          UpgradeButtonA;
    public TextMeshProUGUI UpgradeButtonAText;
    public Button          UpgradeButtonB;
    public TextMeshProUGUI UpgradeButtonBText;
    public Button          PassButton;

    [FormerlySerializedAs("Selection")] public SelectionResult selection;

    private Canvas _canvas;

    public void Start()
    {
        _canvas = FindFirstObjectByType<Canvas>();
        HideAll();
    }

    public void HideAll()
    {
        titleText.SetActive(false);
        selectText.SetActive(false);
        SelectWeaponNameText.gameObject.SetActive(false);
        noUpgradeText.SetActive(false);
        UpgradeButtonA.gameObject.SetActive(false);
        UpgradeButtonB.gameObject.SetActive(false);
        PassButton.gameObject.SetActive(false);

        selection = SelectionResult.None;
    }

    public void ShowTitle()
    {
        titleText.gameObject.SetActive(true);
    }

    public void ShowSelectText(string weaponName)
    {
        selectText.gameObject.SetActive(true);
        SelectWeaponNameText.gameObject.SetActive(true);
        SelectWeaponNameText.text = weaponName;
    }

    public void ShowNoUpgradeText()
    {
        noUpgradeText.gameObject.SetActive(false);
    }

    public void ShowButtons(string upgradeAText, string upgradeBText)
    {
        UpgradeButtonA.gameObject.SetActive(true);
        UpgradeButtonAText.text = upgradeAText;
        UpgradeButtonB.gameObject.SetActive(true);
        UpgradeButtonBText.text = upgradeBText;
        PassButton.gameObject.SetActive(true);
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