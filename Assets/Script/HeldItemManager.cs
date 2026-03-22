using UnityEngine;
using UnityEngine.InputSystem;

public class HeldItemManager : MonoBehaviour
{
    [Header("Slots d'inventaire (assignés dans l'Inspector)")]
    public GameObject[] itemPrefabs; // slot 0 = touche 1, slot 1 = touche 2, etc.

    [Header("Référence main")]
    public Transform handTransform;

    private GameObject currentHeldItem;
    private ItemAuraController currentAura;
    private int currentSlot = -1;

    void Update()
    {
        // --- Clavier : touches 1, 2, 3... ---
        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            if (Keyboard.current[Key.Digit1 + i].wasPressedThisFrame)
            {
                SelectSlot(i);
                return;
            }
        }

        // --- Manette PS : L1 maintenu + X ---
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            if (gamepad.leftShoulder.isPressed && gamepad.buttonSouth.wasPressedThisFrame)
            {
                SelectSlot(0); // L1 + X → slot 0 (adapte selon tes besoins)
            }
            // Exemple : L1 + carré → slot 1
            if (gamepad.leftShoulder.isPressed && gamepad.buttonWest.wasPressedThisFrame)
            {
                SelectSlot(1);
            }
        }
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= itemPrefabs.Length) return;
        if (itemPrefabs[index] == null) return;

        // Même slot déjà équipé → déséquipe (toggle)
        if (currentSlot == index)
        {
            UnequipItem();
            return;
        }

        EquipItem(itemPrefabs[index], index);
    }

    void EquipItem(GameObject prefab, int slotIndex)
    {
        // Retire l'item actuel
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
        if (currentAura != null) currentAura.EnableAura();
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
