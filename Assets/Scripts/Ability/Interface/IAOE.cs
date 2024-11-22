using UnityEngine;

public interface IAOE {
    float AreaRadius { get; }
    void ApplyAOE(GameObject caster);
}
