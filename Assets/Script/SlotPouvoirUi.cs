using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotPouvoirUi : MonoBehaviour
{
    [SerializeField] private Image imageIcone;
    [SerializeField] private Image cercleFond;
    [SerializeField] private Image cercleCooldown;
    [SerializeField] private TextMeshProUGUI texteNom;
    [SerializeField] private GameObject indicateurSelectionne;

    [Header("Couleurs")]
    [SerializeField] private Color couleurNormale = new Color(1f, 1f, 1f, 0.15f);
    [SerializeField] private Color couleurSelectionnee = new Color(1f, 0.86f, 0.31f, 0.9f);

    private PouvoirData donnees;
    private int monIndex;
    private RoueAccesRapide roue;

    void Update()
    {
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
        donnees = pouvoir;
        monIndex = index;
        roue = roueParente;

        imageIcone.sprite = pouvoir.icone;
        if (texteNom) texteNom.text = pouvoir.nomPouvoir;

        DefinirSelectionne(false);

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