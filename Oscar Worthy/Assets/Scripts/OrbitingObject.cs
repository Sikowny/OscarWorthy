using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingObject : MonoBehaviour
{
    public float radiusMin;
    public float radiusMax;

    public float speedMin;
    public float speedMax;

    public float rotationSpeedMin;
    public float rotationSpeedMax;

    public float scaleMin;
    public float scaleMax;

    private Vector3 axis;
    private float radius;
    private float speed;
    private Vector3 rotationEulers;
    private float rotationSpeed;

    public void SetParams(float radiusMin, float radiusMax, float speedMin, float speedMax, float rotationSpeedMin, float rotationSpeedMax, float scaleMin, float scaleMax, GameObject meshObject)
    {
        this.radiusMin = radiusMin;
        this.radiusMax = radiusMax;
        this.speedMin = speedMin;
        this.speedMax = speedMax;
        this.rotationSpeedMin = rotationSpeedMin;
        this.rotationSpeedMax = rotationSpeedMax;

        if (meshObject.GetComponent<MeshRenderer>() != null)
        {
            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            filter.mesh = meshObject.GetComponent<MeshFilter>().sharedMesh;
            renderer.material = meshObject.GetComponent<MeshRenderer>().sharedMaterial;
        }

        GameObject tempObj = Instantiate(meshObject);

        foreach(Transform child in tempObj.transform)
        {
            child.parent = transform;
        }

        Destroy(tempObj);

        transform.localScale = meshObject.transform.localScale * Random.Range(scaleMin, scaleMax);
    }

    // Start is called before the first frame update
    void Awake()
    {
        radius = Random.Range(radiusMin, radiusMax);
        speed = Random.Range(speedMin, speedMax);
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);
        transform.position = transform.parent.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * radius;
        axis = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        rotationEulers = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * rotationSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.RotateAround(transform.parent.position, axis, speed);
        transform.Rotate(rotationEulers);
    }
}
