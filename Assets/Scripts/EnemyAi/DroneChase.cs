using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneChase : BaseState
{
    public override void OnEnter()
    {

    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        Debug.Log("Persiguiendo");
        var dir = _enemy.target.transform.position - _enemy.transform.position;
        dir.y = 0;
        _enemy.transform.forward = (_enemy.transform.forward * 0.9f + dir * 0.1f);

        if (dir.magnitude < 1) return;

        _enemy.transform.position += _enemy.transform.forward * _enemy.speed * Time.deltaTime;
    }
}
