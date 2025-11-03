using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ghost : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform Player;
    [SerializeField] float speed = 30.0f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Player = FindObjectOfType<PlayerController>().transform;   
    }


    private void FixedUpdate()
    {
        if (Player == null) return;

        float currentSpeed = speed * GameManager.ghostSpeedMulitplier;

        Vector2 dir = (Player.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir* speed * Time.fixedDeltaTime);
    }

}
