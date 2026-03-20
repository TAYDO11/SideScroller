using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ─────────────────────────────────────────────────────────────────────────────
// SlotPouvoirUI — un bouton dans la roue radiale
// ─────────────────────────────────────────────────────────────────────────────
public class SlotPouvoirUI : MonoBehaviour
{
    [SerializeField] private Image imageIcone;
    [SerializeField] private Image cercleFond;
    [SerializeField] private Image cercleCooldown;   // Image type "Filled" pour l'arc de cooldown
    [SerializeField] private TextMeshProUGUI texteNom;
    [SerializeField] private GameObject indicateurSelectionne;

    [Header("Couleurs")]
    [SerializeField] private Color couleurNormale    = new Color(1f, 1f, 1f, 0.15f);
    [SerializeField] private Color couleurSelectionnee = new Color(1f, 0.86f, 0.31f, 0.9f);

    private PouvoirData donnees;
    private int         monIndex;
    private RoueAccesRapide roue;

    void Update()
    {
        // Mettre à jour l'arc de cooldown
        if (donnees != null && cercleCooldown != null)
        {
            bool enCD = GestionnairePouvoirs.Instance.EstEnCooldown(donnees);
            cercleCooldown.gameObject.SetActive(enCD);
            if (enCD)
            {
                float restant = GestionnairePouvoirs.Instance.ObtenirCooldownRestant(donnees);
                cercleCooldown.fillAmount = restant / donnees.cooldown;
            }
        }
    }

    public void Configurer(PouvoirData pouvoir, int index, RoueAccesRapide roueParente)
    {
        donnees  = pouvoir;
        monIndex = index;
        roue     = roueParente;

        imageIcone.sprite = pouvoir.icone;
        if (texteNom) texteNom.text = pouvoir.nomPouvoir;

        DefinirSelectionne(false);

        // Clic souris sur le slot
        var btn = GetComponent<Button>();
        if (btn) btn.onClick.AddListener(() => roue.SelectionnerSlot(monIndex));
    }

    public void DefinirSelectionne(bool selectionne)
    {
        cercleFond.color = selectionne ? couleurSelectionnee : couleurNormale;
        if (indicateurSelectionne)
            indicateurSelectionne.SetActive(selectionne);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// JouetCollectable — à mettre sur les jouets dans le niveau
// ─────────────────────────────────────────────────────────────────────────────
public class JouetCollectable : MonoBehaviour
{
    [Header("Quel pouvoir ce jouet donne ?")]
    [SerializeField] private PouvoirData pouvoirAccorde;

    [Header("Effets")]
    [SerializeField] private GameObject effetCollecte;   // Particules, son, etc.
    [SerializeField] private float rotationVitesse = 60f;

    void Update()
    {
        // Petite rotation pour attirer l'oeil
        transform.Rotate(Vector3.up, rotationVitesse * Time.deltaTime);
    }

    void OnTriggerEnter(Collider autre)
    {
        // Vérifie que c'est le joueur qui touche
        if (!autre.CompareTag("Player")) return;

        GestionnairePouvoirs.Instance.CollecterPouvoir(pouvoirAccorde);

        if (effetCollecte != null)
            Instantiate(effetCollecte, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
