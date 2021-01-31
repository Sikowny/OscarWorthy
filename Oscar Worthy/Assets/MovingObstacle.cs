using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public Vector3 pathStart;
    public Vector3 pathEnd;

    float velocity;
    bool forward;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = pathStart;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
