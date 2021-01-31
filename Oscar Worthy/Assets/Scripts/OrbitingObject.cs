using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingObject : MonoBehaviour
{
    public float radiusMin;
    public float radiusMax;

    public float speedMin;
    public float speedMax;

    private Vector3 axis;
    private float radius;
    private float speed;

    void SetParams(float radiusMin, float radiusMax, float speedMin, float speedMax, GameObject meshObject)
    {
        this.radiusMin = radiusMin;
        this.radiusMax = radiusMax;
        this.speedMin = speedMin;
        this.speedMax = speedMax;
        gameObject.AddComponent<MeshRenderer>();
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = meshObject.GetComponent<MeshFilter>().sharedMesh;
    }

    // Start is called before the first frame update
    void Awake()
    {
        radius = Random.Range(radiusMin, radiusMax);
        speed = Random.Range(speedMin, speedMax);
        transform.position = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * radius;
        axis = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Vector3.zero, axis, speed);
    }
}
