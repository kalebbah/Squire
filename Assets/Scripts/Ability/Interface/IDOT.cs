using UnityEngine;

public interface IDOT {
    float Duration { get; }
    float TickInterval { get; }
    int DamagePerTick { get; }
    void ApplyDOT(GameObject target);
}
