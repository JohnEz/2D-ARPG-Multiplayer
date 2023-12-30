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
        SceneLoadData data = new SceneLoadData("Town");
        data.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(data);
    }
}