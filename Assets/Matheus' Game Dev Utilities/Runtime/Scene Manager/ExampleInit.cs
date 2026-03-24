using System.Collections;
using UnityEngine;

namespace com.matheusbosc.utilities
{
    public class ExampleInit : MonoBehaviour, ILevelInitializer
    {
        public static ExampleInit current { get; private set; }
        public bool isDone { get; private set; }
        public float progress { get; private set; }
        
        public Transform player;
        public Transform spawnPoint;
        
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
            
            // Load everything that needs to load before the game starts
            
            
            player.position = spawnPoint.position;
            player.eulerAngles = spawnPoint.eulerAngles;
			
			// Repeat the following 2 lines after each step
			loadingStep++;
			progress = (float)loadingStep / (float)maxSteps;
            
            
            // Leave lines below this comment the way it is
            
            

            yield return new WaitForSeconds(0.5f);
            
            isDone = true;
        }
    }
}