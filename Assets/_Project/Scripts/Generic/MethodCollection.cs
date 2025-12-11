using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using AS = _Project.Scripts.EventStructs.AudioStruct;

namespace _Project.Scripts.Generic
{
    public static class MethodCollection
    {
        public static void AddListenerToAllButton()
        {
            List<Button> buttons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
            buttons.ForEach(x =>
            {
                x.onClick.RemoveListener(InvokeUISound);
                x.onClick.AddListener(InvokeUISound);
            });
            return;

            void InvokeUISound()
            {
                EventHub.Instance?.RaiseEvent(new AS.PlayUiAudioEvent(AudioType.UISample, 1f));
            }
        }
    }
}
