using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
public class GuardAi : MonoBehaviour
{

    public Transform[] patrolPoints; // точки патруля
    public float dayRadius = 10f;    // радиус днём
    public float nightRadius = 8f;   // радиус ночью
    public float nightAngle = 45f;   // угол ночью
    public bool isNight = false;    // день/ночь
    bool forward = true;

    NavMeshAgent agent;              // агент
    Transform player;                // игрок
    int currentPoint = 0;            // текущая точка
    bool chasing = false;            // погоня
    Vector3 lastKnownPos;            // последняя позиция игрока

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // берём агент
        player = GameObject.FindGameObjectWithTag("Player").transform; // ищем игрока

        agent.destination = patrolPoints[currentPoint].position; // идём к первой точке
    }

    void Update()
    {
        if (chasing)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0;

            transform.forward = dir.normalized; // сразу смотрит на игрока
        }

        if (!chasing)
        {
            Patrol(); // если не гонится — патрулирует
        }

        DetectPlayer(); // проверяем игрока
        Chase();        // если гонится — идёт за игроком
    }

    void Patrol()
    {
        if (agent.remainingDistance < 0.3f)
        {
            if (forward)
            {
                currentPoint++;

                if (currentPoint >= patrolPoints.Length - 1)
                    forward = false; // дошли до конца — идём назад
            }
            else
            {
                currentPoint--;

                if (currentPoint <= 0)
                    forward = true; // дошли до начала — идём вперёд
            }

            agent.destination = patrolPoints[currentPoint].position;
        }
    }

    void DetectPlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        // ДЕНЬ — просто радиус
        if (!isNight)
        {
            if (dist < dayRadius && HasLineOfSight())
            {
                chasing = true;
                lastKnownPos = player.position;
            }
            return;
        }

        // НОЧЬ — радиус + угол
        if (dist < nightRadius)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);

            if (angle < nightAngle && HasLineOfSight())
            {
                chasing = true;
                lastKnownPos = player.position;
            }
        }
    }

    bool HasLineOfSight()
    {
        RaycastHit hit;
        Vector3 start = transform.position + Vector3.up * 1.7f; 
        Vector3 dir = player.position - start;

        if (Physics.Raycast(start, dir, out hit))
        {
            return hit.collider.CompareTag("Player"); 
        }

        return false;
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
    }


    void OnDrawGizmos()
    {
        // зелёный — дневной радиус
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, dayRadius);

        // красный — ночной радиус
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, nightRadius);

        // жёлтый — конус ночного зрения
        Gizmos.color = Color.yellow;

        Vector3 forward = transform.forward * nightRadius;

        // левая граница конуса
        Vector3 left = Quaternion.Euler(0, -nightAngle, 0) * forward;
        Gizmos.DrawLine(transform.position, transform.position + left);

        // правая граница конуса
        Vector3 right = Quaternion.Euler(0, nightAngle, 0) * forward;
        Gizmos.DrawLine(transform.position, transform.position + right);
    }


}
