using UnityEngine;

public interface IBuff {
    StatType Stat { get; }
    float BuffAmount { get; }
    float Duration { get; }
    void ApplyBuff(GameObject target);
}
