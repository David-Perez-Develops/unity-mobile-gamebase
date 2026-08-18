using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public partial class PlayerData
{
    public const int CurrentSaveVersion = 1;

    [SerializeField] private int saveVersion = CurrentSaveVersion;
    [SerializeField] private bool isFirstPlaying = true;
    [SerializeField] private int currentEnergy;
    [SerializeField] private int currentGold;
    [SerializeField] private int currentDiamond;
    [SerializeField] private GameReward savingReward;
    [SerializeField] private string refillEnergyPoint = DateTime.UtcNow.ToString(Utility.DateTimeFormat, CultureInfo.InvariantCulture);
    [SerializeField] private Dictionary<string, string> gameData = new Dictionary<string, string>();

    public int SaveVersion => saveVersion;

    public bool IsFirstPlaying
    {
        get => isFirstPlaying;
        set => isFirstPlaying = value;
    }

    public int CurrentEnergy
    {
        get => currentEnergy;
        set
        {
            Observer.EnergyChanged?.Invoke(value - currentEnergy);
            currentEnergy = Mathf.Max(0, value);
            Observer.EnergyChangedDone?.Invoke();
        }
    }

    public int CurrentGold
    {
        get => currentGold;
        set
        {
            Observer.GoldChanged?.Invoke(value - currentGold);
            currentGold = Mathf.Max(0, value);
            Observer.GoldChangedDone?.Invoke();
        }
    }

    public int CurrentDiamond
    {
        get => currentDiamond;
        set
        {
            Observer.DiamondChanged?.Invoke(value - currentDiamond);
            currentDiamond = Mathf.Max(0, value);
            Observer.DiamondChangedDone?.Invoke();
        }
    }

    public GameReward SavingReward
    {
        get => savingReward;
        set => savingReward = value;
    }

    public string RefillEnergyPoint
    {
        get => refillEnergyPoint;
        set => refillEnergyPoint = value;
    }

    /// <summary>
    /// Escape hatch for small game-specific values without introducing a StarterKit -> Game dependency.
    /// Larger games should keep a typed model in Assets/Game and serialize it into this store.
    /// </summary>
    public IDictionary<string, string> GameData => gameData;

    public void MigrateIfNeeded()
    {
        if (gameData == null) gameData = new Dictionary<string, string>();

        // Add sequential migrations here as the reusable save schema evolves.
        if (saveVersion < 1) saveVersion = 1;
        if (saveVersion > CurrentSaveVersion)
            Debug.LogWarning($"Save version {saveVersion} is newer than supported version {CurrentSaveVersion}.");
    }

    public void PrepareForSave()
    {
        MigrateIfNeeded();
        saveVersion = CurrentSaveVersion;
    }
}

[Serializable]
public class GameReward
{
    public int energyValue;
    public int goldValue;
    public int diamondValue;
}
