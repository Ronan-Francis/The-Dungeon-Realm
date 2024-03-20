using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance;

    public Image[] inventorySlots; // Assign these in the editor
    public Sprite emptySprite; // Assign an empty/default sprite in the editor
    public GameObject selectionBorderObj; // Assign a UI Image to act as the selection border in the editor
    [SerializeField] private InputActionReference scrollActionReference; // Assign this in the editor

    private int selectedItemIndex = 0; // Index of the currently selected item

    private Image selectionBorder; 
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
    }

    private void OnEnable()
    {
        // Subscribe to the scroll action event
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
        foreach (Image slot in inventorySlots)
        {
            if (slot.sprite == emptySprite)
            {
                slot.sprite = itemSprite;
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
        UpdateSelectionBorder();
    }

    private void SelectPreviousItem()
    {
        selectedItemIndex = selectedItemIndex - 1 < 0 ? inventorySlots.Length - 1 : selectedItemIndex - 1;
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

        // Enable or disable the selection border based on whether the selected slot is empty
        selectionBorder.enabled = inventorySlots[selectedItemIndex].sprite != emptySprite;
    }
}
