using UnityEngine;

public interface IRoot {
    float Duration { get; }
    void ApplyRoot(GameObject target);
}
