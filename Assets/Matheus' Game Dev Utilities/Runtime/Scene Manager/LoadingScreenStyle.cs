using System;
using TMPro;
using UnityEngine;

namespace com.matheusbosc.utilities
{
    [CreateAssetMenu(fileName = "New Loading Screen Style", menuName = "Matheus' Utilities/Loading Screen Style", order = 0)]
    public class LoadingScreenStyle : ScriptableObject
    {
        
        public Color accentColor = new Color(255, 218, 0); 
        public TextAlignmentOptions loadingTextLocation;
        public TextAlignmentOptions levelTextLocation;
        public string levelText;
        public string tipText;
        public LocationVertical loadingBarLocation;
        public Sprite backgroundImage;
        public enum LocationSpinWheel
        {
            TopLeft = 0,
            TopRight = 1,
            BottomRight = 2,
            BottomLeft = 3
        }
    }
    
    [Serializable]
    public enum LocationVertical
    {
        Top = 0,
        Middle = 1,
        Bottom = 2
    }
    
    [Serializable]
    public enum LocationHorizontal
    {
        Left = 0,
        Middle = 1,
        Right = 2
    }
}