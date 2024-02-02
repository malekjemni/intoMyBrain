using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedLogic.MathUtils;

public class TowerBuyMenu : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject towerBuyPrefab;
    [SerializeField] private GameObject tooltipObj;
    [SerializeField] private TextMeshProUGUI headerTMP;
    [SerializeField] private TextMeshProUGUI descriptionTMP;
    [SerializeField] private TextMeshProUGUI loreDescriptionTMP;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button quitBtn;

    [SerializeField] private List<TowerSO> towerListDebug;

    private TowerDefenseManager tdManager;
    private InputManager inputManager;
    private bool isPaused = false;

    private void Start()
    {
        inputManager = InputManager.instance;
        inputManager.PauseMenu += PauseMenu;
        resumeBtn.onClick.AddListener(PauseMenu);
        homeBtn.onClick.AddListener(() => SceneController.instance.LoadScene(SceneIndexes.TITLE, MapIndexes.RUINS, true));
        quitBtn.onClick.AddListener(() => SceneController.instance.QuitGame());
        tdManager = TowerDefenseManager.instance;

        for (int i = 0; i < tdManager.AllTowerList.TowerList.Count; i++)
        {
            TowerSO towerSO = tdManager.AllTowerList.TowerList[i];
            GameObject buttonObject = Instantiate(towerBuyPrefab, parent);
            TowerBuyButton towerBuyButton = buttonObject.GetComponent<TowerBuyButton>();
            TowerDescription towerDescription = buttonObject.GetComponent<TowerDescription>();
            towerDescription.tooltipObj = tooltipObj;
            towerDescription.headerTMP = headerTMP;
            towerDescription.descriptionTMP = descriptionTMP;
            towerDescription.loreDescriptionTMP = loreDescriptionTMP;

            if (!towerBuyButton) return;

            tdManager.CashSystem.OnModified += (sender, eventArgs) =>
            {
                towerBuyButton.TowerBtn.interactable = eventArgs.currentValue >= towerSO.TowerCost;
            };

            int index = i;
            towerBuyButton.Initialize(towerSO.TowerCost, towerSO.IconSpr, () => 
            {
                if (tdManager.CashSystem.HasEnough(towerSO.TowerCost))
                {
                    BuyTower(index);
                }
            });
            towerDescription.Initialize(towerSO);
        }
    }
    private void PauseMenu()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    void PauseGame()
    {
        Time.timeScale = 0f; // Set time scale to 0 to pause the game
        pauseMenu.SetActive(true); // Activate your pause menu UI

    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // Set time scale back to 1 to resume the game
        pauseMenu.SetActive(false); // Deactivate your pause menu UI

    }
    private void BuyTower(int index)
    {
        TowerDefenseManager.instance.BuildTower(TowerDefenseManager.instance.AllTowerList.TowerList[index]);
    }
}
