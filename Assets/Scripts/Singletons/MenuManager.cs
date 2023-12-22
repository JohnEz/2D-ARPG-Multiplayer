using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager> {

    private enum MenuType {
        MAIN_MENU,
        OPTIONS_MENU,
        NONE
    }

    [SerializeField]
    private GameObject _mainMenu;

    [SerializeField]
    private GameObject _optionsMenu;

    private MenuType _currentMenu;

    private void Start() {
        _currentMenu = MenuType.NONE;
        _mainMenu.SetActive(false);
        _optionsMenu.SetActive(false);
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
            OpenMenu(MenuType.MAIN_MENU);
        }
    }

    private void MainMenuUpdate() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            CloseCurrentMenu();
            OpenMenu(MenuType.NONE);
        }
    }

    private void OptionsMenuUpdate() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            CloseCurrentMenu();
            OpenMenu(MenuType.MAIN_MENU);
        }
    }

    public void OpenMainMenu() {
        _mainMenu.SetActive(true);
        _currentMenu = MenuType.MAIN_MENU;
    }

    private void OpenMenu(MenuType menuToOpen) {
        switch (menuToOpen) {
            case MenuType.MAIN_MENU:
            OpenMainMenu();
            break;

            case MenuType.OPTIONS_MENU:
            OpenOptionsMenu();
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
            CloseMainMenu();
            break;

            case MenuType.OPTIONS_MENU:
            CloseOptionsMenu();
            break;

            case MenuType.NONE:
            default:
            break;
        }
        _currentMenu = MenuType.NONE;
    }

    public void CloseMainMenu() {
        _mainMenu.SetActive(false);
    }

    public void OpenOptionsMenu() {
        _optionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu() {
        _optionsMenu.SetActive(false);
    }

    public bool IsAMenuOpen() {
        return _currentMenu != MenuType.NONE;
    }
}