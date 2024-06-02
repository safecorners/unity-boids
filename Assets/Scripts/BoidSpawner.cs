using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoidSpawner : MonoBehaviour
{
    public static BoidSpawner S;


    public int numBoids = 100;
    public GameObject boidPrefab;
    public float spawnRadius = 100f;
    public float spawnVelocity = 10f;
    public float minVelocity = 0f;
    public float maxVelocity = 30f;
    public float nearDist = 30f;
    public float collisionDist = 5f;
    public float velocityMatchingAmt = 0.01f;
    public float flockCenteringAmt = 0.15f;
    public float collisionAvoidanceAmt = -0.5f;
    public float mouseAttractionAmt = 0.01f;
    public float mouseAvoidanceAmt = 0.75f;
    public float mouseAvoidanceDist = 15f;
    public float velocityLerpAmt = 0.25f;

    public bool ________________;

    public Vector3 mousePosition;
    // Start is called before the first frame update
    void Start()
    {
        S = this;
        for (int i = 0; i < numBoids; i++)
        {
            Instantiate(boidPrefab);
        }
    }

    void LateUpate()
    {
        UpdateMousePosition();
    }

    private void UpdateMousePosition()
    {
        Vector3 mousePos2d = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.transform.position.y);
        mousePosition = this.GetComponent<Camera>().ScreenToWorldPoint(mousePos2d);
    }
}
