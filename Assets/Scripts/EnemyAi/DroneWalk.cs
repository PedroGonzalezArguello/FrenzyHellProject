using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneWalk : BaseState
{
    int wayPointCounter = 0;
    bool isWaiting;
    float waitingTime;
    float viewAngle = 50;
    List<Transform> target;

    public DroneWalk (List<Transform> _target)
    {
        target = _target;
    }
    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        Debug.Log("Caminando");
        #region Comentado Player cercano
        var playerInRange = Physics.OverlapSphere(_enemy.transform.position, 5f, _playerMask);
        //    /*.OrderBy(x => Vector3.Distance(_enemy.transform.position, x.transform.position))
        //    .FirstOrDefault().gameObject.GetComponent<Player>()*/;
        //if (target == null && playerInRange.Any())
        /*{
            target = playerInRange[0].transform;
            for (int i = playerInRange.Length > 1 ? 1 : 0; i < playerInRange.Length; i++)
            {
               target = i != 0 && Vector3.Distance(playerInRange[i].transform.position, _enemy.transform.position) <
                     Vector3.Distance(target.transform.position, _enemy.transform.position) ?
                    playerInRange[i].transform : target;
            }

           _enemy.ActualState = _enemy._posibleStates[EnemyIA.IAStates.CHASE];


            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 5f);
    }*/

        #endregion

        var posiblePlayer = Physics.OverlapSphere(_enemy.transform.position, 5, _playerMask);

        if (posiblePlayer.Length > 0)
        {
            var playerDir = (posiblePlayer[0].transform.position - _enemy.transform.position);
            if (Vector3.Angle(_enemy.transform.forward, playerDir) < viewAngle / 2 &&
                !Physics.Raycast(_enemy.transform.position,
                playerDir, playerDir.magnitude, _enemy.obstacleMask))
            {
                _enemy.target = posiblePlayer[0].transform;
                _enemy.ChangeState(EnemyIA.IAStates.CHASE);
                Debug.Log("Encontre");
                return;
            }
        }

        #region Patrol
        var actualDir = (_enemy.waypoints[wayPointCounter].position - _enemy.transform.position);
        actualDir.y = 0;

        _enemy.transform.forward = (_enemy.transform.forward * 0.99f + actualDir.normalized * 0.01f);

        if (isWaiting)
        {
            waitingTime -= Time.deltaTime;
            if (waitingTime < 0)
                isWaiting = false;
            return;
        }

        _enemy.transform.position +=
            actualDir.normalized
            * Time.deltaTime * _enemy.speed;

        if (actualDir.magnitude < .2f)
        {
            wayPointCounter++;

            if (wayPointCounter >= _enemy.waypoints.Count)
            {
                wayPointCounter = 0;
            }
            isWaiting = true;
            waitingTime = 1;
        }
        #endregion
    }
}
