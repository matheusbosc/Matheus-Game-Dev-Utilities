using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace com.matheusbosc.utilities
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager instance;
        public Slider bar;
        public bool hasLoaded;
        public SaveManager saveManager;
        public LoadingScreenUIManager loadingScreenManager;
        private bool isLoading;
        public bool enableLogging = true;
        [Tooltip("The first scene loaded when the game starts")] public SceneIndexes firstGameScene;
        
        private bool enableSaving = false;

        private void Awake()
        {
            instance = this;

            LogManager.enableLogger = enableLogging;
            
            if (saveManager != null) enableSaving = true;

            SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);
        }

        public void StartGame(int saveSlot)
        {
            if (enableSaving)
            {
                var saveReturn = saveManager.LoadGame(saveSlot);
                if (saveReturn == 1)
                {
                    saveManager.CreateNewGame(saveSlot);
                }

                if (saveManager.saveData.gameVersion != Application.version)
                {
                    LogManager.Error("Game version does not match this save file!");
                    return;
                }
            }

            switch (enableSaving ? saveManager.saveData.levelIndex : firstGameScene)
            {
                // 3. Add a case for each of the levels
                case SceneIndexes.LEVEL_1:
                    LoadLevel1(true);
                    break;
                default:
                    LogManager.Error("Invalid Level");
                    break;
            }
        }

        private float totalSceneProgress;
        private List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

        #region Load Levels
        
        public void LoadMainMenu()
        {
            if (isLoading == true) return;
            isLoading = true;
            
            // Load Info
            string levelNumberStr = "mm";
            SceneIndexes baseSceneIndex = SceneIndexes.TITLE_SCREEN;
            // ReSharper disable once CollectionNeverUpdated.Local
            List<SceneIndexes> scenesToLoad = new List<SceneIndexes>();
            // Append to list
            scenesToLoad.Add(baseSceneIndex);
            
            string loadingScreenStylePath = "LoadingScreen/LSS_MM";
            
            LoadBase(false, loadingScreenStylePath, scenesToLoad, baseSceneIndex, levelNumberStr);
        }
        
        // 2. Copy this for each level
        public void LoadLevel1(bool fromMainMenu = false)
        {
            if (isLoading == true) return;
            isLoading = true;
            
            // Load Info
            string levelNumberStr = "0";                            // 2.1. Set the level number
            SceneIndexes baseSceneIndex = SceneIndexes.LEVEL_1;     // 2.2. Set the level index to load for the main scene
            
            List<SceneIndexes> scenesToLoad = new List<SceneIndexes>();
            
            // Append to list
            scenesToLoad.Add(baseSceneIndex);                       // 2.3. Add all the scenes you need to load here
            
            string loadingScreenStylePath = "LoadingScreen/LSS_01"; // 2.4. The path for the loading screen style (this example would be: Assets/Resources/LoadingScreen/LSS_01)
            
            LoadBase(fromMainMenu, loadingScreenStylePath, scenesToLoad, baseSceneIndex, levelNumberStr);
        }

        #endregion
        
        #region DO NOT CHANGE

        private float totalSpawnProgress;

        public IEnumerator GetSceneLoadProgress(string level)
        {
            float totalProgress = 0;
            
            LogManager.Info($"Get scene load progress coroutine started.");
            
            for (int i = 0; i < scenesLoading.Count; i++)
            {
                while (!scenesLoading[i].isDone)
                {
                    totalSceneProgress = 0;

                    foreach (AsyncOperation operation in scenesLoading)
                    {
                        totalSceneProgress += operation.progress;
                    }
                    
                    totalSceneProgress = (totalSceneProgress / scenesLoading.Count) * 100f;
                    LogManager.Info($"Scene Loading Progress: {totalSceneProgress}");
                    
                    totalProgress = (totalSceneProgress + totalSpawnProgress) / 2;
                    bar.value = totalProgress;
                    
                    yield return null;
                }
            }
            
            LogManager.Info($"Scene Loading Done!");

            yield return new WaitForSeconds(0.5f);
            
            StartCoroutine(GetTotalProgress(level, totalProgress));
        }

        private IEnumerator GetTotalProgress(string level, float tP)
        {
            LogManager.Info($"Get total progress coroutine started.");
            float totalProgress = tP;
            
            ILevelInitializer initializer = GameObject.Find("Init").GetComponent<ILevelInitializer>();

            while (initializer == null || !initializer.isDone)
            {
                if (initializer == null)
                {
                    totalSpawnProgress = 0;
                    LogManager.Warn($"Initializer not found!");
                    initializer = GameObject.Find("Init").GetComponent<ILevelInitializer>();
                }
                else
                {
                    totalSpawnProgress = Mathf.Round(initializer.progress * 100f);
                    LogManager.Info($"Initializer Progress: {totalSpawnProgress}.");
                }

                totalProgress = (totalSceneProgress + totalSpawnProgress) / 2;
                bar.value = totalProgress;
                
                yield return null;
            }
            
            LogManager.Info($"Initializer done!");
            
            if (level == "mm")
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            
            if (enableSaving) saveManager.SaveGame();
            loadingScreenManager.Hide();
            hasLoaded = true;
            
            yield return null;
            
            LogManager.Info("Level Initialization Complete.");
            isLoading = false;
        }
        
        /// <summary>
        /// Loads a level:
        /// string levelNumberStr = Then number or level code provided to the coroutine.
        /// SceneIndexes baseSceneIndex = The base scene saved to the game save file.
        /// SceneIndexes List scenesToLoad = Scenes to load.
        /// SceneIndexes List scenesToUnload = Scenes to unload.
        /// </summary>
        /// <param name="fromMainMenu"></param>
        /// <param name="loadingScreenStylePath"></param>
        /// <param name="scenesToLoad"></param>
        /// <param name="baseSceneIndex"></param>
        /// <param name="levelNumberStr"></param>
        private void LoadBase(bool fromMainMenu, string loadingScreenStylePath, List<SceneIndexes> scenesToLoad, SceneIndexes baseSceneIndex, string levelNumberStr, int spawnpoint = 0)
        {
            loadingScreenManager.Show(Resources.Load<LoadingScreenStyle>(loadingScreenStylePath));
    
            // Ensure there are no duplicates in the scenesToLoad list
            var uniqueScenesToLoad = new HashSet<SceneIndexes>(scenesToLoad);
            scenesToLoad = new List<SceneIndexes>(uniqueScenesToLoad);

            List<SceneIndexes> scenesToUnload = new List<SceneIndexes>();
    
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                int buildIndex = scene.buildIndex;
        
                if (scene.isLoaded && !(buildIndex == (int)SceneIndexes.TITLE_SCREEN || buildIndex == (int)SceneIndexes.MANAGER))
                {
                    scenesToUnload.Add((SceneIndexes)buildIndex);
                }
            }

            scenesLoading.Clear();
            if (fromMainMenu)
            {
                scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.TITLE_SCREEN));
            }
            else
            {
                foreach (var s in scenesToUnload)
                {
                    scenesLoading.Add(SceneManager.UnloadSceneAsync((int)s));
                }
            }

            foreach (var s in scenesToLoad)
            {
                scenesLoading.Add(SceneManager.LoadSceneAsync((int)s, LoadSceneMode.Additive));
            }

            if (levelNumberStr != "mm" && enableSaving) saveManager.saveData.levelIndex = baseSceneIndex;

            LogManager.Info("Level Initializing.");
            
            StartCoroutine(GetSceneLoadProgress(levelNumberStr));
        }
        
        #endregion
    }
    
    // 1. Add the scene indexes here for each scene
    public enum SceneIndexes
    {
        MANAGER = 0,       // DO NOT DELETE
        TITLE_SCREEN = 1,  // DO NOT DELETE
        LEVEL_1 = 2
    }
}