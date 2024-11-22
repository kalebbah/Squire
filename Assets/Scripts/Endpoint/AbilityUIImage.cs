using UnityEngine;
using UnityEngine.UI;

public class AbilityUIImage : MonoBehaviour
{
    public BaseAbility ability;
    public Image abilityImage;

    public void Initialize(BaseAbility newAbility, Sprite abilitySprite)
    {
        if (newAbility != null) 
        {
            ability = newAbility;
            Debug.Log("Ability assigned.");
        }  
        else 
        {
            Debug.Log("No ability assigned.");
        }
        
        if (abilityImage != null)
        {
            abilityImage.type = Image.Type.Filled;
            abilityImage.fillMethod = Image.FillMethod.Radial360;
            abilityImage.sprite = abilitySprite;
            abilityImage.fillAmount = 1f; // Start fully filled
            Debug.Log("Image initialized.");
        } 
        else 
        {
            Debug.Log("No image assigned.");
        }
    }

    public void UpdateCooldown()
    {
        if (ability != null && abilityImage != null)
        {
            float timeSinceActivation = Time.time - ability.lastActivationTime;

            if (ability.IsReady())
            {
                abilityImage.fillAmount = 1f;
                //Debug.Log($"{ability.Name} is ready, fillAmount set to 1.");
            }
            else
            {
                float fillAmount = Mathf.Clamp01(timeSinceActivation / ability.Cooldown);
                abilityImage.fillAmount = 1f - fillAmount; // Reverse to show countdown from full to empty
                //Debug.Log($"{ability.Name} fillAmount set to {abilityImage.fillAmount} based on cooldown.");
            }
        }
    }
}
