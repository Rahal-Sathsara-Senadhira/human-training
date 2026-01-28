using UnityEngine;
using UnityEngine.UI;

public class WorldHealthBar : MonoBehaviour
{
    [Header("UI")]
    public Slider slider;

    [Header("Follow")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0.25f, 0f);
    public Camera faceCamera;

    private MonoBehaviour healthMb;
    private Health healthTyped;

    public void Init(MonoBehaviour health, Transform followTarget, Camera cam)
    {
        healthMb = health;
        healthTyped = health as Health;

        target = followTarget;
        faceCamera = cam;

        if (slider == null) slider = GetComponentInChildren<Slider>(true);

        // initial set
        UpdateBar();

        // subscribe if possible
        if (healthTyped != null)
        {
            healthTyped.OnHealthChanged += OnHealthChanged;
        }
    }

    private void OnDestroy()
    {
        if (healthTyped != null)
            healthTyped.OnHealthChanged -= OnHealthChanged;
    }

    private void LateUpdate()
    {
        if (target != null)
            transform.position = target.position + offset;

        if (faceCamera != null)
        {
            // Make bar face the camera
            transform.forward = faceCamera.transform.forward;
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        if (slider == null) return;
        slider.minValue = 0f;
        slider.maxValue = max;
        slider.value = current;
    }

    private void UpdateBar()
    {
        if (slider == null) return;

        if (healthTyped != null)
        {
            slider.minValue = 0f;
            slider.maxValue = healthTyped.MaxHealth;
            slider.value = healthTyped.CurrentHealth;
        }
        else
        {
            // if no Health found, keep full
            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.value = 100f;
        }
    }
}
