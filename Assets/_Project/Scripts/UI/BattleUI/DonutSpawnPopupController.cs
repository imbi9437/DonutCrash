using _Project.Scripts.EventStructs;
using _Project.Scripts.InGame;
using System.Collections.Generic;
using UnityEngine;

using IGS = _Project.Scripts.EventStructs.InGameStructs;
using AS = _Project.Scripts.EventStructs.AudioStruct;

public class DonutSpawnPopupController : MonoBehaviour
{
    public DonutSelectUI selectUIPrefab;

    private void Start()
    {
        EventHub.Instance?.RegisterEvent<IGS.RequestSelectDonutSpawned>(OnRequestPopupSelection);
        EventHub.Instance?.RegisterEvent<IGS.CompleteSelectDonut>(OnCompleteSelectDonut);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<IGS.RequestSelectDonutSpawned>(OnRequestPopupSelection);
        EventHub.Instance?.UnRegisterEvent<IGS.CompleteSelectDonut>(OnCompleteSelectDonut);
    }

    private void OnRequestPopupSelection(IGS.RequestSelectDonutSpawned evt) => PopupSelection(evt.selector, evt.donuts);
    private void OnCompleteSelectDonut(IGS.CompleteSelectDonut evt) => CloseSelection();
    
    private void PopupSelection(InGameOwner selector, List<DonutInstanceData> donutsData)
    {
        foreach (DonutInstanceData i in donutsData)
        {
            DonutSelectUI ui = Instantiate(selectUIPrefab, transform);
            ui.Setup(selector, i);
        }
        
        EventHub.Instance.RaiseEvent(new AS.PlayUiAudioEvent(AudioType.SFX_UI_08, 1f));
    }

    private void CloseSelection()
    {
        EventHub.Instance.RaiseEvent(new AS.PlayUiAudioEvent(AudioType.SFX_UI_09, 1f));
    }
}
