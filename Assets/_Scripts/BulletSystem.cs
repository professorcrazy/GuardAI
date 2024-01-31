using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class BulletSystem : MonoBehaviour
{
    [SerializeField] float damage = 20f;

    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
        Destroy(gameObject);
    }
}
