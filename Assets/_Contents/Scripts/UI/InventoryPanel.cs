using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;



public class InventoryPanel : UIPanel<InventoryPanel> {

    public Transform barkpackContent;

    public ItemData[] barkpackItems;

    public Transform itemTemplate;

    public Image[] equipSolt;

    public void RefreshEquipInventory() {
        var inventory = GameController.Instance.player.inventory;

        for(int i = 0; i < inventory.Count; i++) {
            var item = DataTable.Get<ItemData>(inventory[i].id);
            var atlas = Resources.Load<SpriteAtlas>("Icon/" + item.atlas);
            var sprite = atlas.GetSprite(item.icon);
            equipSolt[i].sprite = sprite;
        }
    }

    void Start () {
        Hide();
        itemTemplate.gameObject.SetActive(false);
        barkpackItems = new ItemData[2];
        barkpackItems[0] = DataTable.Get<ItemData>(10001);
        barkpackItems[1] = DataTable.Get<ItemData>(10002);
        
        foreach (var data in barkpackItems) {
            var item = Instantiate(itemTemplate);
            item.GetComponentInChildren<Text>().text = data.name;
            var atlas = Resources.Load<SpriteAtlas>("Icon/" + data.atlas);
            item.GetChild(0).GetComponent<Image>().sprite = atlas.GetSprite(data.icon);
            item.transform.SetParent(barkpackContent);
            item.gameObject.SetActive(true);
        }

    }

}
