using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HeldItemManager : MonoBehaviour
{
    [Header("Référence main")]
    public Transform handTransform;

    private GameObject currentHeldItem;
    private ItemAuraController currentAura;
    private int currentSlot = -1;

    void Update()
    {
        // Clavier : touches 1, 2, 3, 4
        for (int i = 0; i < 4; i++)
        {
            if (Keyboard.current[Key.Digit1 + i].wasPressedThisFrame)
            {
                SelectSlot(i);
                return;
            }

            if(currentHeldItem != null)
            {
                SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
                SpriteRenderer itemSprite = currentHeldItem.GetComponent<SpriteRenderer>();
                if (playerSprite != null && itemSprite != null)
                    itemSprite.flipX = playerSprite.flipX;
            }

        }

        // Manette PS : L1 + X = slot 0, L1 + carré = slot 1
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            if (gamepad.leftShoulder.isPressed && gamepad.buttonSouth.wasPressedThisFrame)
                SelectSlot(0);
            if (gamepad.leftShoulder.isPressed && gamepad.buttonWest.wasPressedThisFrame)
                SelectSlot(1);
        }
    }

    public void SelectSlot(int index)
    {
        // Vérifie que l'item est dans l'inventaire
        if (!inventory.instance.HasItemInSlot(index))
        {
            Debug.Log("Slot " + index + " vide.");
            return;
        }

        // Toggle
        if (currentSlot == index)
        {
            UnequipItem();
            return;
        }

        Item item = inventory.instance.GetItemInSlot(index);

        // Vérifie qu'un prefab est assigné sur l'item
        if (item.heldPrefab == null)
        {
            Debug.LogWarning("Pas de heldPrefab sur l'item " + index);
            return;
        }

        EquipItem(item.heldPrefab, index);
    }

    void EquipItem(GameObject prefab, int slotIndex)
    {
        if (currentHeldItem != null)
        {
            if (currentAura != null) currentAura.DisableAura();
            Destroy(currentHeldItem);
        }

        currentSlot = slotIndex;
        currentHeldItem = Instantiate(prefab, handTransform.position, Quaternion.identity);
        currentHeldItem.transform.SetParent(handTransform);
        currentHeldItem.transform.localPosition = Vector3.zero;

        currentAura = currentHeldItem.GetComponent<ItemAuraController>();
        if (currentAura != null) StartCoroutine(EnableAuraNextFrame(currentAura));
    }

    IEnumerator EnableAuraNextFrame(ItemAuraController aura)
    {
        yield return null;
        aura.EnableAura();
    }

    public void UnequipItem()
    {
        if (currentAura != null) currentAura.DisableAura();
        if (currentHeldItem != null) Destroy(currentHeldItem);
        currentHeldItem = null;
        currentAura = null;
        currentSlot = -1;
    }
}
