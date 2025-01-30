using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScripting : MonoBehaviour
{
    public Slider XSensitivitySlider;
    public Slider YSensitivitySlider;
    public Toggle YInvertToggle;
    private string FilePath;
    void Start()
    {
        FilePath = Path.Combine(Application.streamingAssetsPath + "/PlayerSettings.csv");
        if (File.Exists(FilePath)) {
            string FileDataPlain = File.ReadAllText(FilePath);
            string[] FileDataList = FileDataPlain.Split(",");
            YSensitivitySlider.value = int.Parse(FileDataList[1]);
            XSensitivitySlider.value = int.Parse(FileDataList[3]);
            YInvertToggle.isOn = bool.Parse(FileDataList[5]);
        }
        else {
            Debug.Log(FilePath);
        }
        YSensitivitySlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); }); 
        XSensitivitySlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); }); 
        YInvertToggle.onValueChanged.AddListener(delegate{ OnToggleValueChanged(); });
    }

    void OnSliderValueChanged() {
        File.WriteAllText(FilePath, "Y-Sensitivity," + YSensitivitySlider.value + ",\nX-Sensitivity," + XSensitivitySlider.value +",\nY Invert," + YInvertToggle.isOn);
    }
    void OnToggleValueChanged() {
        File.WriteAllText(FilePath, "Y-Sensitivity," + YSensitivitySlider.value + ",\nX-Sensitivity," + XSensitivitySlider.value +",\nY Invert," + YInvertToggle.isOn);
    }
}
