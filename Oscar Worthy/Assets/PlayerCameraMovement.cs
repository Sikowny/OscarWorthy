using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraMovement : MonoBehaviour
{
    Camera playerCam;
    float rotation = 0f;
    float rotationTime = 0.2f;

    float cameraDistance = -8;
    float cameraHeight = 3;
    float cameraVerticalRotation = 10;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = GameObject.Find("Main Camera").GetComponent<Camera>();   
    }

    // Update is called once per frame
    void Update()
    {
        CheckForCameraRotate();

        PositionCamera();
    }

    Tween rotationTween = new Tween();
    void CheckForCameraRotate()
    {
        if (Input.GetKeyDown("left"))
        {
            rotationTween.StartTween((val) => rotation = val, rotation, rotation - 90, rotationTime, Tween.Interpolator.Cosine);
        }
        else if (Input.GetKeyDown("right"))
        {
            rotationTween.StartTween((val) => rotation = val, rotation, rotation + 90, rotationTime, Tween.Interpolator.Cosine);
        }
    }

    void PositionCamera()
    {
        Vector3 cameraPosition = new Vector3();

        Vector3 cameraOffset = Quaternion.Euler(0, rotation, 0) * new Vector3(0, cameraHeight, cameraDistance);

        cameraPosition = transform.position + cameraOffset;

        playerCam.transform.rotation = Quaternion.Euler(cameraVerticalRotation, rotation, 0);

        playerCam.transform.position = cameraPosition;
    }
}
