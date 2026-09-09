using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class PlayPay : MonoBehaviour
{
    public Transform[] runPoints;   // точки для беготни
    public float viewRadius = 10f;  // радиус обзора игрока

    NavMeshAgent agent;
    int currentPoint = 0;
    bool running = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        CheckGuard();

        if (running)
        {
            RunAround();
        }
    }

    void CheckGuard()
    {
        // ищем всех охранников по тегу
        GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard");

        foreach (GameObject g in guards)
        {
            float dist = Vector3.Distance(transform.position, g.transform.position);

            // если охранник попал в радиус — начинаем бегать
            if (dist < viewRadius)
            {
                running = true;
                return; // достаточно одного охранника
            }
        }
    }

    void RunAround()
    {
        // если почти дошёл до точки — переключаемся на следующую
        if (agent.remainingDistance < 0.3f)
        {
            currentPoint++;

            if (currentPoint >= runPoints.Length)
                currentPoint = 0; // по кругу

            agent.destination = runPoints[currentPoint].position;
        }
    }
}
