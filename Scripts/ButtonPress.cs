using UnityEngine;
using UnityEngine.UI;

public class ButtonPress : MonoBehaviour
{
    public Button ExitButton;
    void Start() {
        ExitButton.onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        Application.Quit();
    }
}
