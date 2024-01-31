using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour, IDamageable
{
    float speed = 5f;
    Rigidbody rb;
    Vector3 moveDir;
    KeyCode runKay = KeyCode.LeftShift;
    public float walkSpeed = 5f, runSpeed = 8f;
    public float HP = 100f;
    private float currentHP;
    private Slider hpBar;

    // Start is called before the first frame update
    void Start(){
        currentHP = HP;
        hpBar = GameObject.FindGameObjectWithTag("HPBar").GetComponent<Slider>();
        hpBar.maxValue = HP;
        hpBar.value = currentHP;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update(){
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");
        moveDir.Normalize();

        if (Input.GetKeyDown(runKay))
        {
            speed = runSpeed;
        }
        if (Input.GetKeyUp(runKay))
        {
            speed = walkSpeed;
        }
        if (moveDir != Vector3.zero)
        {
            transform.forward = moveDir;
            //alternative method
            //Quaternion toRotate = Quaternion.LookRotation(moveDir, Vector3.up);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, 3 * Time.deltaTime);
        }
        rb.velocity = new Vector3(moveDir.x * speed, rb.velocity.y, moveDir.z * speed);
    }
    public void TakeDamage(float damageAmount)
    {
        currentHP -= damageAmount;
        hpBar.value = currentHP;
        if (currentHP <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        SpawnManager.Instance.RespawnPlayer(transform);
        currentHP = HP;
        hpBar.value += currentHP;
        Debug.Log("PlayerDied");
    }
}