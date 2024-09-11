using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankEnemy : MonoBehaviour
{
    //Referencias
    public EnemiesScript enemy;
    public Transform enemyTransorm;
    public NavMeshAgent agent;

    //Stats
    public bool isEnraged;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<EnemiesScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.health <= 3 && !isEnraged)
        {
            Enraged();
        }
    }

    void Enraged()
    {
        isEnraged = true;
        enemy.timeBetweenAttacks = 1;
        enemyTransorm.localScale *= 2;
        agent.speed *= 3;
        agent.acceleration *= 3;
    }
}
