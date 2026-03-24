using System;
using System.Collections.Generic;

namespace com.matheusbosc.utilities
{
    [Serializable]
    public class SaveData
    {
        public string gameVersion;
        public int saveSlot;
        public SceneIndexes levelIndex;
        public PlayerStats playerStats;
    }
}