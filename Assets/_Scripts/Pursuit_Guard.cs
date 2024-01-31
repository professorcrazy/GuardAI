using UnityEngine;

public class Pursuit_Guard : MonoBehaviour
{
    public Transform target;
    Vector3 targetPos = Vector3.zero;
    Vector3 lastKnownPos = Vector3.zero;
    Vector3 lastSeenPos = Vector3.zero;
    //public float rangeSqr;
    public float viewingRadius = 10f;
    public float huntRadius = 10f;

    public Color huntingTargetColor = Color.red;
    public Color searchingTargetColor = new Color(1f,0.5f,0f);
    public Color waypointTargetColor = Color.blue;
    Renderer myRenderer;
    public Transform[] waypoints;
    int currentWaypointID = 0;

    public enum State {
        waypoint,
        hunt,
        search
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
    float currentSearchTime = 0f;

    void Start()
    {
        currentState = State.waypoint;
        myRenderer = GetComponent<Renderer>();
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            lastSeenPos = target.position;
        }
        rb = GetComponent<Rigidbody>();
        lastKnownPos = target.position;
    }

    

    // FixedUpdate is run 50 times per second
    void FixedUpdate()
    {
        if (currentState == State.waypoint) {
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
                    currentState = State.waypoint;
                }
            }
        }
    }
    private bool LookForPlayer()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, (target.position - transform.position), out hit, viewingRadius);

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
        if (dist < stoppingDistance) {
            currentWaypointID = (currentWaypointID + 1) % waypoints.Length;
        }else if(currentState == State.waypoint) {
            targetPos = waypoints[currentWaypointID].position;
        }
        move = transform.forward;
        transform.LookAt(pos + Vector3.up);

        rb.velocity = move.normalized * (speed);
    }
}
