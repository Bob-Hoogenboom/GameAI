using UnityEngine;
using System.Collections.Generic;

public class BoidManager : MonoBehaviour
{
    //pacman example*
    public GameObject mainPrefab;

    public GameObject boidPrefab;
    public int boidCount = 10;
    public float speedLimit = 5f;
    public float neighborRadius = 3f;
    public float separationDistance = 2f;
    public Vector3 bounds = new Vector3(10f, 10f, 10f);

    private List<Boid> _boids = new List<Boid>();

    void Start()
    {
        InitializeMainBoid();
        InitializeBoids();
    }

    void Update()
    {
        MoveBoids();
    }

    private void InitializeMainBoid()
    {
        GameObject boidObj = Instantiate(mainPrefab, Random.insideUnitSphere * 10, Quaternion.identity);
        Boid boid = boidObj.GetComponent<Boid>();
        boid.velocity = Random.insideUnitSphere * speedLimit;
        _boids.Add(boid);
    }

    private void InitializeBoids()
    {
        for (int i = 0; i < boidCount; i++)
        {
            GameObject boidObj = Instantiate(boidPrefab, Random.insideUnitSphere * 10, Quaternion.identity);
            Boid boid = boidObj.GetComponent<Boid>();
            boid.velocity = Random.insideUnitSphere * speedLimit;
            _boids.Add(boid);
        }
    }

    private void MoveBoids()
    {
        foreach (Boid boid in _boids)
        {
            if (boid.isPerching)
            {
                if (boid.perchTimer > 0)
                {
                    boid.perchTimer -= Time.deltaTime;
                    continue;
                }
                else
                {
                    boid.isPerching = false;
                }
            }

            Vector3 cohesion = Cohesion(boid);
            Vector3 seperation = Seperation(boid);
            Vector3 alignment = Alignment(boid);
            Vector3 vBounds = BoundPosition(boid);

            boid.velocity += cohesion + seperation + alignment + vBounds;
            LimitVelocity(boid);
            boid.transform.position += boid.velocity * Time.deltaTime;
        }
    }

    private Vector3 Cohesion(Boid boid)
    {
        Vector3 perceivedCenter = Vector3.zero;
        int neighborCount = 0;

        foreach (Boid other in _boids)
        {
            if (other != boid && Vector3.Distance(boid.transform.position, other.transform.position) < neighborRadius)
            {
                perceivedCenter += other.transform.position;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            perceivedCenter /= neighborCount;
            return (perceivedCenter - boid.transform.position) / 100;
        }

        return Vector3.zero;
    }

    private Vector3 Seperation(Boid boid)
    {
        Vector3 separationVector = Vector3.zero;

        foreach (Boid other in _boids)
        {
            if (other != boid && Vector3.Distance(boid.transform.position, other.transform.position) < separationDistance)
            {
                separationVector -= (other.transform.position - boid.transform.position);
            }
        }

        return separationVector;
    }

    private Vector3 Alignment(Boid boid)
    {
        Vector3 perceivedVelocity = Vector3.zero;
        int neighborCount = 0;

        foreach (Boid other in _boids)
        {
            if (other != boid && Vector3.Distance(boid.transform.position, other.transform.position) < neighborRadius)
            {
                perceivedVelocity += other.velocity;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            perceivedVelocity /= neighborCount;
            return (perceivedVelocity - boid.velocity) / 8;
        }

        return Vector3.zero;
    }

    private Vector3 BoundPosition(Boid boid)
    {
        Vector3 force = Vector3.zero;

        if (boid.transform.position.x < -bounds.x) force.x = 10;
        else if (boid.transform.position.x > bounds.x) force.x = -10;

        if (boid.transform.position.y < -bounds.y) force.y = 10;
        else if (boid.transform.position.y > bounds.y) force.y = -10;

        if (boid.transform.position.z < -bounds.z) force.z = 10;
        else if (boid.transform.position.z > bounds.z) force.z = -10;

        return force;
    }

    private void LimitVelocity(Boid boid)
    {
        if (boid.velocity.magnitude > speedLimit)
        {
            boid.velocity = boid.velocity.normalized * speedLimit;
        }
    }
}