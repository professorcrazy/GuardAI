using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardMove : MonoBehaviour
{
    public Transform target;
    NavMeshAgent agent;
    public float areaDetectRadius = 10;
    public LayerMask layersToDetect;

    public enum GuardState
    {
        Hunt,
        Search,
        Patrol,
        Attack
    }
    public GuardState currentState;
    public float viewDist = 20f;
    [SerializeField] private int currentWaypointIndex;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float stoppingDistance;
    private Vector3 lastSeenPos;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();   
        agent.SetDestination(transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentState == GuardState.Hunt)
        {

            if (CanSeeTarget(DetectClosestEnemy()) != null)
            {
                target = CanSeeTarget(DetectClosestEnemy());
                agent.SetDestination(target.position);
            }
            else
            {
                currentState = GuardState.Search;
            }
        }
        if (currentState == GuardState.Search) {
            SearchForEnemy();
        }
        if( currentState == GuardState.Patrol)
        {
            if (CanSeeTarget(DetectClosestEnemy()) != null)
            {
                currentState = GuardState.Hunt;
                return;
            }
            FindClosestWaypoint();

        }
    }

    private bool SearchForEnemy()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, (target.position - transform.position), out hit, viewDist);

        //// Does the ray intersect any objects excluding the player layer
        if (!hit.Equals(null))
        {
            //if we see the target
            if (hit.transform == target)
            {
                lastSeenPos = target.position;
                return true;
            }
        }
        return false;
    }

    public Transform CanSeeTarget(Transform searchTarget)
    {
        if (searchTarget == null)
        {
            return null;
        }
        RaycastHit hit;
        Physics.Raycast(transform.position, target.position - transform.position, out hit, viewDist, layersToDetect);
        if (hit.collider != null) {
            searchTarget = hit.transform;
        }
        else
        {
            searchTarget = null;
        }
        return searchTarget;
    }

    private Transform DetectClosestEnemy()
    {
        Transform closestEnemy = null;
        float distance = float.MaxValue;
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, areaDetectRadius, layersToDetect);
        foreach (Collider enemy in enemiesInRange)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < distance)
            {
                distance = Vector3.Distance(transform.position, enemy.transform.position);
                closestEnemy = enemy.transform;
                
            }
        }
        return closestEnemy;
    }

    private void FindClosestWaypoint()
    {
//        Transform closestWaypoint = null;
        float dist = float.MaxValue;
        for (int i = 0; i < waypoints.Length; i++) { 
            float tempDist = Vector3.Distance(transform.position, waypoints[i].position);
            if (tempDist < dist) {
                dist = tempDist;
//                closestWaypoint = waypoints[i];
                currentWaypointIndex = i;
            }
        }
//        return closestWaypoint;
    }

    private void Patrolling()
    {

        float dist = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
        if (dist <= stoppingDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }
}
