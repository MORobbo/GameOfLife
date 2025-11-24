using System;
using UnityEngine;

public class AccelerometerController : MonoBehaviour
{
    private IGrid m_gridBehaviours;

    void Awake()
    {
        m_gridBehaviours = GetComponent<IGrid>();
    }

    void FixedUpdate()
    {
        Vector3 dir = Vector3.zero;
        dir.x = -Input.acceleration.y;
        dir.z = Input.acceleration.x;
        float magnitude = Input.acceleration.magnitude;

        if (magnitude > 2.0f)
        {
            m_gridBehaviours.Clear();
        }
    }
}
