using UnityEngine;
public class FollowTarget : MonoBehaviour
{
    Transform target;
    [SerializeField] Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = target.transform.position + offset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = target.transform.position + offset;
    }
}
