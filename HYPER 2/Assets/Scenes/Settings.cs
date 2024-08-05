

using UnityEngine;

public class Settings : MonoBehaviour {
    [SerializeField] private PlayerSettings ps;
    [SerializeField] private GraphicsSettings gs;
    private Doer onExit;
    private Doer onAfterSave;

    private ISettings currentlySelected;
    private Doer onSettingsDisable;

    public void DStart(Doer onExit, Doer onAfterSave) {
        this.onAfterSave = onAfterSave;
        this.onExit = onExit;
        gs.DStart();
        currentlySelected = ps;
        ps.DStart();
        onSettingsDisable = ps.Enable();
    }

    public void OpenPlayerSettings() {
        if ((Object)currentlySelected == ps) { return; }
        onSettingsDisable();
        currentlySelected = ps;
        onSettingsDisable = ps.Enable();
    }

    public void OpenGraphicsSettings() {
        if ((Object)currentlySelected == gs) { return; }
        onSettingsDisable();
        currentlySelected = gs;
        onSettingsDisable = gs.Enable();
    }


    public void Enable() {
        gameObject.SetActive(true);
    }

    public void Disable() {
        onExit.Invoke();
        gameObject.SetActive(false);
    }

    public void SaveData() {
        ps.SaveData();
        gs.SaveData();

        Save.UpdateJsonData(DataType.PLAYER);

        onAfterSave();
    }
}