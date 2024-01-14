using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Panel))]
public class Menu : MonoBehaviour {

    [SerializeField]
    public string ID;

    [SerializeField]
    private string _previousMenuId; // Menu to open when this one closes

    public virtual void Start() {
        if (!MenuManager.Instance) {
            return;
        }

        MenuData menuData = new MenuData();
        menuData.ID = ID;
        menuData.Panel = GetComponent<Panel>();
        menuData.PreviousMenuId = _previousMenuId;

        MenuManager.Instance.RegisterMenu(menuData);
    }

    public virtual void OnDestroy() {
        if (!MenuManager.Instance) {
            return;
        }

        MenuManager.Instance.UnregisterMenu(ID);
    }

    public void Close() {
        MenuManager.Instance.CloseMenu(ID);
    }
}