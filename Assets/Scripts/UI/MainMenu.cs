using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FishNet.Managing.Logging;

public class MainMenu : Singleton<MainMenu> {

    [Server(Logging = LoggingType.Off)]
    public void ExitScenario() {
        SceneLoadData data = new SceneLoadData("Town");
        data.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(data);
    }
}