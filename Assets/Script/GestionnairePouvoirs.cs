using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Gère tous les pouvoirs collectés par le joueur.
/// Un seul sur le joueur, reste actif tout le jeu.
/// </summary>
public class GestionnairePouvoirs : MonoBehaviour
{
    public static GestionnairePouvoirs Instance { get; private set; }

    [Header("Slots de la roue (max 6 recommandé)")]
    [SerializeField] private int nombreSlotsMax = 4;

    // Tous les pouvoirs collectés dans le niveau
    private List<PouvoirData> pouvoirsCollectes = new List<PouvoirData>();

    // Cooldowns en cours
    private Dictionary<PouvoirData, float> cooldownsEnCours = new Dictionary<PouvoirData, float>();

    // Événements
    public UnityEvent<PouvoirData> SurCollecte    = new UnityEvent<PouvoirData>();
    public UnityEvent<PouvoirData> SurActivation  = new UnityEvent<PouvoirData>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        // Décrémenter les cooldowns
        var cles = new List<PouvoirData>(cooldownsEnCours.Keys);
        foreach (var p in cles)
        {
            cooldownsEnCours[p] -= Time.deltaTime;
            if (cooldownsEnCours[p] <= 0f)
                cooldownsEnCours.Remove(p);
        }
    }

    // ── Collecte ──────────────────────────────────────────────────────────────

    /// <summary>
    /// À appeler quand le joueur touche un jouet dans le niveau.
    /// </summary>
    public void CollecterPouvoir(PouvoirData pouvoir)
    {
        if (pouvoirsCollectes.Contains(pouvoir)) return;
        if (pouvoirsCollectes.Count >= nombreSlotsMax)
        {
            Debug.Log("[Pouvoirs] Roue pleine ! Remplace un pouvoir ou augmente nombreSlotsMax.");
            return;
        }

        pouvoirsCollectes.Add(pouvoir);
        SurCollecte.Invoke(pouvoir);
        Debug.Log($"[Pouvoirs] Collecté : {pouvoir.nomPouvoir}");
    }

    public void SupprimerPouvoir(PouvoirData pouvoir)
    {
        pouvoirsCollectes.Remove(pouvoir);
        cooldownsEnCours.Remove(pouvoir);
    }

    public void ViderPouvoirs()
    {
        pouvoirsCollectes.Clear();
        cooldownsEnCours.Clear();
    }

    // ── Activation ────────────────────────────────────────────────────────────

    /// <summary>
    /// Appelle ça depuis la RoueAccesRapide quand le joueur choisit un pouvoir.
    /// </summary>
    public bool ActiverPouvoir(PouvoirData pouvoir, Transform lanceur)
    {
        if (!pouvoirsCollectes.Contains(pouvoir)) return false;
        if (EstEnCooldown(pouvoir)) return false;

        // Spawn de l'effet
        if (pouvoir.prefabEffet != null)
            Instantiate(pouvoir.prefabEffet, lanceur.position, lanceur.rotation);

        // Démarrer le cooldown
        if (pouvoir.cooldown > 0f)
            cooldownsEnCours[pouvoir] = pouvoir.cooldown;

        SurActivation.Invoke(pouvoir);
        Debug.Log($"[Pouvoirs] Activé : {pouvoir.nomPouvoir}");
        return true;
    }

    // ── Accesseurs ────────────────────────────────────────────────────────────

    public IReadOnlyList<PouvoirData> ObtenirPouvoirs() => pouvoirsCollectes;

    public bool EstEnCooldown(PouvoirData pouvoir) => cooldownsEnCours.ContainsKey(pouvoir);

    public float ObtenirCooldownRestant(PouvoirData pouvoir) =>
        cooldownsEnCours.TryGetValue(pouvoir, out float val) ? val : 0f;
}
