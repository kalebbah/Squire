using UnityEngine;
using UnityEngine.UI;

public class PlayerExperienceSlider : GenericSlider<ExperienceHandle>
{
    protected override void RegisterEvents()
    {
        EventManager<ExperienceHandle>.RegisterEvent(EventKey.UPDATE_SLIDER_EXPERIENCE_DISPLAY, UpdateSlider);
    }

    protected override void UnregisterEvents()
    {
        EventManager<ExperienceHandle>.UnregisterEvent(EventKey.UPDATE_SLIDER_EXPERIENCE_DISPLAY, UpdateSlider);
    }

    // Calculates the fill amount for health
    protected override float CalculateFillAmount(ExperienceHandle experienceData)
    {
        // Check to prevent division by zero
        if (experienceData.GetThreshold() == 0) 
        {
            return 0f; // Avoid division by zero, return 0 fill amount
        }

        // Calculate the fill amount
        float fillAmount = (float)experienceData.GetCollectedExperience() / experienceData.GetThreshold();

        // Clamp the value between 0 and 1
        fillAmount = Mathf.Clamp01(fillAmount);

        return fillAmount;
    }
    protected override void UpdateSlider(ExperienceHandle data)
    {
        float fillValue = CalculateFillAmount(data);

        // Update the fill value of the slider object
        if (sliderObject != null)
        {
            sliderObject.GetComponent<Image>().fillAmount = fillValue;
        }
    }

}
