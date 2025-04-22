using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonPress : MonoBehaviour
{
    public Button ButtonSelf;
    void Start() {
        ButtonSelf.onClick.AddListener(OnClick);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void OnClick()
    {
        if (ButtonSelf.gameObject.CompareTag("ExitButton")) {
            Application.Quit();
        }
        else if (ButtonSelf.gameObject.CompareTag("LevelChangerButton")) {
            Cursor.lockState = CursorLockMode.Locked;
            if (ButtonSelf.gameObject.name == "LevelButton (1)") {
                SceneManager.LoadScene("Level1");
            }
            else if (ButtonSelf.gameObject.name == "LevelButton (2)") {
                SceneManager.LoadScene("Level2");
            }
        }
        else if (ButtonSelf.gameObject.CompareTag("SettingsButton")) {

        }
    }
}
