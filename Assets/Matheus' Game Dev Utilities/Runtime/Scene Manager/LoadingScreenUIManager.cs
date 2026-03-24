using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.matheusbosc.utilities
{
    public class LoadingScreenUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI tipText;
        [SerializeField] private GameObject spinWheelImage;
        [SerializeField] private GameObject progressBar;
        [SerializeField] private Image backgroundImageComponent;
        [SerializeField] private Image[] usesAccentColor;

        public void Show(LoadingScreenStyle style)
        {

            backgroundImageComponent.sprite = style.backgroundImage;
            
            // Progress bar position
            
            RectTransform pBT = progressBar.GetComponent<RectTransform>();
            float height = pBT.rect.height;

            switch (style.loadingBarLocation)
            {
                case LocationVertical.Bottom:
                    pBT.anchorMin = new Vector2(0, 0);
                    pBT.anchorMax = new Vector2(1, 0);
                    pBT.pivot = new Vector2(0.5f, 0);
        
                    pBT.offsetMin = new Vector2(0, 0);
                    pBT.offsetMax = new Vector2(0, height);
                    break;

                case LocationVertical.Middle:
                    pBT.anchorMin = new Vector2(0, 0.5f);
                    pBT.anchorMax = new Vector2(1, 0.5f);
                    pBT.pivot = new Vector2(0.5f, 0.5f);
        
                    pBT.offsetMin = new Vector2(0, -height / 2f);
                    pBT.offsetMax = new Vector2(0, height / 2f);
                    break;

                case LocationVertical.Top:
                default:
                    pBT.anchorMin = new Vector2(0, 1);
                    pBT.anchorMax = new Vector2(1, 1);
                    pBT.pivot = new Vector2(0.5f, 1);
        
                    pBT.offsetMin = new Vector2(0, -height);
                    pBT.offsetMax = new Vector2(0, 0);
                    break;
            }

            
            // Text

            loadingText.alignment = style.loadingTextLocation;

            levelText.alignment = style.levelTextLocation;

            levelText.text = style.levelText;
            tipText.text = style.tipText;

            foreach (var i in usesAccentColor)
            {
                i.color = style.accentColor;
            }
            
            loadingScreen.SetActive(true);
        }

        public void Hide()
        {
            loadingScreen.SetActive(false);
        }
    }
}