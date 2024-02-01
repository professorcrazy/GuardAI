using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour, IDamageable
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

    [Header("Attack Settings")]
    public GameObject bulletPrefab;
    public float rps = 1/2f;
    public float bulletSpeed = 10f;
    public Transform bulletSpawnPos;
    private float lastShot = 0;
    public float attackRange = 10f;

    [Header("HP Settings")]
    public float maxHP = 100;
    public float currentHP;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
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

            //Check for attack state
            if(agent.remainingDistance < attackRange)
            {
                currentState = State.attack;
            }
            else
            {
                agent.SetDestination(lastKnownPos);
            }
        }
        else if (currentState != State.patrol)
        {
            currentState = State.searching;
        }

        //add attack state
        if (currentState == State.attack)
        {
            if (!DetectedPlayer())
            {
                currentState = State.searching;
            }
//            agent.stoppingDistance = attackRange/2f;
            agent.SetDestination(lastKnownPos);
            RangeAttack();
        }

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
        if(lastShot + rps < Time.time)
        {
            GameObject tempBullet = Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity);
            tempBullet.GetComponent<Rigidbody>().velocity = bulletSpawnPos.forward * bulletSpeed;
            lastShot = Time.time;

        }
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

    public void TakeDamage(float damageAmount)
    {
        //implement damage
        currentHP -= damageAmount;
        if (currentHP <= 0) {
            Destroy(gameObject);
        }
    }
}
