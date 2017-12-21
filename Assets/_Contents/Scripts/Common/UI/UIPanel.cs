using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel<T> : MonoBehaviour where T: Component  {

    static GameObject _ins = null;
    static GameObject ins {
        get {
            if (_ins == null) {
                var t = Resources.FindObjectsOfTypeAll<T>();
                _ins = t[0].gameObject;
            }
            return _ins;
        }
    }


    public static void Show() {
        ins.SetActive(true);
    }

    public static void Hide() {
        ins.SetActive(false);
    }

    public static T Get() {
        if (_ins) {
            return _ins.GetComponent<T>();
        }
        return null;
    }
}
