using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace com.matheusbosc.utilities
{
    public class MainMenu_Manager : MonoBehaviour
    {

        public TextMeshProUGUI versionText;

        public GameObject[] menus;
        public float menuFadeDuration = 0.5f;

        public Color accentColour;
        private Color _lighterAccentColour;
        public Image[] imagesWithAccent;
        public TextMeshProUGUI[] textsWithAccent;
        public Image[] imagesWithLighterAccent;
        public TextMeshProUGUI[] textsWithLighterAccent;

        [Header("Title")]
        public TextMeshProUGUI titleText;
        public Image logo;
        public string gameTitle;
        public Sprite gameLogo;
        public enum TitleType
        {
            Logo,
            Name
        }
        public TitleType titleType;
        
        // Change Between Backgrounds
        [Header("Multiple Backgrounds")]
        public bool multipleBackgrounds = true;
        public RawImage imageA;
        public RawImage imageB;
        public Texture2D[] backgrounds;
        public float fadeDuration = 2f;
        public float displayDuration = 5f;
        
        [Header("Settings")]
        public List<Slider> sliders;
        public AudioMixer[] mixers;
        public string[] mixerExposedParameterName;

        public TMP_Dropdown qualityDropdown;

        private int currentIndex = 0;
        private bool isATop = true;
        private CanvasGroup currentMenu;

        void Start()
        {
            _lighterAccentColour = new Color(accentColour.r + 30,accentColour.g + 30,accentColour.b + 30);
            
            foreach (var i in imagesWithAccent)
            {
                i.color = accentColour;
            }
            
            foreach (var t in textsWithAccent)
            {
                t.color = accentColour;
            }
            
            foreach (var i in imagesWithLighterAccent)
            {
                i.color = _lighterAccentColour;
            }
            
            foreach (var t in textsWithLighterAccent)
            {
                t.color = _lighterAccentColour;
            }
            
            currentMenu = menus[0].GetComponent<CanvasGroup>();
            versionText.text = "v" + Application.version;
            
            qualityDropdown.options.Clear();

            foreach (var item in QualitySettings.names)
            {
                qualityDropdown.options.Add(new TMP_Dropdown.OptionData(item));
            }
            
            qualityDropdown.value = QualitySettings.GetQualityLevel();

            if (titleType == TitleType.Logo)
            {
                logo.sprite = gameLogo;
                logo.gameObject.SetActive(true);
                titleText.gameObject.SetActive(false);
            }
            else
            {
                titleText.text = gameTitle;
                titleText.gameObject.SetActive(true);
                logo.gameObject.SetActive(false);
            }

            if (!multipleBackgrounds)
            {
                if (backgrounds.Length < 1) return;
                
                imageA.texture = backgrounds[0];
                imageA.color = Color.white;
                
            }
            else
            {
                if (backgrounds.Length < 2) return;
                
                imageA.texture = backgrounds[0];
                imageB.texture = backgrounds[1];
                imageA.color = Color.white;
                imageB.color = new Color(1, 1, 1, 0);

                StartCoroutine(FadeLoop());
            }
        }

        IEnumerator FadeLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(displayDuration);

                int nextIndex = (currentIndex + 1) % backgrounds.Length;
                RawImage top = isATop ? imageB : imageA;
                RawImage bottom = isATop ? imageA : imageB;

                top.texture = backgrounds[nextIndex];

                // Fade in top, fade out bottom
                float t = 0f;
                while (t < fadeDuration)
                {
                    t += Time.deltaTime;
                    float alpha = t / fadeDuration;
                    top.color = new Color(1, 1, 1, alpha);
                    bottom.color = new Color(1, 1, 1, 1 - alpha);
                    yield return null;
                }

                // Finish fade
                top.color = Color.white;
                bottom.color = new Color(1, 1, 1, 0);
                isATop = !isATop;
                currentIndex = nextIndex;
            }
        }

        public void ChangeMenu(GameObject menu)
        {
            if (!menus.Contains(menu)) return;

            StartCoroutine(FadeMenus(menu.GetComponent<CanvasGroup>()));
        }
        
        IEnumerator FadeMenus(CanvasGroup menuToEnable)
        {
            
                float t = 0f;
                while (t < menuFadeDuration)
                {
                    t += Time.deltaTime;
                    float alpha = t / menuFadeDuration;
                    menuToEnable.alpha = alpha;
                    currentMenu.alpha = 1 - alpha;
                    yield return null;
                }

                // Finish fade
                menuToEnable.alpha = 1;
                currentMenu.alpha = 0;
                currentMenu.interactable = false;
                currentMenu.blocksRaycasts = false;
                
                menuToEnable.interactable = true;
                menuToEnable.blocksRaycasts = true;
                menuToEnable.transform.SetAsLastSibling();
                currentMenu = menuToEnable;
            
        }

        public void StartGame(int slot)
        {
            GameSceneManager.instance.StartGame(slot);
        }

        public void DeleteGame(int slot)
        {
            GameSceneManager.instance.saveManager.DeleteGame(slot);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void SetVolume(Slider slider)
        {
            int index = -1;
            foreach (var s in sliders)
            {
                index++;
                if (s == slider)
                {
                    break;
                }
                
                if (index + 1 == sliders.Count) index = -1;
            }

            if (index == -1) return;
            
            mixers[index].SetFloat(mixerExposedParameterName[index], slider.value);
            
            PlayerPrefs.SetFloat(mixerExposedParameterName[index], slider.value);
        }

        public void SetQualityLevel()
        {
            QualitySettings.SetQualityLevel(qualityDropdown.value);
        }
    }
}