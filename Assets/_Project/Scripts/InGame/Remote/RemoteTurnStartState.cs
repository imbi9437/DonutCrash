using _Project.Scripts.EventStructs;
using UnityEngine;
using UnityEngine.InputSystem;

using PS = _Project.Scripts.EventStructs.PhotonStructs;
using IS = _Project.Scripts.EventStructs.InputStructs;
using AS = _Project.Scripts.EventStructs.AudioStruct;
using IGS = _Project.Scripts.EventStructs.InGameStructs;

namespace _Project.Scripts.InGame.Remote
{
    public class RemoteTurnStartState : MonoState
    {
        public override int index => (int)BattleState.TurnStart;
    
        private string _selectedUid;
        private Vector2 _point;
        private Vector2 _startPosition;
        private Vector2 _endPosition;
        private DonutObject _clickedDonut;
        private bool _isInteractor;

        private GuidelineRender _guidelineRender;
        private SplineController _splineController;
        private float _donutRadius = 1.2f;
    
        private RemoteBattleController _remoteBattleController;

        [SerializeField] private float maxDragableDistance = 30f;

        public void SetupVisualizers(GuidelineRender guidelineRenderer, SplineController splineController)
        {
            _guidelineRender = guidelineRenderer;
            _splineController = splineController;
        }
        public override void Initialize(MonoStateMachine machine)
        {
            base.Initialize(machine);
            _remoteBattleController = machine as RemoteBattleController;

            _remoteBattleController?.SetupVisualizers(_guidelineRender, _splineController);
        }

        protected override void OnEnable()
        {
            InGameOwner currentTurnOwner = _remoteBattleController.GetTurnOwner();
            string currentNickname = _remoteBattleController.GetNickname(currentTurnOwner);
            
            EventHub.Instance?.RaiseEvent(new IGS.ShowTurnStartPopup(currentTurnOwner, currentNickname));
            
            if (_guidelineRender == null && _remoteBattleController != null)
            {
                SetupVisualizers(_remoteBattleController.GuidelineRenderer, _remoteBattleController.SplineController);
            }

            _isInteractor = InGameOwner.Right == _remoteBattleController.TurnOwner;
            if (_isInteractor)
            {
                Debug.Log($"[RemoteTurnStartState] 게스트 차례. 입력 활성화.");
                EventHub.Instance?.RegisterEvent<IS.Point>(OnPoint);
                EventHub.Instance?.RegisterEvent<IS.Click>(OnTouch);
            }
        }

        protected override void OnDisable()
        {
            EventHub.Instance?.UnRegisterEvent<IS.Point>(OnPoint);
            EventHub.Instance?.UnRegisterEvent<IS.Click>(OnTouch);
            
            _guidelineRender?.HideGuideline();
            _splineController?.gameObject.SetActive(false);
        }

        private void OnTouch(IS.Click evt) => Touch(evt.context);
        private void OnPoint(IS.Point evt) => Point(evt.context);
        
        private void Touch(InputAction.CallbackContext context)
        {
            if(!_isInteractor)
                return;

            float value = context.ReadValue<float>();
            if (value > .5f)
                OnClickDown(_point);
            else
                OnClickUp(_point);
        }

        private void Point(InputAction.CallbackContext context)
        {
            if(!_isInteractor)
                return;
            Vector2 value = context.ReadValue<Vector2>();
            _point = value;
            OnClick(_point);
        }
    
        private void OnClickDown(Vector2 position)
        {
            Ray ray = Camera.main?.ScreenPointToRay(position) ?? new Ray();

            if (!Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Donut")))
                return;

            if (!hit.transform.TryGetComponent(out DonutObject donut))
                return;

            if (donut.Owner != _remoteBattleController.GetTurnOwner())
                return;

            if (!donut.Interactable)
                return;

            _clickedDonut = donut;

            if (donut.TryGetComponent(out CapsuleCollider donutCollider))
            {
                _donutRadius = donutCollider.radius * donut.transform.localScale.x;
            }
            else
            {
                _donutRadius = 1.2f;
            }

            _selectedUid = donut.GetUid();
            _startPosition = new (donut.transform.position.x, donut.transform.position.z);

            EventHub.Instance.RaiseEvent(new AS.PlaySfxAudioEvent(AudioType.SFX_Fight_06, 1f));
        }

        private void OnClick(Vector2 position)
        {
            if (!_clickedDonut) return;
            
            Vector2 prevVector = _startPosition - _endPosition;
            _endPosition = GetCanvasToWorldPoint(position);
            Vector2 rawDragVector = _startPosition - _endPosition;
            
            _clickedDonut.SetDragging(rawDragVector);

            if (rawDragVector.magnitude >= 2f && prevVector.magnitude < 2f)
                EventHub.Instance.RaiseEvent(new AS.PlaySfxAudioEvent(AudioType.SFX_Fight_01, 1f));

            if (rawDragVector.magnitude >= 2f)
            {
                if (_guidelineRender != null)
                {
                    Vector3 worldStartDirection = ConvertScreenToWorldDirection(rawDragVector);

                    float maxDistance = Mathf.Min(rawDragVector.magnitude * 15f, 15f);

                    _guidelineRender.DrawGuideline(_clickedDonut.transform.position, worldStartDirection.normalized, maxDistance);
                }
                else
                    _guidelineRender?.HideGuideline();
                
                if (_splineController != null)
                {
                    Vector3 donutPos = _clickedDonut.transform.position;

                    Vector3 mainFoci = new (donutPos.x, 0, donutPos.z);
                    Vector3 force = new (rawDragVector.x, 0, rawDragVector.y);
                    force = Vector3.ClampMagnitude(force, 8f);

                    _splineController.gameObject.SetActive(true);
                    _splineController.SetEllipse(mainFoci, mainFoci + force);
                }
                else
                    _splineController?.gameObject.SetActive(false);
            }
        }

        private void OnClickUp(Vector2 position)
        {
            if (_selectedUid == null)
                return;
            
            _endPosition = GetCanvasToWorldPoint(position);
            Vector2 shotForce = _startPosition - _endPosition;

            if (_clickedDonut)
                _clickedDonut.SetDragging(Vector2.zero);

            _guidelineRender?.HideGuideline();
            _splineController?.gameObject.SetActive(false);


            if (shotForce.magnitude < 2f)
            {
                EventHub.Instance?.RaiseEvent(new IGS.DragCancelledEvent(
                        _selectedUid, 
                        _remoteBattleController.GetTurnOwner()
                    ));
                _selectedUid = null;
                _clickedDonut = null;
                return;
            }
            
            shotForce = Vector3.ClampMagnitude(shotForce, 8f);
            EventHub.Instance?.RaiseEvent(new PS.ShotDonut(_selectedUid, shotForce));
            _selectedUid = null;
            _clickedDonut = null;
            _isInteractor = false;
        }

        private Vector2 GetCanvasToWorldPoint(Vector2 position)
        {
            Plane plane = new (Vector3.up, Vector3.zero);
            Ray ray = Camera.main?.ScreenPointToRay(position) ?? new Ray();
            if (plane.Raycast(ray, out float enter) == false)
                return Vector2.zero;
            
            Vector3 point = ray.GetPoint(enter);
            return new Vector2(point.x, point.z);
        }
        
        private Vector3 ConvertScreenToWorldDirection(Vector2 screenDragVector)
        {
            Vector3 worldDirection = new Vector3(screenDragVector.x, 0f, screenDragVector.y);
            return worldDirection;
        }
    }
}
