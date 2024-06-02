using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenCover.Framework.Model;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public static List<Boid> boids;

    public Vector3 velocity;
    public Vector3 newVelocity;
    public Vector3 newPosition;

    public Boid closest;

    void Awake()
    {
        JoinFlock();
        RandomizePosition();
        RandomizeColor();
    }

    private void JoinFlock()
    {
        if (boids == null)
        {
            boids = new List<Boid>();
        }
        boids.Add(this);
        this.transform.parent = GameObject.Find("Boids").transform;
    }

    private void RandomizePosition()
    {
        Vector3 randPos = Random.insideUnitSphere * BoidSpawner.S.spawnRadius;
        randPos.y = 0;
        this.transform.position = randPos;
        velocity = Random.onUnitSphere;
        velocity *= BoidSpawner.S.spawnVelocity;
    }

    private void RandomizeColor()
    {
        Color color = PickRandomColor();
        ApplyColorToRenderers(color);
    }

    private Color PickRandomColor()
    {
        Color color = Color.black;
        while (color.r + color.g + color.b < 1.0f)
        {
            return new Color(Random.Range(0.4f, 1f), Random.Range(0.4f, 1f), Random.Range(0.4f, 1f));
        }
        return color;
    }

    private void ApplyColorToRenderers(Color color)
    {
        Renderer[] renderers = this.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.material.color = color;
        }
    }

    void Update()
    {
        List<Boid> neighbors = GetNeighbors(this);
        List<Boid> collisionRisks = GetCollisionRisks(this);

        newVelocity = velocity;

        // Alignment: Steer towards the average heading of local flockmates
        ApplyAlignment(neighbors);
        // Cohesion: Steer to move toward the average position of local flockmates
        ApplyCohesion(neighbors);
        // Separation: Steer to avoid crowding local flockmates
        ApplySeparation(collisionRisks);
        // Mouse Attraction: Steer to move toward the mouse cursor for player interaction 
        ApplyMouseAttraction();
    }

    void LateUpdate()
    {
        UpdateVelocity();
        UpdatePosition();
    }

    private void UpdateVelocity()
    {
        InterpolateVelocity();
        ClampVelocity();
    }
    private void InterpolateVelocity()
    {
        velocity = (1 - BoidSpawner.S.velocityLerpAmt) * velocity + BoidSpawner.S.velocityLerpAmt * newVelocity;
    }

    private void ClampVelocity()
    {
        if (velocity.magnitude > BoidSpawner.S.maxVelocity)
        {
            velocity = velocity.normalized * BoidSpawner.S.maxVelocity;
        }
        if (velocity.magnitude < BoidSpawner.S.minVelocity)
        {
            velocity = velocity.normalized * BoidSpawner.S.minVelocity;
        }
    }

    private void UpdatePosition()
    {
        newPosition = this.transform.position + velocity * Time.deltaTime;
        newPosition.y = 0;
        this.transform.LookAt(newPosition);
        this.transform.position = newPosition;
    }


    private List<Boid> GetNeighbors(Boid boi)
    {
        float closestDist = float.MaxValue;
        List<Boid> neighbors = new List<Boid>();

        foreach (Boid b in boids)
        {
            if (b == boi)
            {
                continue;
            }
            Vector3 delta = b.transform.position - boi.transform.position;
            if (delta.magnitude < closestDist)
            {
                closestDist = delta.magnitude;
                closest = b;
            }
            if (delta.magnitude < BoidSpawner.S.nearDist)
            {
                neighbors.Add(b);
            }
        }
        if (!neighbors.Any())
        {
            neighbors.Add(closest);
        }
        return neighbors;
    }
    private List<Boid> GetCollisionRisks(Boid boi)
    {
        List<Boid> collisionRisks = new List<Boid>();
        foreach (Boid b in boids)
        {
            if (b == boi)
            {
                continue;
            }
            Vector3 delta = b.transform.position - boi.transform.position;
            if (delta.magnitude < BoidSpawner.S.collisionDist)
            {
                collisionRisks.Add(b);
            }
        }
        return collisionRisks;
    }

    private void ApplyAlignment(List<Boid> neighbors)
    {
        Vector3 neighborVel = GetAverageVelocity(neighbors);
        newVelocity += neighborVel * BoidSpawner.S.velocityMatchingAmt;
    }

    private void ApplyCohesion(List<Boid> neighbors)
    {
        Vector3 neighborCenterOffset = GetAveragePosition(neighbors) - this.transform.position;
        newVelocity += neighborCenterOffset * BoidSpawner.S.flockCenteringAmt;
    }

    private void ApplySeparation(List<Boid> collisionRisks)
    {
        if (collisionRisks.Any())
        {
            Vector3 collisionAveragePos = GetAveragePosition(collisionRisks);
            Vector3 delta = collisionAveragePos - this.transform.position;
            newVelocity += delta * BoidSpawner.S.collisionAvoidanceAmt;
        }
    }

    private void ApplyMouseAttraction()
    {
        Vector3 delta = BoidSpawner.S.mousePosition - this.transform.position;
        if (delta.magnitude > BoidSpawner.S.mouseAvoidanceDist)
        {
            newVelocity += delta * BoidSpawner.S.mouseAttractionAmt;
        }
        else
        {
            newVelocity -= delta.normalized * BoidSpawner.S.mouseAvoidanceDist * BoidSpawner.S.mouseAvoidanceAmt;
        }
    }

    private Vector3 GetAveragePosition(List<Boid> someBoids)
    {
        Vector3 sum = Vector3.zero;
        foreach (Boid b in someBoids)
        {
            sum += b.transform.position;
        }
        Vector3 center = sum / someBoids.Count;
        return center;
    }

    private Vector3 GetAverageVelocity(List<Boid> someBoids)
    {
        Vector3 sum = Vector3.zero;
        foreach (Boid b in someBoids)
        {
            sum += b.velocity;
        }
        Vector3 avg = sum / someBoids.Count;
        return avg;
    }

}
