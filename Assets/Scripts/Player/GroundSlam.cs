using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlam : MonoBehaviour
{
    private float _speed;
    private DavesPM _player;
    
    public GroundSlam(float speed, DavesPM player)
    {
        _speed = speed;
        _player = player;
    }

    public IEnumerator DoGroundSlam()
    {
        while (!_player.grounded)
        {
            _player.transform.Translate(Vector3.down * _speed * Time.deltaTime, Space.World);
            yield return null;
        }
    }
}
