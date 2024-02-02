using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedLogic.WaveSystems.Asynchronous.Timed;
using DG.Tweening;

public class WaveTextDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveTMP;
    [SerializeField] private GameSpawnManager spawnManager;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingTMP;
    [SerializeField] private TextMeshProUGUI secTMP;

    private void Start()
    {
        spawnManager.OnInitialized += SpawnManager_OnInitialized;    
    }

    private void SpawnManager_OnInitialized(object sender, System.EventArgs e)
    {
        spawnManager.WaveSpawner.OnEnterState += WaveSpawner_OnEnterState;
    }

    private void Update()
    {       
        switch (spawnManager.WaveSpawner.CurrentState)
        {        
            case TimedWaveSpawner.SpawnerState.Starting:              
                loadingTMP.text = "Starting in ";
                secTMP.text = $"{ Math.Round(spawnManager.WaveSpawner.Interval, 1)}sec(s)";
                break;
            case TimedWaveSpawner.SpawnerState.SpawningWave:
                loadingTMP.text = "Spawning...";
                secTMP.text = "";
                TextColorChange(secTMP, Color.white);
                break;
            case TimedWaveSpawner.SpawnerState.Waiting:              
                if (spawnManager.WaveSpawner.WaveIndex < spawnManager.WaveSpawner.WaveAmount - 1)
                {
                    loadingTMP.text = "Next Wave in";
                    secTMP.text = $"{Math.Round(spawnManager.WaveSpawner.Interval, 1)}sec(s)";
                    if (spawnManager.WaveSpawner.Interval <= 5f)
                    {
                        TextColorChange(secTMP, Color.red);
                    } 
                } else
                {                   
                    waveTMP.text = $"Clear all enemies to finish the level";
                }
                break;
            default:
                break;
        }
    }
    public void TextColorChange(TextMeshProUGUI text,Color color)
    {
        text.DOColor(color, 1.0f).SetUpdate(true);
    }

    private void WaveSpawner_OnEnterState(object sender, TimedWaveSpawner.SpawnerState e)
    {
        switch (e)
        {
            case TimedWaveSpawner.SpawnerState.SpawningWave:
                waveTMP.text = $"Wave: {spawnManager.WaveSpawner.WaveIndex + 1}";
                break;
        }
    }
}
