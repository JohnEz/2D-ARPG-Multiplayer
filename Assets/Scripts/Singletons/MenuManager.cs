using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum MenuType {
    MAIN_MENU,
    OPTIONS_MENU,
    SCENARIO_SELECT,
    BLACKSMITH,
    NONE
}

public class MenuManager : Singleton<MenuManager> {

    [SerializeField]
    private Panel _mainMenu;

    [SerializeField]
    private Panel _optionsMenu;

    private MenuType _currentMenu;

    private void Start() {
        _currentMenu = MenuType.NONE;
        _mainMenu.SetPanelEnabled(false, false);
        _optionsMenu.SetPanelEnabled(false, false);
    }

    private void Update() {
        switch (_currentMenu) {
            case MenuType.MAIN_MENU:
            MainMenuUpdate();
            break;

            case MenuType.OPTIONS_MENU:
            OptionsMenuUpdate();
            break;

            case MenuType.NONE:
            default:
            EmptyUpdate();
            break;
        }
    }

    private void EmptyUpdate() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            OpenMainMenu();
        }
    }

    private void MainMenuUpdate() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            CloseMainMenu();
        }
    }

    private void OptionsMenuUpdate() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            CloseOptionsMenu();
        }
    }

    public void OpenMenu(MenuType menuToOpen) {
        CloseCurrentMenu();

        switch (menuToOpen) {
            case MenuType.MAIN_MENU:
            DisplayMainMenu();
            break;

            case MenuType.OPTIONS_MENU:
            DisplayOptionsMenu();
            break;

            case MenuType.NONE:
            default:
            break;
        }

        _currentMenu = menuToOpen;
    }

    private void CloseCurrentMenu() {
        switch (_currentMenu) {
            case MenuType.MAIN_MENU:
            HideMainMenu();
            break;

            case MenuType.OPTIONS_MENU:
            HideOptionsMenu();
            break;

            case MenuType.NONE:
            default:
            break;
        }
        _currentMenu = MenuType.NONE;
    }

    private void DisplayMainMenu() {
        _mainMenu.SetPanelEnabled(true);
    }

    private void HideMainMenu() {
        _mainMenu.SetPanelEnabled(false);
    }

    private void DisplayOptionsMenu() {
        _optionsMenu.SetPanelEnabled(true);
    }

    private void HideOptionsMenu() {
        _optionsMenu.SetPanelEnabled(false);
    }

    // Functions for buttons to call

    public void OpenMainMenu() {
        OpenMenu(MenuType.MAIN_MENU);
    }

    public void CloseMainMenu() {
        OpenMenu(MenuType.NONE);
    }

    public void OpenOptionsMenu() {
        OpenMenu(MenuType.OPTIONS_MENU);
    }

    public void CloseOptionsMenu() {
        OpenMenu(MenuType.MAIN_MENU);
    }

    public bool IsBlockingMenuOpen() {
        return _currentMenu != MenuType.NONE;
    }
}