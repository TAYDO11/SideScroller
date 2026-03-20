using UnityEngine;

public class jouetCollectable : MonoBehaviour
{
    [Header("Quel pouvoir ce jouet donne ?")]
    [SerializeField] private PouvoirData pouvoirAccorde;

    [Header("Effets")]
    [SerializeField] private GameObject effetCollecte;
    [SerializeField] private float rotationVitesse = 60f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationVitesse * Time.deltaTime);
    }

    void OnTriggerEnter(Collider autre)
    {
        if (!autre.CompareTag("Player")) return;

        GestionnairePouvoirs.Instance.CollecterPouvoir(pouvoirAccorde);

        if (effetCollecte != null)
            Instantiate(effetCollecte, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
