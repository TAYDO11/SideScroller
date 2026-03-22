using Unity.VisualScripting;
using UnityEngine;


public class Item : MonoBehaviour
{
    [Header("Item")]
    public Sprite icon;
    [SerializeField] private string nameItem;
    [SerializeField] private int amount;
    [SerializeField] private bool isUsed;

    [Header("Prefab pour la main")]
    public GameObject heldPrefab; // glisse le prefab ici dans l'Inspector


    public virtual void AddItemToInventory()
    {

        var haveFoundItem = false;

        for (int i = 0; i < inventory.instance.inventories.Length; i++)
        {
            if (inventory.instance.inventories[i] != null)
            {
                if (inventory.instance.inventories[i].nameItem == this.nameItem)
                {
                    if(inventory.instance.inventories[i].amount < 99)
                    {
                        haveFoundItem = true;
                        inventory.instance.inventories[i].amount += amount;
                        break;
                    }
                }
            }
        }

        if (haveFoundItem == false)
        {
            for (int i = 0; i < inventory.instance.inventories.Length; i++)
            {
                if(inventory.instance.inventories[i] == null)
                {
                    inventory.instance.inventories[i] = this;
                    break;
                }
            }
        }

        if (haveFoundItem) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (isUsed)
            {
                return;
            }

            GetComponent<SpriteRenderer>().enabled = false;

            foreach(var item in GetComponents<BoxCollider2D>())
            {
                item.isTrigger = true;
            }

            isUsed = true;

            AddItemToInventory();

            inventory.instance.LoadInventory();
        }
    }

}







