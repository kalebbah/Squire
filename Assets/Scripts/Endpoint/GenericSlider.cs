using UnityEngine;
using UnityEngine.UI;

public abstract class GenericSlider<T> : MonoBehaviour
{
    public GameObject sliderObject;  // Reference to the slider's GameObject

    protected virtual void Awake()
    {
        RegisterEvents();  // Hook into events (can be overridden in derived classes)
    }

    // Abstract method to be implemented in derived classes to update the slider
    protected abstract float CalculateFillAmount(T data);

    // Method to update the slider fill
    protected abstract void UpdateSlider(T data);
        // float fillValue = CalculateFillAmount(data);
        // if (sliderObject != null)
        // {
        //     sliderObject.GetComponent<Image>().fillAmount = fillValue;
        // }


    // Register for events (to be overridden in derived classes)
    protected virtual void RegisterEvents() { }

    // Unregister events on destroy
    protected virtual void OnDestroy()
    {
        UnregisterEvents();
    }

    // Method to unregister events (to be overridden in derived classes)
    protected virtual void UnregisterEvents() { }
}
