using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_moveSpeed = 10f;

    Rigidbody m_rb;
    Vector3 m_vel;

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var movInput = m_moveSpeed * (Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.forward * Input.GetAxisRaw("Vertical")).normalized;

        m_vel = movInput;
        m_vel.y += Physics.gravity.y;

        if (movInput.magnitude > 0)
            transform.rotation = Quaternion.LookRotation(movInput);
    }

    private void FixedUpdate()
    {
        m_rb.velocity = m_vel;
    }
}
