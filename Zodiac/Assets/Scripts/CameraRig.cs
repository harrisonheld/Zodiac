using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CameraRig : MonoBehaviour
{
    // (in tiles)
    [SerializeField] private float height = 21.0f;
    [SerializeField] private float width = 28.0f;


    private void Update()
    {
        Camera.main.orthographicSize = height / 2.0f;
        Camera.main.aspect = width / height;
        Camera.main.rect = new Rect(0f, 0f, 1/Camera.main.aspect, 1f);

        // since sprites are one tile width, the sprite at (0.0, 0.0) will have it's art end at (-0.5, -0.5)
        float posX = width  / 2.0f - 0.5f;
        float posY = height / 2.0f - 0.5f;
        Camera.main.transform.position = new Vector3(posX, posY, -10f);
    }
}
