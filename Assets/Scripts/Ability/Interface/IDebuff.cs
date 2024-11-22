using UnityEngine;

public interface IDebuff {
    StatType Stat { get; }
    float DebuffAmount { get; }
    float Duration { get; }
    void ApplyDebuff(GameObject target);
}
