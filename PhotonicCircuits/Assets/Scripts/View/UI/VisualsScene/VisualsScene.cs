using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Game.UI
{
    public class VisualsScene : MonoBehaviour
    {
        [SerializeField] private Canvas sceneCanvas;

        public event Action<VisualsScene> onLoadScene;
        public event Action<VisualsScene> onUnloadScene;

        public virtual void LoadScene()
        {
            onLoadScene?.Invoke(this);
        }
        public virtual void UnloadScene()
        {
            onUnloadScene?.Invoke(this);
        }
    }
}
