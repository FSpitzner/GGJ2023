using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContactPoint : MonoBehaviour, IDamageable
{
    private int health = 1;

    public void ApplyDamage(int damage = 1)
    {
        health -= damage;

        if (health <= 0)
            Destroy(gameObject);
    }
}
