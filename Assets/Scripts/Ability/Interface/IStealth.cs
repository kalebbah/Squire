using UnityEngine;

public interface IStealth {
    float Duration { get; }
    void ApplyStealth(GameObject target);
}
