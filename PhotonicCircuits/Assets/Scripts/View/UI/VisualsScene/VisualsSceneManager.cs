using SadUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class VisualsSceneManager : Singleton<VisualsSceneManager>
    {
        private List<VisualsScene> activeScenes = new List<VisualsScene>();

        #region Awake and set events
        protected override void Awake()
        {
            SetInstance(this);
        }
        #endregion

        public void LoadScene(VisualsScene scene)
        {
            activeScenes.Add(scene);
            scene.LoadScene();
        }
        public void UnloadScene(VisualsScene scene)
        {
            activeScenes.Remove(scene);
            scene.UnloadScene();
        }
    }
}
