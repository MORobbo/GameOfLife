using System;
using UnityEngine;

public class AccelerometerController : MonoBehaviour
{
    private IGrid m_gridBehaviours;

    void Awake()
    {
        m_gridBehaviours = GetComponent<IGrid>();
    }

    void Update()
    {
        Vector3 dir = Vector3.zero;

        dir.x = -Input.acceleration.y;
        dir.z = Input.acceleration.x;

        if (dir.sqrMagnitude > 1)
            dir.Normalize();

        dir *= Time.deltaTime;

        if (m_gridBehaviours.Playing && (dir.x > 0 || dir.y > 0))
        {
            m_gridBehaviours.Clear();
        }
    }
}
