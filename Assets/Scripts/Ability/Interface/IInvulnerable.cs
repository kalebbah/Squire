using UnityEngine;

public interface IInvulnerable {
    float Duration { get; }
    void ApplyInvulnerability(GameObject target);
}
