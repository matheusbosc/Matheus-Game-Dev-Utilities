using System.IO;
using UnityEngine;

namespace com.matheusbosc.utilities
{
    public class SaveManager : MonoBehaviour
    {
        public bool encryptSave = true;
        public SaveData saveData;
        
        public void SaveGame()
        {
            string path = Path.Combine(Application.persistentDataPath, "saves", "slot" + saveData.saveSlot, "save" + saveData.saveSlot + ".savefile");

            LogManager.Info("Saving to: " + path);

            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonUtility.ToJson(saveData, prettyPrint: true);
            
            if (encryptSave)
            {
                string encrypted = SaveEncryption.Encrypt(json); // Encrypt JSON
                File.WriteAllText(path, encrypted); // Save to file
            }
            else
            {
                File.WriteAllText(path, json); // Save JSON
            }
        }

        public int LoadGame(int saveSlot)
        {
            string path = Path.Combine(Application.persistentDataPath, "saves", "slot" + saveSlot, "save" + saveSlot + ".savefile");
            
            if (!File.Exists(path))
            {
                LogManager.Warn("Save file not found: " + path);
                return 1;
            }
            
            string encrypted = File.ReadAllText(path);
            if (encryptSave)
            {
                string json = SaveEncryption.Decrypt(encrypted);
                saveData = JsonUtility.FromJson<SaveData>(json); // Load Decrypted JSON
            }
            else
            {
                saveData = JsonUtility.FromJson<SaveData>(encrypted); // Load Base File
            }
            return 0;
        }

        public void CreateNewGame(int saveSlot)
        {
            saveData = new SaveData();
            saveData.saveSlot = saveSlot;
            saveData.levelIndex = SceneIndexes.LEVEL_1;
            saveData.gameVersion = Application.version;   // Set Application.version in 'File/Project Settings/Player/Version'
            SaveGame();
        }

        public void UnloadGame()
        {
            saveData = null;
        }
        
        public int DeleteGame(int saveSlot)
        {
            string path = Path.Combine(Application.persistentDataPath, "saves", "slot" + saveSlot, "save" + saveSlot + ".savefile");
            
            if (!File.Exists(path))
            {
                LogManager.Warn("Save file not found: " + path);
                return 1;
            }
            
            File.Delete(path);
            return 0;
        }
    }
}