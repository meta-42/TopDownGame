using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;



public class InventoryPanel : UIPanel<InventoryPanel> {

    public Transform aroundContent;

    public Transform itemTemplate;

    public Image[] inventorySolt;

    public List<Transform> aroundItems = new List<Transform>();


    public void RefreshEquipInventory() {
        var inventory = GameController.Instance.player.inventory;

        for(int i = 0; i < inventory.Count; i++) {
            var item = DataTable.Get<ItemData>(inventory[i].id);
            var atlas = Resources.Load<SpriteAtlas>("Icon/" + item.atlas);
            var sprite = atlas.GetSprite(item.icon);
            inventorySolt[i].sprite = sprite;
        }
    }

    void Start () {
        Hide();
        itemTemplate.gameObject.SetActive(false);
    }

    private void Update() {
        for (int i = 0; i < aroundContent.childCount; i++) {
            Destroy(aroundContent.GetChild(i).gameObject);
        }
        var player = GameController.Instance.player;
        var rets = Physics.OverlapSphere(player.transform.position, 2);

        foreach(var ret in rets) {
            var weapon = ret.GetComponent<Weapon>();
            if (weapon && !weapon.isInInventory) {
                var item = Instantiate(itemTemplate);

                item.GetComponentInChildren<Text>().text = weapon.data.name;
                var atlas = Resources.Load<SpriteAtlas>("Icon/" + weapon.data.atlas);
                item.GetChild(0).GetComponent<Image>().sprite = atlas.GetSprite(weapon.data.icon);
                item.transform.SetParent(aroundContent);
                item.gameObject.SetActive(true);
            }
        }
    }

}
