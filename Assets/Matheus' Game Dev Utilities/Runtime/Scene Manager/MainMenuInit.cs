using System.Collections;
using UnityEngine;

namespace com.matheusbosc.utilities
{
    public class MainMenuInit : MonoBehaviour, ILevelInitializer
    {
        public static MainMenuInit current { get; private set; }
        public bool isDone { get; private set; }
        public float progress { get; private set; }
        
        private GameSceneManager gameManager = GameSceneManager.instance;

        private int loadingStep = 0, maxSteps = 1;
        

        private void Start()
        {
            current = this;
            isDone = false;
            StartCoroutine(Begin());
        }

        IEnumerator Begin()
        {
		
			maxSteps = 1;
            
			loadingStep++;
			progress = (float)loadingStep / (float)maxSteps;
            
            yield return new WaitForSeconds(0.5f);
            
            isDone = true;
        }
    }
}