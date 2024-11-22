using UnityEngine;

public interface IStun {
    float Duration { get; }
    void ApplyStun(GameObject target);
}
