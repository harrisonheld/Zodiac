using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    class CameraRig : MonoBehaviour
    {
        // singleton
        public static CameraRig Instance { get; private set; }
        public void Awake()
        {
            if (Instance == null) // If there is no instance already
            {
                DontDestroyOnLoad(this.gameObject); // Keep the GameObject, this component is attached to, across different scenes
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject); // Destroy the GameObject, this component is attached to
            }
        }
        private void Update()
        {
            // size the camera to fit the game area
            Camera.main.orthographicSize = (float)WorldGen.World.GetCurrentZoneHeight / 2.0f;
            Camera.main.aspect = (float)WorldGen.World.GetCurrentZoneWidth / (float)WorldGen.World.GetCurrentZoneHeight;
            Camera.main.rect = new Rect(0f, 0f, 1f / Camera.main.aspect, 1f);

            // position the camera
            // since sprites are one tile width, the sprite at (0.0, 0.0) will have it's art end at (-0.5, -0.5)
            float posX = (float)WorldGen.World.GetCurrentZoneWidth / 2.0f - 0.5f;
            float posY = (float)WorldGen.World.GetCurrentZoneHeight / 2.0f - 0.5f;
            Camera.main.transform.position = new Vector3(posX, posY, -10f);

            // update status menu width to fill the empty part of the screen
            int statusWidth = Screen.width - Camera.main.pixelWidth;
            int statusHeight = Screen.height;
            MenuManager.Instance.SetStatusMenuSize(new(statusWidth, statusHeight));
        }
    }
}
