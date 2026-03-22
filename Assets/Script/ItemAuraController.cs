using UnityEngine;

public class ItemAuraController : MonoBehaviour
{
    [Header("Aura")]
    public Color auraColor = new Color(0.4f, 0.8f, 1f, 1f);
    public float auraIntensity = 2f;
    public float auraRange = 4f;

    [Header("Pulse")]
    public bool pulse = true;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.3f;

    private Light auraLight;
    private float baseIntensity;

    void Start()
    {
        auraLight = GetComponent<Light>();
        if (auraLight == null)
            auraLight = gameObject.AddComponent<Light>();

        auraLight.type = LightType.Point;
        auraLight.color = auraColor;
        auraLight.intensity = auraIntensity;
        auraLight.range = auraRange;
        auraLight.enabled = false;
        baseIntensity = auraIntensity;
    }

    void Update()
    {
        if (auraLight == null || !auraLight.enabled || !pulse) return;
        auraLight.intensity = baseIntensity
            + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
    }

    public void EnableAura()
    {
        if (auraLight != null) auraLight.enabled = true;
    }

    public void DisableAura()
    {
        if (auraLight != null) auraLight.enabled = false;
    }
}
