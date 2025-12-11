using _Project.Scripts.BuffSystem;
using _Project.Scripts.EffectSystem;
using _Project.Scripts.EventStructs;
using _Project.Scripts.Interface;
using _Project.Scripts.SkillSystem;
using DG.Tweening;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using IGS = _Project.Scripts.EventStructs.InGameStructs;
using PS = _Project.Scripts.EventStructs.PhotonStructs;
using DNC = _Project.Scripts.EventStructs.DamageNumberStructs;
using STS = _Project.Scripts.EventStructs.SkillTextStructs;

namespace _Project.Scripts.InGame
{
    [RequireComponent(typeof(DonutMove), typeof(DonutHighlight))]
    public class DonutObject : MonoBehaviourPun, IBuffable, IPunObservable
    {
        // TODO : 샘플용 인디케이터 이미지
        [SerializeField] private Sprite indicator;
        
        // TODO : 아래의 두 상수는 GameSettings 로 이동할 데이터 입니다.
        private const float MaxSpeed = 5f;
        private const float MinSpeed = 1f;

        public InGameOwner Owner { get; private set; }

        private DonutState _state;
        private DonutInstanceData _donutInstanceData;
        private DonutInGameData _donutInGameData;
        private float _health;
        private Vector2 _shotForce;
        public bool Interactable { get; private set; }

        private bool _isDragging;

        private readonly List<SkillObject> _skillObjects = new List<SkillObject>();
        private readonly List<BuffBase> _buffObjects = new List<BuffBase>();

        private DonutMove _donutMove;
        private DonutHighlight _donutHighlight;
        private DonutStatusUI  _donutStatusUI;
        private Transform _donutOutfit;

        private void OnDisable()
        {
            EventHub.Instance?.UnRegisterEvent<IGS.TurnStartEvent>(OnTurnStart);
            EventHub.Instance?.UnRegisterEvent<IGS.ShotDonutEvent>(OnShotDonut);
            EventHub.Instance?.UnRegisterEvent<IGS.TurnRunningEvent>(OnTurnRunning);
            EventHub.Instance?.UnRegisterEvent<IGS.TurnEndEvent>(OnTurnEnd);
             EventHub.Instance?.UnRegisterEvent<IGS.DragCancelledEvent>(OnDragCancelledEvent);

            _buffObjects.ToList().ForEach(x => x.RemoveBuff());
            _buffObjects.Clear();
        }

        public void Initialize(DonutInstanceData donutInstanceData, InGameOwner owner, List<SkillObject> initSkills = null, List<BuffBase> initBuffs = null)
        {
            _donutMove = GetComponent<DonutMove>();
            _donutHighlight = GetComponent<DonutHighlight>();
            _donutStatusUI = GetComponentInChildren<DonutStatusUI>();
            
            DataManager.Instance.TryGetDonutData(donutInstanceData.origin, out DonutData donutData);
            GetComponent<Rigidbody>().mass = donutData.mass;
            
            _donutInstanceData = donutInstanceData;
            _donutInGameData = new DonutInGameData(donutInstanceData);

            AddressableLoader.AssetLoadByPath<GameObject>(donutData.resourcePath, x =>
            {
                _donutOutfit = Instantiate(x, transform).transform;
                _donutOutfit.localScale = Vector3.one;
                _donutOutfit.localPosition = Vector3.zero;
            }).Forget();

            Owner = owner;
            Interactable = true;

            _buffObjects.Clear();
            if (initBuffs != null)
                _buffObjects.AddRange(initBuffs);

            _skillObjects.Clear();
            donutData?.skillIds?.ForEach(x =>
            {
                if (DataManager.Instance.TryGetSkillData(x, out SkillData skillData) == false)
                {
                    Debug.LogError($"{donutData.uid}의 스킬 데이터 {x} 탐색 실패");
                    return;
                }
                if (SkillFactory.TryGetSkillObject(skillData, this, out SkillObject skillObject) == false)
                {
                    Debug.LogError($"{donutData.uid}의 스킬 {skillData.uid} 생성 실패");
                    return;
                }
                _skillObjects.Add(skillObject);
            });
            
            if (initSkills != null)
                _skillObjects.AddRange(initSkills);
            
            OnSpawn();
            
            OnChangeBuff();
            
            _health = _donutInGameData.hp;
            
            gameObject.name = $"{donutData.donutName} : {Owner.ToString()}";
            
            _donutStatusUI.Setup(_health, GetAtk());
            
            photonView.RPC("InitializeRPC", RpcTarget.Others, donutInstanceData.uid, donutInstanceData.origin, owner);
            
            this._donutInstanceData = donutInstanceData;
            this.Owner = owner;

            // TODO : 샘플용 인디케이터 생성. 제거
            GameObject go = new GameObject("Indicator");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(0, 2, 2);
            go.transform.forward = Vector3.down;
            go.layer = LayerMask.NameToLayer("UI");
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = indicator;
            sr.color = Owner == InGameOwner.Left ? Color.green : Color.red;
            
            TrailRenderer tr = GetComponentInChildren<TrailRenderer>();
            if (tr != null)
            {
                tr.material.color = Owner == InGameOwner.Left ? new Color(0.4549019f, 0.7529412f, 0.9882353f, 1) : new Color(1, 0.1333333f, 0.3803921f, 1f);
            }
            

            EventHub.Instance?.RegisterEvent<IGS.TurnStartEvent>(OnTurnStart);
            EventHub.Instance?.RegisterEvent<IGS.ShotDonutEvent>(OnShotDonut);
            EventHub.Instance?.RegisterEvent<IGS.TurnRunningEvent>(OnTurnRunning);
            EventHub.Instance?.RegisterEvent<IGS.TurnEndEvent>(OnTurnEnd);
            EventHub.Instance?.RegisterEvent<IGS.DragCancelledEvent>(OnDragCancelledEvent);        }

        public void AddBuff(BuffBase buff)
        {
            BuffBase sameBuff = _buffObjects
                .Find(x => x.IsSameBuff(buff));

            if (sameBuff == null)
                _buffObjects.Add(buff);
            else
                sameBuff.StackedUp(1);

            OnChangeBuff();
        }

        public void RemoveBuff(BuffBase buff)
        {
            if (_buffObjects.Contains(buff))
                _buffObjects.Remove(buff);
            
            OnChangeBuff();
        }

        public void TakeDamage(float damage, bool isCrit, IDamageable source)
        {
            OnDefense(source);
            float previousHealth = _health;
            float finalDamage = damage - _donutInGameData.def;
            finalDamage = finalDamage < 5f ? 5f : finalDamage;
            _health -= finalDamage;
            _health = _health > _donutInGameData.hp ? _donutInGameData.hp : _health;
            _donutStatusUI.Setup(_health, GetAtk());
            
            photonView.RPC("OnTakeDamageEffectRPC", RpcTarget.All, (int)finalDamage, isCrit);
            if (_health <= 0)
                EventHub.Instance?.RaiseEvent(new IGS.RemoveDonutEvent(this));
        }

        public void TakeHeal(float heal)
        {
            heal = heal < 0f ? 0f : heal;
            _health += heal;
            _health = _health > _donutInGameData.hp ? _donutInGameData.hp : _health;
            _donutStatusUI.Setup(_health, GetAtk());
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!photonView.IsMine)
                return;
            
            float speed = _donutMove.Speed;
            float modBySpd = Mathf.Clamp01((speed - MinSpeed) / (MaxSpeed - MinSpeed));
            // 충돌 대상이 피해를 입을 수 있는 대상인 경우
            if (_state == DonutState.Attack && other.gameObject.TryGetComponent(out IDamageable damageable))
            {
                Debug.Log($"소유자 : {Owner}, 피격자 : {damageable.Owner}");
                if (Owner != damageable.Owner)
                {
                    // 적과 충돌
                    OnAttack(damageable);
                    float damage = _donutInGameData.atk;
                    
                    if (modBySpd > 0f)
                    {
                        damage *= modBySpd;
                        bool isCrit = Random.value < _donutInGameData.crit; 
                        damage *= isCrit ? 1.5f : 1f;
                        damageable.TakeDamage(damage, isCrit, this);
                        
                        photonView.RPC("AnimateCollisionEffectRPC", RpcTarget.All, modBySpd);
                        EventHub.Instance.RaiseEvent(new PS.RequestDonutCollisionEvent(other.contacts[0].point, damage, GetUid(), damageable.GetUid(), CollisionType.Enemy, modBySpd, isCrit));
                    }
                }
                else
                {
                    // 같은 팀 끼리 충돌
                    OnAllyCollision(damageable);

                    photonView.RPC("AnimateCollisionEffectRPC", RpcTarget.All, modBySpd);
                    EventHub.Instance.RaiseEvent(new PS.RequestDonutCollisionEvent(other.contacts[0].point, 0, GetUid(), damageable.GetUid(), CollisionType.Ally, modBySpd, false));
                }
            }
            // 충돌 대상이 피해를 입는 대상이 아닌 경우
            else
            {
                OnEmptyCollision(other);

                photonView.RPC("AnimateCollisionEffectRPC", RpcTarget.All, modBySpd);
                EventHub.Instance.RaiseEvent(new PS.RequestDonutCollisionEvent(other.contacts[0].point, 0, GetUid(), null, CollisionType.Mutual, modBySpd, false));
            }
        }

        private void OnChangeBuff()
        {
            DonutInGameData result = new DonutInGameData(_donutInstanceData);
            _buffObjects.ForEach(x => x.ModifyAdd(result));
            _buffObjects.ForEach(x => x.ModifyMulti(result));
            _donutInGameData = result;
            
            _donutStatusUI.Setup(_health, GetAtk());
        }

        private void OnTurnStart(IGS.TurnStartEvent evt)
        {
            if (evt.turnOwner == Owner && evt.resetInteractable)
                Interactable = true;
            
            photonView.RPC("SyncInteractableRPC", RpcTarget.All, Interactable, (int)evt.turnOwner, (int)BattleState.TurnStart);
            OnTurnStarted(evt.turnOwner);
        }

        private void OnShotDonut(IGS.ShotDonutEvent evt)
        {
            if (evt.donutUid == GetUid())
            {
                Interactable = false;
                _shotForce = evt.shotForce;
            }
            else
            {
                _shotForce = Vector2.zero;
            }
            
            photonView.RPC("SyncInteractableRPC", RpcTarget.All, Interactable, (int)Owner, (int)BattleState.TurnStart);
        }
        
        private void OnTurnRunning(IGS.TurnRunningEvent evt)
        {
            _state = evt.turnOwner == Owner ? DonutState.Attack : DonutState.Defense;
            
            Shot(_shotForce * GetSlingShotPower());
            
            photonView.RPC("SyncInteractableRPC", RpcTarget.All, Interactable, (int)Owner, (int)BattleState.TurnRunning);
        }

        private void OnTurnEnd(IGS.TurnEndEvent evt)
        {
            _state = DonutState.Idle;
            _donutMove.SetSpeed(0f);
            
            OnTurnEnded(evt.turnOwner);
        }
        
        #region Skill & Buff Methods

        protected void OnSpawn()
        {
            _buffObjects.ToList().ForEach(x => x.OnSpawn());
            _skillObjects.ToList().ForEach(x => x.OnSpawn());
        }

        private void OnTurnStarted(InGameOwner turnOwner)
        {
            _buffObjects.ToList().ForEach(x => x.OnTurnStarted(turnOwner));
            _skillObjects.ToList().ForEach(x => x.OnTurnStarted(turnOwner));
        }

        private void OnAttack(IDamageable other)
        {
            _buffObjects.ToList().ForEach(x => x.OnAttack(other));
            _skillObjects.ToList().ForEach(x => x.OnAttack(other));
        }

        private void OnDefense(IDamageable source)
        {
            _buffObjects.ToList().ForEach(x => x.OnDefense(source));
            _skillObjects.ToList().ForEach(x => x.OnDefense(source));
        }

        private void OnAllyCollision(IDamageable damageable)
        {
            _buffObjects.ToList().ForEach(x => x.OnAllyCollision(damageable));
            _skillObjects.ToList().ForEach(x => x.OnAllyCollision(damageable));
        }

        private void OnEmptyCollision(Collision other)
        {
            _buffObjects.ToList().ForEach(x => x.OnEmptyCollision());
            _skillObjects.ToList().ForEach(x => x.OnEmptyCollision());
        }
        
        private void OnTurnEnded(InGameOwner turnOwner)
        {
            _buffObjects.ToList().ForEach(x => x.OnTurnEnded(turnOwner));
            _skillObjects.ToList().ForEach(x => x.OnTurnEnded(turnOwner));
        }

        #endregion Skill & Buff Methods

        public int GetAtk() => _donutInGameData.atk;
        public int GetDef() => _donutInGameData.def;
        public int GetHp() => _donutInGameData.hp;
        public float GetSlingShotPower() => _donutInGameData.slingShotPower;
        public string GetUid() => _donutInstanceData.uid;
        public Vector3 GetPosition() => transform.position;

        public float GetCurrentHp() => _health;
        public void SendSkillText(string str, bool isAdd)
        {
            if (photonView.IsMine == false)
                return;
            
            photonView.RPC("OnRequestSkillTextRPC", RpcTarget.All, str.ToString(), isAdd);
        }

        public void SendSkillEffect(ParticleType type)
        {
            if (photonView.IsMine == false)
                return;
            
            photonView.RPC("OnRequestSkillEffectRPC", RpcTarget.All, (int)type);
        }

        public DonutInstanceData GetDonutInstanceData() => _donutInstanceData;
        
        public void SetSpeed(float speed) => _donutMove.SetSpeed(speed);
        
        public bool IsStop() => _donutMove.IsStop();
        private void Shot(Vector2 force) => _donutMove.Shot(force);
        public void SetDragging(Vector2 force) => _donutHighlight.SetSelectedHighlights(force);

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_donutInGameData.atk);
                stream.SendNext(_donutInGameData.def);
                stream.SendNext(_donutInGameData.hp);
                stream.SendNext(_donutInGameData.crit);
                stream.SendNext(_donutInGameData.slingShotPower);
                
                stream.SendNext(_health);
            }
            else if (stream.IsReading)
            {
                _donutInGameData.atk = (int)stream.ReceiveNext();
                _donutInGameData.def = (int)stream.ReceiveNext();
                _donutInGameData.hp = (int)stream.ReceiveNext();
                _donutInGameData.crit = (float)stream.ReceiveNext();
                _donutInGameData.slingShotPower = (float)stream.ReceiveNext();
                
                _health = (float)stream.ReceiveNext();
                
                _donutStatusUI.Setup(_health, GetAtk());
            }
        }

        [PunRPC]
        private void InitializeRPC(string uid, string origin, int owner)
        {
            _donutMove = GetComponent<DonutMove>();
            _donutHighlight = GetComponent<DonutHighlight>();
            _donutStatusUI = GetComponentInChildren<DonutStatusUI>();
            
            _donutInstanceData = new DonutInstanceData() { uid = uid, origin = origin };
            _donutInGameData = new DonutInGameData(_donutInstanceData);
            Owner = (InGameOwner)owner;
            Interactable = true;
            
            DataManager.Instance.TryGetDonutData(_donutInstanceData.origin, out DonutData donutData);
            
            AddressableLoader.AssetLoadByPath<GameObject>(donutData.resourcePath, x =>
            {
                _donutOutfit = Instantiate(x, transform).transform;
                _donutOutfit.transform.localScale = Vector3.one;
                _donutOutfit.localPosition = Vector3.zero;
            }).Forget();
            
            gameObject.name = $"{donutData.donutName} : {Owner.ToString()}";
            
            // TODO : 샘플용 인디케이터 생성. 제거
            GameObject go = new GameObject("Indicator");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(0, 2, 2);
            go.transform.forward = Vector3.down;
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = indicator;
            sr.color = Owner == InGameOwner.Right ? Color.green : Color.red;
            
            TrailRenderer tr = GetComponentInChildren<TrailRenderer>();
            if (tr != null)
            {
                tr.material.color = Owner == InGameOwner.Left ? new Color(0.4549019f, 0.7529412f, 0.9882353f, 1) : new Color(1, 0.1333333f, 0.3803921f, 1f);
            }
            
            EventHub.Instance?.RegisterEvent<IGS.DragCancelledEvent>(OnDragCancelledEvent); 
        }

        [PunRPC]
        private void SyncInteractableRPC(bool interactable, int turnOwner, int turn)
        {
            Interactable = interactable;
            // _donutHighlight.SetColor(Owner, interactable);
            _donutHighlight.SetSelectHighlights(Owner, (InGameOwner)turnOwner, interactable, photonView.IsMine ? InGameOwner.Left : InGameOwner.Right, (BattleState)turn);
        }

        [PunRPC]
        private void AnimateCollisionEffectRPC(float modBySpd)
        {
            _donutOutfit.DOKill(true);
            _donutOutfit.DOPunchScale(Vector3.one * .2f * modBySpd, 1f);
            EventHub.Instance.RaiseEvent(new CameraAnimationStructs.RequestCameraAnimation("Punch", modBySpd));
        }
        
        private void OnDragCancelledEvent(IGS.DragCancelledEvent evt)
        {
            // 1. 이벤트의 UID가 이 도넛의 UID와 다르면 무시합니다.
            if (evt.donutUid != GetUid())
            {
                return;
            }

            // 2. 드래그가 취소되었으므로, 다시 턴 시작(TurnStart) 상태의 하이라이트를 복구합니다.
            
            InGameOwner localPlayerOwner = photonView.IsMine ? InGameOwner.Left : InGameOwner.Right;
            
            Interactable = true;
            // 하이라이트를 복구합니다. (Interactable은 이미 TurnStartState에서 true를 유지하고 있습니다.)
            _donutHighlight.SetSelectHighlights(
                Owner,                     // 이 도넛의 소유자
                evt.currentTurnOwner,      // 이벤트로 받은 현재 턴 소유자
                Interactable,              // Interactable 상태 (true)
                localPlayerOwner,          // 로컬 플레이어 진영
                BattleState.TurnStart      // 턴 시작 상태로 복귀
            );
        }

        [PunRPC]
        private void OnTakeDamageEffectRPC(int damage, bool isCrit)
        {
            EventHub.Instance?.RaiseEvent(new DNC.RequestDamageNumber(damage, isCrit, transform.position));
        }

        [PunRPC]
        private void OnRequestSkillTextRPC(string str, bool isAdd)
        {
            EventHub.Instance.RaiseEvent(new STS.RequestSkillText(str, isAdd, transform.position));
        }

        [PunRPC]
        private void OnRequestSkillEffectRPC(int type)
        {
            EventHub.Instance.RaiseEvent(new EffectStructs.PlayParticleEvent((ParticleType)type, transform.position + Vector3.up * 2f, Quaternion.identity, Vector3.one * 10f));
        }
    }
}
