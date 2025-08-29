using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mouse : MonoBehaviour
{
    public GameObject mouseItemUi;
    public UnityEngine.UI.Image mouseCursor;
    public ItemSlotInfo itemSlotInfo;
    public UnityEngine.UI.Image itemImage;
    public TextMeshProUGUI stacksText;

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            mouseCursor.enabled = false;
            mouseItemUi.SetActive(false);
        }
        else
        {
            mouseCursor.enabled = true;
            if (itemSlotInfo.item != null)
            {
                mouseItemUi.SetActive(true);
            }
            else
            {
                mouseItemUi.SetActive(false);
            }
        }
    }

    public void SetUi()
    {
        // stacksText.text = " " + itemSlotInfo.stacks;
        // itemImage.sprite = itemSlotInfo.item.GiveItemImage();
       if (itemSlotInfo != null && itemSlotInfo.item != null)
    {
        stacksText.text = " " + itemSlotInfo.stacks;
        itemImage.sprite = itemSlotInfo.item.GiveItemImage();
        itemImage.gameObject.SetActive(true);
        stacksText.gameObject.SetActive(true);
    }
    else
    {
        // Empty slot visuals
        stacksText.text = "";
        itemImage.sprite = null;
        itemImage.gameObject.SetActive(false);
        stacksText.gameObject.SetActive(false);
    }
    }
    public void EmptySlot()
    {
        itemSlotInfo = new ItemSlotInfo(null, 0);
    }
}
