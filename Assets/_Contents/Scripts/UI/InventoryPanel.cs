using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;



public class InventoryPanel : MonoBehaviour {

    public Transform barkpackContent;

    public ItemData[] barkpackItems;

    public Transform itemTemplate;


    void Start () {

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
