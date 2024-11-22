using UnityEngine;

public interface IProjectile {
    float EffectAmount { get; }
    float ProjectileSpeed { get; }
    void LaunchProjectile(GameObject caster, GameObject target);
}
