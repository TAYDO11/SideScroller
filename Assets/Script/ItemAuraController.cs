using UnityEngine;

public class ItemAuraController : MonoBehaviour
{
    [Header("Aura visuelle")]
    public GameObject auraPrefab; // un sprite cercle flou
    public Color auraColor = new Color(0.4f, 0.8f, 1f, 0.4f);

    [Header("Pulse")]
    public bool pulse = true;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.08f;

    private GameObject auraInstance;
    private SpriteRenderer auraRenderer;
    private Vector3 baseScale;

    public void EnableAura()
    {
        if (auraPrefab == null) return;

        Transform player = transform.root;
        auraInstance = Instantiate(auraPrefab, player.position, Quaternion.identity);
        auraInstance.transform.SetParent(player);
        auraInstance.transform.localPosition = new Vector3(0, 0, 0.1f);

        auraRenderer = auraInstance.GetComponent<SpriteRenderer>();
        if (auraRenderer != null) auraRenderer.color = auraColor;
        baseScale = auraInstance.transform.localScale;
    }

    public void DisableAura()
    {
        if (auraInstance != null) Destroy(auraInstance);
    }

    void Update()
    {
        if (auraInstance == null || !pulse) return;

        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        auraInstance.transform.localScale = baseScale * scale;
    }
}
