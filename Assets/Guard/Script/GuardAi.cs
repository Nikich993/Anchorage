using UnityEngine;
using UnityEngine.AI;

public class GuardAi : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float dayRadius = 10f;
    public bool isNight = false;

    NavMeshAgent agent;
    Transform player;
    int currentPoint = 0;
    bool chasing = false;
    Vector3 lastKnownPos;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.destination = patrolPoints[currentPoint].position;
    }

    void Update()
    {
        if (!chasing)
            Patrol();

        DetectPlayer();
        Chase();

        if (chasing)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0;

            transform.forward = Vector3.Lerp(
                transform.forward,
                dir.normalized,
                Time.deltaTime * 5f
            );
        }
    }

    void Patrol()
    {
        if (agent.remainingDistance < 0.3f)
        {
            currentPoint++;

            if (currentPoint >= patrolPoints.Length)
                currentPoint = 0;

            agent.destination = patrolPoints[currentPoint].position;
        }
    }

    void DetectPlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < dayRadius)
        {
            chasing = true;
            lastKnownPos = player.position;
        }
    }

    void Chase()
    {
        if (!chasing) return;

        agent.destination = lastKnownPos;

        if (agent.remainingDistance < 0.2f)
        {
            chasing = false;
            agent.destination = patrolPoints[currentPoint].position;
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > dayRadius * 2f)
        {
            chasing = false;
            agent.destination = patrolPoints[currentPoint].position;
        }
    }
}
