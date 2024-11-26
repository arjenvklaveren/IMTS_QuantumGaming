using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Test
{
    public class SceneLoadTest : MonoBehaviour
    {
        void Update()
        {
            if(UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Additive);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            {
                SceneManager.LoadScene("WorkSpaceScene", LoadSceneMode.Additive);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
            {
                SceneManager.LoadScene("SimulationDataScene", LoadSceneMode.Additive);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
            {
                SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5))
            {
                SceneManager.LoadScene("MasterScene", LoadSceneMode.Single);
            }
        }
    }
}
