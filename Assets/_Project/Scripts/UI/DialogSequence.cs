using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;
using US = _Project.Scripts.EventStructs.UIStructs;

namespace _Project.Scripts.UI
{
    public class DialogSequence
    {
        private DialogFactor[] _factors;
        private Transform _parent;
        private bool _isEndDialog;
        private DialogPanel _currentDialog;
        
        private Action _skipAction;
        private Action _endAction;
    
        private CancellationTokenSource _cts;

        private DialogSequence() { }
        public DialogSequence(Transform parent, DialogFactor[] factors)
        {
            _parent = parent;
            _factors = factors;
        }

        public void StartSequence()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            _skipAction = null;
            _endAction = null;
            
            StartSequenceAsync(_cts.Token).Forget();
        }

        public void CancelSequence()
        {
            _cts?.Cancel();
            if (_currentDialog)
                Object.Destroy(_currentDialog.gameObject);
            
            UnRegisterEvent();
        }

        private void OnSkipDialog(US.SkipDialogEvent evt)
        {
            _skipAction?.Invoke();
        }
        private void OnEndDialog(US.EndDialogEvent evt)
        {
            _endAction?.Invoke();
            _isEndDialog = true;
        }
    
        private async UniTaskVoid StartSequenceAsync(CancellationToken ct)
        {
            foreach (DialogFactor i in _factors)
            {
                if (_currentDialog)
                    Object.Destroy(_currentDialog.gameObject);
                
                _currentDialog = Object.Instantiate(i.dialogPrefab, _parent);
                _currentDialog.Initialize(i.iconSprite, i.iconStr, i.contentStr, i.typeDelay, i.endAction);
                _skipAction += _currentDialog.SkipStreamText;
                _endAction += _currentDialog.EndDialog;
                RegisterEvent();
                _isEndDialog = false;
                await UniTask.WaitUntil(() => _isEndDialog, cancellationToken: ct);
                UnRegisterEvent();
                _skipAction -= _currentDialog.SkipStreamText;
                _endAction -= _currentDialog.EndDialog;
            }
        }

        private void RegisterEvent()
        {
            EventHub.Instance.RegisterEvent<US.SkipDialogEvent>(OnSkipDialog);
            EventHub.Instance.RegisterEvent<US.EndDialogEvent>(OnEndDialog);
        }

        private void UnRegisterEvent()
        {
            EventHub.Instance?.UnRegisterEvent<US.SkipDialogEvent>(OnSkipDialog);
            EventHub.Instance?.UnRegisterEvent<US.EndDialogEvent>(OnEndDialog);
        }
    }
}
