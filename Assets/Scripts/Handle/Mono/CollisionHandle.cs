using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionHandle : MonoBehaviour
{
    public int collisionDamage;
    // Delegates to define actions for different collision events
    public Action<GameObject> OnCollisionEnterAction;
    public Action<GameObject> OnCollisionStayAction;

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionEnterAction?.Invoke(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        OnCollisionStayAction?.Invoke(collision.gameObject);
    }

}
