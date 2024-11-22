using UnityEngine;

public interface IKnockback {
    float KnockbackForce { get; }
    void ApplyKnockback(GameObject caster, GameObject target);
}
