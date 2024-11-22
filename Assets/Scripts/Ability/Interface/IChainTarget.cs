using UnityEngine;

public interface IChainTarget {
    int MaxChains { get; }
    float ChainRange { get; }
    void ApplyChain(GameObject initialTarget);
}
