using UnityEngine;

public interface ISummon {
    GameObject SummonedEntity { get; }
    void Summon(GameObject caster);
}
