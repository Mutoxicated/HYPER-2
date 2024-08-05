using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour, ISettings {
    [SerializeField] private Button button;
    [SerializeField] private BloomSettingsGUI bloom;
    [SerializeField] private VignetteSettingsGUI vignette;
    [SerializeField] private ScanlineSettingsGUI scanline;
    [SerializeField] private ChromaAbbSettingsGUI chromaAbb;

    public void DStart() {
        button.Start();
        button.SetTextColor(Color.grey);
        bloom.Init();
        vignette.Init();
        scanline.Init();
        chromaAbb.Init();
    }

    public Doer Enable() {
        button.SetTextColor(Color.white);
        gameObject.SetActive(true);
        return () => {
            gameObject.SetActive(false);
            button.SetTextColor(Color.grey);
        };;
    }

    public void Disable() {
        gameObject.SetActive(false);
        button.Disable();
    }

    public void SaveData() {
        Save.pData.SetBloomSettings(bloom.Into());
        Save.pData.SetVignetteSettings(vignette.Into());
        Save.pData.SetScalineSettings(scanline.Into());
        Save.pData.SetChromaAbb(chromaAbb.Into());
    }
}