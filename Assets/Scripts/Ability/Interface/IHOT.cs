using UnityEngine;

public interface IHOT {
    float Duration { get; }
    float TickInterval { get; }
    int HealPerTick { get; }
    void ApplyHOT(GameObject target);
}
