using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonPress : MonoBehaviour
{
    public Button ButtonSelf;
    public Button ShotgunButton;
    public Button PistolButton;
    void Start()
    {
        ButtonSelf.onClick.AddListener(OnClick);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SetButtonInteractivity()
    {
        if (WeaponController.Weapon.GetWeaponKey() == "shotgun")
        {
            ShotgunButton.interactable = false;
            PistolButton.interactable = true;
        }
        else if (WeaponController.Weapon.GetWeaponKey() == "pistol")
        {
            PistolButton.interactable = false;
            ShotgunButton.interactable = true;
        }
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
            if (ButtonSelf.gameObject.name == "ShotgunButton")
            {
                WeaponController.LoadFromWeaponKey("shotgun");
                SetButtonInteractivity();
            }
            else if (ButtonSelf.gameObject.name == "PistolButton")
            {
                WeaponController.LoadFromWeaponKey("pistol");
                SetButtonInteractivity();
            }
        }
    }
}
