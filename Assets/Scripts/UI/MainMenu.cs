using FishNet.Managing.Scened;
using FishNet;

public class MainMenu : Menu {

    public void OpenOptions() {
        MenuManager.Instance.OpenMenu("OPTIONS");
    }

    public void OpenKeybinds() {
        MenuManager.Instance.OpenMenu("KEYBINDS");
    }

    public void ExitScenario() {
        NetworkSceneLoader.Instance.LoadScene("Town");
    }
}