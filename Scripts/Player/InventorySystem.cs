using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance;

    public Image[] inventorySlots; // UI slots for items
    public Sprite emptySprite; // Default sprite for an empty slot
    public GameObject selectionBorderObj; // UI Image for selection border
    [SerializeField] private InputActionReference scrollActionReference; // Scroll input action reference
    public GameObject[] itemObjects; // Corresponding item objects to appear next to the player
    public Transform playerTransform; // Player's transform to position items next to
    public Vector3 itemOffset = new Vector3(1f, 0f, 0f); // Offset from the player for the item's position
    public float itemScale = 0.5f; // Scale for the item when shown

    private int selectedItemIndex = 0; // Currently selected item index
    private Image selectionBorder;
    public GameObject nullItemObject;

    private void Awake()
    {
        selectionBorder = selectionBorderObj.GetComponent<Image>();
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitializeInventorySlots();
        InitializeItemObjectsArray();
        HideAllItemObjects();
        // Check if inventorySlots and itemObjects lengths match
        if (inventorySlots.Length != itemObjects.Length)
        {
            Debug.Log("Mismatch between inventory slots and item objects lengths.");
        }
    }

    private void Update()
    {
        UpdateItemObjectsFromChildren();
    }

    private void UpdateItemObjectsFromChildren()
    {
        // Check if the player has any children
        if (playerTransform.childCount != itemObjects.Length)
        {
            // Resize the itemObjects array to match the number of children
            itemObjects = new GameObject[playerTransform.childCount];

            // Populate itemObjects with player's children
            for (int i = 0; i < playerTransform.childCount; i++)
            {
                itemObjects[i] = playerTransform.GetChild(i).gameObject;
            }

            // Since the inventory items have changed, update the UI accordingly
            //UpdateSelectionBorder();
            // Optionally, call other methods if needed to reflect changes in the UI or logic
        }
    }

    public void InitializeItemObjectsArray()
    {
        // Ensure the itemObjects array has the same length as inventorySlots
        itemObjects = new GameObject[inventorySlots.Length];

        // Populate the array with nulls
        for (int i = 0; i < itemObjects.Length; i++)
        {
            itemObjects[i] = nullItemObject;
        }
    }

    private void OnEnable()
    {
        scrollActionReference.action.Enable();
        scrollActionReference.action.performed += OnScrollPerformed;
    }

    private void OnDisable()
    {
        scrollActionReference.action.Disable();
        scrollActionReference.action.performed -= OnScrollPerformed;
    }

    private void InitializeInventorySlots()
    {
        foreach (Image slot in inventorySlots)
        {
            slot.sprite = emptySprite;
        }
        UpdateSelectionBorder();
    }

    public void AddItem(Sprite itemSprite)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].sprite == emptySprite)
            {
                inventorySlots[i].sprite = itemSprite;
                break;
            }
        }
    }

    private void OnScrollPerformed(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<float>();
        if (scrollValue > 0)
        {
            SelectNextItem();
        }
        else if (scrollValue < 0)
        {
            SelectPreviousItem();
        }
    }

    private void SelectNextItem()
    {
        selectedItemIndex = (selectedItemIndex + 1) % inventorySlots.Length;
        selectedItemIndex = Mathf.Clamp(selectedItemIndex, 0, itemObjects.Length - 1); // Ensure within bounds
        UpdateSelectionBorder();
    }

    private void SelectPreviousItem()
    {
        selectedItemIndex = (selectedItemIndex - 1 + inventorySlots.Length) % inventorySlots.Length;
        selectedItemIndex = Mathf.Clamp(selectedItemIndex, 0, itemObjects.Length - 1); // Ensure within bounds
        UpdateSelectionBorder();
    }


    private void UpdateSelectionBorder()
    {
        if (inventorySlots.Length == 0) return;

        // Assuming inventorySlots are UI Images with RectTransform components
        RectTransform selectedSlotRectTransform = inventorySlots[selectedItemIndex].GetComponent<RectTransform>();
        RectTransform selectionBorderRectTransform = selectionBorder.GetComponent<RectTransform>();

        // Set the selection border position to match the selected item slot
        selectionBorderRectTransform.anchoredPosition = selectedSlotRectTransform.anchoredPosition;

        // Optionally, match the size as well if needed
        selectionBorderRectTransform.sizeDelta = selectedSlotRectTransform.sizeDelta;
        selectionBorder.enabled = inventorySlots[selectedItemIndex].sprite != emptySprite;

        UpdateSelectedItemObject();
    }

    private void UpdateSelectedItemObject()
    {
        HideAllItemObjects();
        if (selectedItemIndex >= 0 && selectedItemIndex < itemObjects.Length && inventorySlots[selectedItemIndex].sprite != emptySprite)
        {
            GameObject selectedItemObject = itemObjects[selectedItemIndex];
            selectedItemObject.SetActive(true);
            selectedItemObject.transform.position = playerTransform.position + itemOffset;
            selectedItemObject.transform.localScale = Vector3.one * itemScale;
        }
    }


    private void HideAllItemObjects()
    {
        foreach (var itemObject in itemObjects)
        {
            itemObject.SetActive(false);
        }
    }
}
