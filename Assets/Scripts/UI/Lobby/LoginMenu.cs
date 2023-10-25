using TMPro;
using UnityEngine;

public class LoginMenu : MonoBehaviour {

    [SerializeField]
    private TMP_InputField usernameInput;

    public void Login() {
        LobbyManager.Instance.Authenticate(usernameInput.text);
    }
}