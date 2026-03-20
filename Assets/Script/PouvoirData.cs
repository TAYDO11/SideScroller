using UnityEngine;

[CreateAssetMenu(fileName = "NouveauPouvoir", menuName = "Pouvoirs/Pouvoir")]
public class PouvoirData : ScriptableObject
{
    [Header("Identité")]
    public string nomPouvoir;
    public Sprite icone;
    [TextArea] public string description;

    [Header("Capacité")]
    public GameObject prefabEffet; // Ce qui se passe quand on active le pouvoir
    public float cooldown = 2f;
}
