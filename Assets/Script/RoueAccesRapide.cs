using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Roue d'accès rapide — version OLD INPUT SYSTEM
/// 
/// Dans Edit > Project Settings > Input Manager :
/// - Crée un axe "OuvrirRoue" → Positive Button : joystick button 4 (L1 manette)
///   ou mets "left shift" pour tester au clavier
/// - Les axes "Horizontal" et "Vertical" existent déjà par défaut
/// </summary>
public class RoueAccesRapide : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private GameObject panneauRoue;
    [SerializeField] private Transform parentSlots;
    [SerializeField] private SlotPouvoirUI prefabSlot;
    [SerializeField] private TextMeshProUGUI texteNomSelectionne;
    [SerializeField] private TextMeshProUGUI texteDescSelectionne;

    [Header("Paramètres")]
    [SerializeField] private float rayonRoue = 140f;
    [SerializeField] private float ralentiTemps = 0.25f;
    [SerializeField] private float seuilStick = 0.4f;

    [Header("Touches (Input Manager)")]
    [SerializeField] private string boutonOuvrir = "OuvrirRoue";
    [SerializeField] private string axeHorizontal = "Horizontal";
    [SerializeField] private string axeVertical = "Vertical";

    private bool roueouverte = false;
    private int indexSelectionne = -1;
    private List<SlotPouvoirUI> slotsSpawnes = new List<SlotPouvoirUI>();

    void Start()
    {
        panneauRoue.SetActive(false);
        GestionnairePouvoirs.Instance.SurCollecte.AddListener((_) => ReconstruireRoue());
    }


    void Update()
    {
        if (Input.GetButtonDown(boutonOuvrir))
            OuvrirRoue();

        if (Input.GetButtonUp(boutonOuvrir))
            FermerRoue();

        if (roueouverte)
        {
            float h = Input.GetAxisRaw(axeHorizontal);
            float v = Input.GetAxisRaw(axeVertical);
            if (new Vector2(h, v).magnitude >= seuilStick)
                SelectionnerDepuisDirection(new Vector2(h, v));
        }
    }

    void OuvrirRoue()
    {
        if (roueouverte) return;
        roueouverte = true;
        panneauRoue.SetActive(true);
        Time.timeScale = ralentiTemps;
        ReconstruireRoue();
    }

    void FermerRoue()
    {
        if (!roueouverte) return;
        roueouverte = false;

        if (indexSelectionne >= 0)
        {
            var pouvoirs = GestionnairePouvoirs.Instance.ObtenirPouvoirs();
            if (indexSelectionne < pouvoirs.Count)
            {
                var joueur = GameObject.FindWithTag("Player");
                GestionnairePouvoirs.Instance.ActiverPouvoir(
                    pouvoirs[indexSelectionne],
                    joueur.transform
                );
            }
        }

        indexSelectionne = -1;
        Time.timeScale = 1f;
        panneauRoue.SetActive(false);
        MettreAJourTexteInfo(null);
    }

    void ReconstruireRoue()
    {
        Debug.Log("ReconstruireRoue appelé, nb pouvoirs : " + GestionnairePouvoirs.Instance.ObtenirPouvoirs().Count);

        foreach (var s in slotsSpawnes) Destroy(s.gameObject);
        slotsSpawnes.Clear();

        var pouvoirs = GestionnairePouvoirs.Instance.ObtenirPouvoirs();
        int nb = pouvoirs.Count;
        if (nb == 0) return;

        for (int i = 0; i < nb; i++)
        {
            float angle = (360f / nb) * i - 90f;
            float rad   = angle * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * rayonRoue;

            var slot = Instantiate(prefabSlot, parentSlots);
            slot.GetComponent<RectTransform>().anchoredPosition = pos;
            slot.Configurer(pouvoirs[i], i, this);
            slotsSpawnes.Add(slot);
        }
    }

    void SelectionnerDepuisDirection(Vector2 dir)
    {
        var pouvoirs = GestionnairePouvoirs.Instance.ObtenirPouvoirs();
        if (pouvoirs.Count == 0) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        float pasAngle = 360f / pouvoirs.Count;
        int   index    = Mathf.RoundToInt(angle / pasAngle) % pouvoirs.Count;

        SelectionnerSlot(index);
    }

    public void SelectionnerSlot(int index)
    {
        indexSelectionne = index;
        for (int i = 0; i < slotsSpawnes.Count; i++)
            slotsSpawnes[i].DefinirSelectionne(i == index);

        var pouvoirs = GestionnairePouvoirs.Instance.ObtenirPouvoirs();
        if (index >= 0 && index < pouvoirs.Count)
            MettreAJourTexteInfo(pouvoirs[index]);
    }

    void MettreAJourTexteInfo(PouvoirData pouvoir)
    {
        if (texteNomSelectionne)
            texteNomSelectionne.text  = pouvoir != null ? pouvoir.nomPouvoir  : "";
        if (texteDescSelectionne)
            texteDescSelectionne.text = pouvoir != null ? pouvoir.description : "";
    }
}
