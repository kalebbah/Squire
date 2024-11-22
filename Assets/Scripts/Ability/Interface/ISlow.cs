using UnityEngine;

public interface ISlow {
    float SlowAmount { get; }  // Percentage reduction
    float Duration { get; }
    void ApplySlow(GameObject target);
}
