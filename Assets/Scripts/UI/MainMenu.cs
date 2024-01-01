using FishNet.Managing.Scened;
using FishNet;
using FishNet.Managing;

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

    public void ExitGame() {
        NetworkSceneLoader.Instance.LoadScene("MainMenu");
        InstanceFinder.ServerManager.StopConnection(true);
        InstanceFinder.ClientManager.StopConnection();
    }
}