using UnityEngine;

public abstract class BaseAbility
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public float Cooldown { get; protected set; }
    public bool IsActive { get; }
    public int Level { get; private set; } = 1;
    public int Count { get; set; }
    public float lastActivationTime;

    public bool IsCoroutineActive { get; private set; }

    protected BaseAbility(string name, string description, float cooldown, bool isActive)
    {
        Name = name;
        Description = description;
        Cooldown = cooldown;
        IsActive = isActive;
        Level = 1;
        Count = 1;
    }

    protected BaseAbility(string name, string description)
    {
        Name = name;
        Description = description;
        Cooldown = 0;
        IsActive = false;
        Level = 1;
        Count = 0;
    }

    public bool IsReady()
    {
        float modifiedCooldown = StatManager.Instance.CalculateCooldown(Cooldown);
        return Time.time >= lastActivationTime + modifiedCooldown;
    }

    public virtual void Activate(GameObject caster)
    {
        if (!IsActive) return;
        lastActivationTime = Time.time;
        Debug.Log($"{Name} activated by {caster.name}");
    }

    public virtual void LevelUp()
    {
        Level++;
        Debug.Log($"{Name} leveled up to {Level}");
    }

    public Sprite LoadAbilitySprite()
    {
        return Resources.Load<Sprite>($"AbilityUI/{Name}");
    }

    public void StartCooldown()
    {
        IsCoroutineActive = true;
        lastActivationTime = Time.time;
    }

    public void StopCooldown()
    {
        IsCoroutineActive = false;
    }

    public abstract string Print();
}
