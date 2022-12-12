using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CameraRig : MonoBehaviour
{
    [SerializeField] float height = 10.0f; // (in tiles)


    private void Update()
    {
        Camera.main.orthographicSize = height / 2.0f;

        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float width = height * aspectRatio;

        // since sprites are one tile width, the sprite at (0.0, 0.0) will have it's art end at (-0.5, -0.5)
        float posX = width  / 2.0f - 0.5f;
        float posY = height / 2.0f - 0.5f;
        Camera.main.transform.position = new Vector3(posX, posY, -10f);
    }
}
