using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct MenuData {
    public string ID;
    public Panel Panel;
    public string PreviousMenuId; // store the actual previous menu?
}

public class MenuManager : Singleton<MenuManager> {
    private Dictionary<string, MenuData> _menus;

    private string _currentMenuId;

    private void Awake() {
        _menus = new Dictionary<string, MenuData>();
        _currentMenuId = string.Empty;
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            if (_currentMenuId.Equals(string.Empty)) {
                OpenMenu("GAME_MENU");
            } else {
                CloseCurrentMenu();
            }
        }
    }

    public bool RegisterMenu(MenuData menuData) {
        if (_menus.ContainsKey(menuData.ID)) {
            Debug.Log($"Tried to register the menu {menuData.ID} but it there is already a menu with that id registered.");
            return false;
        }

        menuData.Panel.SetPanelEnabled(false, false);
        _menus.Add(menuData.ID, menuData);

        return true;
    }

    public void CloseCurrentMenu() {
        if (_currentMenuId == string.Empty) {
            return;
        }

        CloseMenu(_currentMenuId);
    }

    public void CloseMenu(string menuId) {
        if (menuId == string.Empty) {
            return;
        }

        if (!_menus.ContainsKey(menuId)) {
            Debug.LogError($"Tried to close the menu {menuId} that doesnt exist");
            return;
        }

        MenuData menuToClose = _menus[menuId];

        menuToClose.Panel.SetPanelEnabled(false);

        _currentMenuId = string.Empty;

        if (!menuToClose.PreviousMenuId.Equals(string.Empty)) {
            OpenMenu(menuToClose.PreviousMenuId);
        }
    }

    public void OpenMenu(string menuId) {
        CloseCurrentMenu();

        if (!_menus.ContainsKey(menuId)) {
            Debug.LogError($"Tried to open the menu {menuId} that doesnt exist");
            return;
        }

        MenuData menuToOpen = _menus[menuId];

        menuToOpen.Panel.SetPanelEnabled(true);

        _currentMenuId = menuId;
    }

    public bool IsBlockingMenuOpen() {
        return _currentMenuId != string.Empty;
    }
}