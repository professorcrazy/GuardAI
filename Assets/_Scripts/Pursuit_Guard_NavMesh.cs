using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Pursuit_Guard_NavMesh : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform target;
    [SerializeField] Vector3 targetPos = Vector3.zero;
    [SerializeField] Vector3 lastKnownPos = Vector3.zero;
    [SerializeField] Vector3 lastSeenPos = Vector3.zero;
    //public float rangeSqr;
    public float viewingRadius = 10f;
    public float huntRadius = 10f;

    public Color huntingTargetColor = Color.red;
    public Color searchingTargetColor = new Color(1f,0.5f,0f);
    public Color waypointTargetColor = Color.blue;
    Renderer myRenderer;
    public Transform[] waypoints;
    public int currentWaypointID = 0;

    public enum State {
        patrol,
        hunt,
        search,
        attack
    }

    State currentState;

    //Movement
    public float speed = 5f;
    public float stoppingDistance = 0.5f;
    [SerializeField] Vector3 move;

    //Pursuit steps ahead of the target
    public float predictionSteps = 5f;
    Rigidbody rb;
    public float searchTime = 2f;
    [SerializeField] float currentSearchTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.patrol;
        myRenderer = GetComponent<Renderer>();
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
          
            lastSeenPos = target.position;
        }
        rb = GetComponent<Rigidbody>();
        lastKnownPos = target.position;
    }

    private bool LookForPlayer() {
        RaycastHit hit;
        Physics.Raycast(transform.position, (target.position - transform.position), out hit, viewingRadius);

        //// Does the ray intersect any objects excluding the player layer
        if (!hit.Equals(null)) {
            //if we see the target
            if (hit.transform == target) {
                lastSeenPos = target.position;
                return true;
            }
        }
        return false;
    }

    // FixedUpdate is run 50 times per second
    void FixedUpdate()
    {
        if (currentState == State.patrol) {
            if (LookForPlayer()) {
                myRenderer.material.color = huntingTargetColor;
                currentState = State.hunt;
            }
            else {
                Walk(waypoints[currentWaypointID].position);
            }
        }
        if (currentState == State.hunt) {
            if (LookForPlayer()) {
                targetPos = target.position;
                HuntPlayer(targetPos);
            }
            else {
                currentSearchTime = Time.time + searchTime;
                myRenderer.material.color = searchingTargetColor;
                currentState = State.search;
            }
        }
        if (currentState == State.search) {
            if (LookForPlayer()) {
                myRenderer.material.color = huntingTargetColor;
                currentState = State.hunt;
            }
            else {
                Walk(lastSeenPos);
                if (currentSearchTime <= Time.time) {
                    myRenderer.material.color = waypointTargetColor;
                    currentState = State.patrol;
                }
            }
        }
    }
    void HuntPlayer(Vector3 pos) {
        //Calculating steps
        Vector3 targetDir = target.position - lastKnownPos;
        Vector3 targetPredictionPos = target.position + (predictionSteps * targetDir);
        Debug.DrawLine(transform.position, targetPredictionPos, Color.red);
        Walk(targetPredictionPos);
        lastKnownPos = target.position;
    }
    void Walk(Vector3 pos) {
        //Follow
        float dist = Vector3.Distance(transform.position, pos);
        if (dist < stoppingDistance)
        {
            currentWaypointID = (currentWaypointID + 1) % waypoints.Length;
        }
        else if (currentState == State.patrol)
        {
            targetPos = waypoints[currentWaypointID].position;
        }
        transform.LookAt(pos);
        agent.SetDestination(pos);
    }
    Transform FindNearestWaypoint()
    {
        Transform nearestWaypoint = waypoints[0];
        float dist = (float) int.MaxValue;
        for (int i = 0; i < waypoints.Length; i++)
        {
            float tempDist = Vector3.Distance(transform.position, waypoints[i].position);
            if (tempDist < dist)
            {
                dist = tempDist;
                nearestWaypoint = waypoints[i];
            }
        }
        return nearestWaypoint;
    }
}
