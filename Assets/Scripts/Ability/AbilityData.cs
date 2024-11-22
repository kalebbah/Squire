using System.Collections.Generic;

[System.Serializable]
public class AbilityData {
    public string name;
    public string description;
    public float cooldown;
    public bool isActive;
    public List<string> abilityTypes;          // List of interfaces or types the ability implements
    public Dictionary<string, float> attributes; // Key-value pairs for additional properties
}
