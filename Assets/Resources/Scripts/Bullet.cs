using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private float speed = 5;
    private Vector3 direction = new Vector3(0, 0, 1);

    private void Update()
    {
        transform.Translate(speed*  Time.deltaTime *direction);
    }
}
