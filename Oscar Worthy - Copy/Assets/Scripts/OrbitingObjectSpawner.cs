using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingObjectSpawner : MonoBehaviour
{
    public GameObject orbitingObject;
    public List<GameObject> meshObjects = new List<GameObject>();

    public int numObjects;

    public float radiusMin;
    public float radiusMax;

    public float speedMin;
    public float speedMax;

    public float rotationSpeedMin;
    public float rotationSpeedMax;

    public float scaleMin;
    public float scaleMax;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numObjects; i++)
        {
            GameObject newObj = Instantiate(orbitingObject);
            newObj.transform.SetParent(transform);
            OrbitingObject orbitScript = newObj.GetComponent<OrbitingObject>();
            orbitScript.SetParams(radiusMin, radiusMax, speedMin, speedMax, rotationSpeedMin, rotationSpeedMax, scaleMin, scaleMax, meshObjects[Random.Range(0, meshObjects.Count)]);
            newObj.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
