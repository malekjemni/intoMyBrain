using System;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedLogic.Currency;
using UnscriptedLogic.MathUtils;

public class TowerDefenseManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int startHealth;
    [SerializeField] private float startCash;
    [SerializeField] private ExperienceLevelsSO experienceLevels;

    [Header("Global Game Variables")]
    [SerializeField] private TowerListSO allUsableTowers;

    [Header("Components")]
    [SerializeField] private BuildManager buildManager;
    [SerializeField] private CameraControls camControls;

    [Header("UI")]
    [SerializeField] private Transform hudUI;
    [SerializeField] private Transform loseState;
    [SerializeField] private Transform winState;
    [SerializeField] private TextMeshProUGUI healthTMP;
    [SerializeField] private TextMeshProUGUI currencyTMP;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button retryBtn;

    private CurrencyHandler healthPoints;
    private CurrencyHandler cashSystem;

    public CurrencyHandler HealthSystem => healthPoints;
    public CurrencyHandler CashSystem => cashSystem;
    public TowerListSO AllTowerList => allUsableTowers;
    public ExperienceLevelsSO ExperienceLevelsSO => experienceLevels;
    public float CurrentCash => cashSystem.Current;

    public event EventHandler OnInitialized;

    public static TowerDefenseManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InvokeRepeating("PassiveHealthLoss", 1, 15f); 
        healthPoints = new CurrencyHandler(startHealth);
        cashSystem = new CurrencyHandler(startCash);

        healthTMP.text = healthPoints.Current.ToString();
        currencyTMP.text = $"{cashSystem.Current}";

        healthPoints.OnModified += HealthSystem_OnModified; ;
        cashSystem.OnModified += CashSystem_OnModified;


        homeBtn.onClick.AddListener(() =>
        {
            SceneChanger.instance.ChangeScene(GameScenes.Home);
        });

        retryBtn.onClick.AddListener(() =>
        {
            SceneChanger.instance.ChangeScene(GameScenes.Game);
        });

        UnitBase.OnAnyUnitTookDamage += UnitBase_OnAnyUnitTookDamage;
        UnitBase.OnAnyUnitCompletedPath += OnUnitCompletedPath;

        buildManager.OnBuild += BuildManager_OnBuild;
        OnInitialized?.Invoke(this, EventArgs.Empty);
    }
    public void PassiveHealthLoss()
    {
        HealthSystem.Modify(ModifyType.Subtract, 1f);
    }

    public void Win()
    {
        hudUI.gameObject.SetActive(false);
        winState.gameObject.SetActive(true);

        EntityHandler.instance.KillAllUnits_Coroutine();

        camControls.InputManager_OnResetCamera();
        camControls.DisableAllInput();

        if (SceneController.instance == null)
        {
            Debug.Log("You're probably in a test scene. Couldn't find SceneController component. That's fine. Just remember to test in the actual game scene too.");
            return;
        }

        SceneController.instance.LoadScene(SceneIndexes.TITLE, MapIndexes.RUINS);
    }

    private void UnitBase_OnAnyUnitTookDamage(object sender, UnitTookDamageEventArgs e)
    {
        cashSystem.Modify(ModifyType.Add, Mathf.RoundToInt(e.damage));
    }

    private void CashSystem_OnModified(object sender, CurrencyEventArgs e)
    {
        currencyTMP.text = $"{e.currentValue}";
    }

    private void HealthSystem_OnModified(object sender, CurrencyEventArgs e)
    {
        // Assuming HealthSystem.Current and HealthSystem.Starting are floats
        float normalizedHealth = HealthSystem.Current / (HealthSystem.Starting * 2);

        // Clamp the normalized health between 0 and 1
        normalizedHealth = Mathf.Clamp01(normalizedHealth);

        // Calculate the health area index (from 0 to 6)
        int healthAreaIndex = Mathf.Clamp(Mathf.FloorToInt(normalizedHealth * 7), 0, 6);
        StateHandler.instance.gameObject.GetComponent<Image>().sprite = StateHandler.instance.spriteStates[healthAreaIndex];




        SliderControlHandler.instance.slider.value = healthPoints.Current / (healthPoints.Starting * 2);
        healthTMP.text = e.currentValue.ToString();

        if (healthPoints.IsEmpty)
        {
            //Game Over
            hudUI.gameObject.SetActive(false);
            loseState.gameObject.SetActive(true);

            EntityHandler.instance.KillAllUnits_Coroutine();
            EntityHandler.instance.DisableAllTowers_Coroutine();

            camControls.stayDisabled = true;
            camControls.InputManager_OnResetCamera();

            SceneController.instance.LoadScene(SceneIndexes.TITLE, MapIndexes.RUINS);
        }
    }

    private void BuildManager_OnBuild(object sender, OnBuildEventArgs e)
    {
        cashSystem.Modify(ModifyType.Subtract, allUsableTowers.TowerList[e.buildIndex].TowerCost);
    }

    public void BuildTower(TowerSO tower)
    {
        int towerIndex = allUsableTowers.TowerList.IndexOf(tower);
        buildManager.AttemptBuild(towerIndex);
    }

    private void OnUnitCompletedPath(object sender, EventArgs eventArgs)
    {
        UnitBase unitScript = sender as UnitBase;
        

        switch (unitScript.MyUnitType)
        {
            case UnitBase.UnitType.Friendly:
                healthPoints.Modify(ModifyType.Add, unitScript.CurrentHealth);
                break;
            case UnitBase.UnitType.Hostile:
                healthPoints.Modify(ModifyType.Subtract, unitScript.CurrentHealth);
                break;
            default:
                break;
        }
        Destroy(unitScript.gameObject);
    }

    public void TogglePause(bool pause)
    {
        float scale = pause ? 0 : 1;
        Time.timeScale = scale;
    }

    /// <summary>
    /// Clears inspect window, kills all units and towers.
    /// </summary>
    /// <returns>Itself as an IEnumerator for the super coroutine to yield return on</returns>
    public IEnumerator SceneExitCleanUp_Coroutine(bool killTowers = true, bool killUnits = true)
    {
        InspectWindow.instance.ClearWindow();
        yield return new WaitForSecondsRealtime(0.5f);

        if (killUnits)
            yield return StartCoroutine(EntityHandler.instance.KillAllUnits_Coroutine());
        
        if (killTowers)
            yield return StartCoroutine(EntityHandler.instance.KillAllTowers_Coroutine());

        yield return new WaitForSecondsRealtime(0.5f);
    }
}
