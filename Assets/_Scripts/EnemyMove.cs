using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    [Header("Navigation Settings")]
    NavMeshAgent agent;
    public Transform target;
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    [SerializeField] float stoppingDistance = 2f;
    public float viewDist = 20f;
    private Vector3 lastKnownPos = Vector3.zero;


    public enum State
    {
        patrol,
        hunt,
        search,
        attack,
        searching
    }
    [Header("State Machine Settings")]
    public State currentState;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.patrol;
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //    agent.SetDestination(target.position);
        if (DetectedPlayer())
        {
            currentState = State.hunt;
            lastKnownPos = target.position;
            agent.SetDestination(lastKnownPos);
            //Check for attack state
        }
        else if (currentState != State.patrol)
        {
            currentState = State.searching;
        }

        //add attack state

        if (currentState == State.searching)
        {
            agent.SetDestination(lastKnownPos);
            if (agent.remainingDistance < stoppingDistance)
            {
                currentWaypointIndex = ReturnToClosestWaypointRoute();
                agent.SetDestination(waypoints[currentWaypointIndex].position);
                currentState = State.patrol;
            }
        }
        if (currentState == State.patrol)
        {
            Patrolling();
        }
    }

    public void RangeAttack()
    {
        //input range attack code
    }
    private void Searching()
    {

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

    private bool DetectedPlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, target.position - transform.position, out hit, viewDist))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private int ReturnToClosestWaypointRoute()
    {
        int waypointIndex = 0;
        float tempDist = float.MaxValue;
        
        for (int i = 0; i < waypoints.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, waypoints[i].position);
            if (dist < tempDist)
            {
                tempDist = dist;
                waypointIndex = i;
            }
        }
        return waypointIndex;
    }
    
}
