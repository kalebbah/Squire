using UnityEngine;

public interface IShield {
    int ShieldAmount { get; }
    float Duration { get; }
    void ApplyShield(GameObject target);
}
