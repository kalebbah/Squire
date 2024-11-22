using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public DamageTextManager damageNumbersManager;
    public Toggle damageNumbersToggle;

    private void Start()
    {
        damageNumbersToggle.onValueChanged.AddListener((value) => damageNumbersManager.ToggleDamageNumbers(value));
    }
}
