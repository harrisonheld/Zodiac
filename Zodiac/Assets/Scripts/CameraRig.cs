using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRig : MonoBehaviour
{
    // (in tiles)
    [SerializeField] public const float GAMEAREA_WIDTH = 28.0f;
    [SerializeField] public const float GAMEAREA_HEIGHT = 21.0f;

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
        Camera.main.orthographicSize = GAMEAREA_HEIGHT / 2.0f;
        Camera.main.aspect = GAMEAREA_WIDTH / GAMEAREA_HEIGHT;
        Camera.main.rect = new Rect(0f, 0f, 1f/Camera.main.aspect, 1f);

        // since sprites are one tile width, the sprite at (0.0, 0.0) will have it's art end at (-0.5, -0.5)
        float posX = GAMEAREA_WIDTH / 2.0f - 0.5f;
        float posY = GAMEAREA_HEIGHT / 2.0f - 0.5f;
        Camera.main.transform.position = new Vector3(posX, posY, -10f);


        
        // update status menu width to fill the empty part of the screen
        int statusWidth = Screen.width - Camera.main.pixelWidth;
        int statusHeight = Screen.height;
        StatusMenu.Instance.SetSize(new(statusWidth, statusHeight));
    }
}
