using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("ToolTip")]
     public GameObject tooltipObj;
     public TextMeshProUGUI headerTMP;
     public TextMeshProUGUI descriptionTMP;
     public TextMeshProUGUI loreDescriptionTMP;


    public static event EventHandler<bool> OnAnyUpgradeButtonClicked;

    private TowerSO towerDetails;
    public void Initialize(TowerSO tower)
    {
        towerDetails = tower;   
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        headerTMP.text = towerDetails.name;
        descriptionTMP.text = towerDetails.TowerDescription;
        loreDescriptionTMP.text = towerDetails.TowerLoreDescription;
        Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hide();
    }

    private void Show()
    {
        tooltipObj.SetActive(true);
    }

    private void Hide()
    {
        tooltipObj.SetActive(false);
    }

    public void InvokeEvent(bool value)
    {
        OnAnyUpgradeButtonClicked?.Invoke(this, value);
    }
}
