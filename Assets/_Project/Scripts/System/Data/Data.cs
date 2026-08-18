using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public static class Data
{
    public static PlayerData PlayerData { get; private set; }
    public static readonly string SavePath = Path.Combine(Application.persistentDataPath, "player_data.json");
    public static readonly string BackupPath = SavePath + ".bak";

    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public static void SaveData()
    {
        if (PlayerData == null) PlayerData = new PlayerData();
        PlayerData.PrepareForSave();

        try
        {
            string jsonData = JsonConvert.SerializeObject(PlayerData, Formatting.Indented, JsonSettings);
            string encryptedData = EncryptionHelper.Encrypt(jsonData);

            if (File.Exists(SavePath)) File.Copy(SavePath, BackupPath, true);
            File.WriteAllText(SavePath, encryptedData);
        }
        catch (Exception exception)
        {
            Debug.LogError($"Unable to save player data: {exception.Message}");
        }
    }

    public static void LoadData()
    {
        if (TryLoad(SavePath, out PlayerData)) return;

        if (TryLoad(BackupPath, out PlayerData))
        {
            Debug.LogWarning("Primary save was invalid. Recovered player data from backup.");
            SaveData();
            return;
        }

        PlayerData = new PlayerData();
        PlayerData.MigrateIfNeeded();
    }

    private static bool TryLoad(string path, out PlayerData playerData)
    {
        playerData = null;
        if (!File.Exists(path)) return false;

        try
        {
            string encryptedData = File.ReadAllText(path);
            string decryptedData = EncryptionHelper.Decrypt(encryptedData);
            playerData = JsonConvert.DeserializeObject<PlayerData>(decryptedData);
            if (playerData == null) return false;
            playerData.MigrateIfNeeded();
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Unable to load save '{path}': {exception.Message}");
            return false;
        }
    }

    public static void ClearData()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
        if (File.Exists(BackupPath)) File.Delete(BackupPath);
        PlayerData = new PlayerData();
    }

    public static async Task UpdateData(string jsonContent)
    {
        PlayerData updated = JsonConvert.DeserializeObject<PlayerData>(jsonContent);
        if (updated == null) throw new InvalidDataException("Player data JSON is invalid.");

        updated.MigrateIfNeeded();
        string encryptedData = EncryptionHelper.Encrypt(jsonContent);
        await File.WriteAllTextAsync(SavePath, encryptedData);
        PlayerData = updated;
    }
}
