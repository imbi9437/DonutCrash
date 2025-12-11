using _Project.Scripts.InGame.Remote;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using IGS = _Project.Scripts.EventStructs.InGameStructs;
using PS = _Project.Scripts.EventStructs.PhotonStructs;
using AS = _Project.Scripts.EventStructs.AudioStruct;
using System;
using Photon.Realtime;
using Random = UnityEngine.Random;

namespace _Project.Scripts.InGame.StatePattern
{
    public class BattleController : MonoStateMachine
    {
        public Dictionary<InGameOwner, string> PlayerNicknames { get; private set; } = new Dictionary<InGameOwner, string>();
        public DonutObject donutObjectPrefab;
        public RemoteBattleController remoteBattleControllerPrefab;

        [Header("Scene Visualizers")]
        [SerializeField] private GuidelineRender _guidelineRender;
        [SerializeField] private SplineController _splineController;
        [Space]
        [Header("Map List")]
        public List<GameObject> maps;
        [Space]
        [Header("Obstacles of Map2")]
        public List<GameObject> jelly;

        public List<GameObject> obstacles;

        public GuidelineRender GuidelineRenderer => _guidelineRender;
        public SplineController SplineController => _splineController;
        
        private InGameOwner _turnOwner = InGameOwner.None;

        private static BattleController _instance;

        private readonly Dictionary<InGameOwner, DeckData> _deckData = new Dictionary<InGameOwner, DeckData>()
        {
            { InGameOwner.Left , null},
            { InGameOwner.Right , null},
        };
        private readonly Dictionary<InGameOwner, BakerObject> _bakers =  new Dictionary<InGameOwner, BakerObject>()
        {
            { InGameOwner.Left , null},
            { InGameOwner.Right , null},
        };
        private readonly Dictionary<InGameOwner, List<DonutObject>> _donuts = new Dictionary<InGameOwner, List<DonutObject>>()
        {
            { InGameOwner.Left, new List<DonutObject>() },
            { InGameOwner.Right, new List<DonutObject>() },
        };

        public List<GameObject> leftPlayerSpawnZones;
        public List<GameObject> rightPlayerSpawnZones;

        protected override void Awake()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                RemoteBattleController remoteInstance = Instantiate(remoteBattleControllerPrefab);

                remoteInstance.SetupVisualizers(_guidelineRender, _splineController);
                Destroy(this);
                return;
            }
            
            base.Awake();

            if(_guidelineRender == null) Debug.LogError("BattleController: GuidelineRender is not assigned in the inspector.");
            if(_splineController == null) Debug.LogError("BattleController: SplineController is not assigned in the inspector.");

            if (_instance && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;
        }

        private void Start()
        {
            EventHub.Instance.RegisterEvent<IGS.SetDeckData>(SetDeckData);
            
            EventHub.Instance.RegisterEvent<IGS.BattleStartEvent>(SendRpcBattleStart);
            EventHub.Instance.RegisterEvent<IGS.TurnStartEvent>(SendRpcTurnStart);
            EventHub.Instance.RegisterEvent<IGS.TurnRunningEvent>(SendRpcTurnRunning);
            EventHub.Instance.RegisterEvent<IGS.TurnEndEvent>(SendRpcTurnEnd);
            EventHub.Instance.RegisterEvent<IGS.BattleEndEvent>(SendRpcBattleEnd);
            
            EventHub.Instance.RegisterEvent<PS.CompleteWaitingPlayer>(OnCompleteWaitingPlayer);
            EventHub.Instance.RegisterEvent<IGS.RemoveDonutEvent>(RequestRemoveDonut);
            EventHub.Instance.RegisterEvent<IGS.CalledGameOver>(CalledGameOver);
            EventHub.Instance.RegisterEvent<IGS.TryRandomObstacle>(OnTryRandomObstacle);
        }

        private void OnDisable()
        {
            EventHub.Instance?.UnRegisterEvent<IGS.SetDeckData>(SetDeckData);
            
            EventHub.Instance?.UnRegisterEvent<IGS.BattleStartEvent>(SendRpcBattleStart);
            EventHub.Instance?.UnRegisterEvent<IGS.TurnStartEvent>(SendRpcTurnStart);
            EventHub.Instance?.UnRegisterEvent<IGS.TurnRunningEvent>(SendRpcTurnRunning);
            EventHub.Instance?.UnRegisterEvent<IGS.TurnEndEvent>(SendRpcTurnEnd);
            EventHub.Instance?.UnRegisterEvent<IGS.BattleEndEvent>(SendRpcBattleEnd);
            
            EventHub.Instance?.UnRegisterEvent<PS.CompleteWaitingPlayer>(OnCompleteWaitingPlayer);
            EventHub.Instance?.UnRegisterEvent<IGS.RemoveDonutEvent>(RequestRemoveDonut);
            EventHub.Instance?.UnRegisterEvent<IGS.CalledGameOver>(CalledGameOver);
            EventHub.Instance?.UnRegisterEvent<IGS.TryRandomObstacle>(OnTryRandomObstacle);
            
            _bakers.ForEach(x =>
            {
                x.Value?.DeActive();
            });
        }
        public string GetNickname(InGameOwner owner)
        {
            if(PlayerNicknames.TryGetValue(owner, out string nickname))
            {
                return nickname;
            }
            return "Unknown Player";
        }

        private void InitializePlayerNicknames()
        {
            if(PhotonNetwork.CurrentRoom == null)
            {
                Debug.LogError("포톤 룸이 없습니다.");
                return;
            }
            PlayerNicknames.Clear();

            foreach (var playerEntry in PhotonNetwork.CurrentRoom.Players)
            {
                Photon.Realtime.Player player = playerEntry.Value;
                InGameOwner owner = player.IsMasterClient ? InGameOwner.Left : InGameOwner.Right;

                if (string.IsNullOrEmpty(player.NickName))
                {
                    Debug.LogError($"플레이어 {owner}의 NickName이 유효하지 않습니다.");
                    continue;
                }
                PlayerNicknames.Add(owner,player.NickName);
                Debug.Log($"[Nickame Init Success] {owner} : {player.NickName}");
            }
            if(PlayerNicknames.Count != 2)
            {
                Debug.LogError($"닉네임 초기화 실패 : 등록된 플레이어 수가 {PlayerNicknames.Count}명입니다.");
            }
        }
        
        #region Event Wrapper Methods

        private void SetDeckData(IGS.SetDeckData evt) => AddDeckData(evt.owner, evt.deck);
        
        private void SendRpcBattleStart(IGS.BattleStartEvent evt) => EventHub.Instance?.RaiseEvent(new PS.ChangeBattleState(BattleState.Start, InGameOwner.None));
        private void SendRpcTurnStart(IGS.TurnStartEvent evt) => EventHub.Instance?.RaiseEvent(new PS.ChangeBattleState(BattleState.TurnStart, evt.turnOwner));
        private void SendRpcTurnRunning(IGS.TurnRunningEvent evt) => EventHub.Instance?.RaiseEvent(new PS.ChangeBattleState(BattleState.TurnRunning, evt.turnOwner));
        private void SendRpcTurnEnd(IGS.TurnEndEvent evt) => EventHub.Instance?.RaiseEvent(new PS.ChangeBattleState(BattleState.TurnEnd, evt.turnOwner));
        private void SendRpcBattleEnd(IGS.BattleEndEvent evt) => EventHub.Instance?.RaiseEvent(new PS.ChangeBattleState(BattleState.End, InGameOwner.None));
        
        private void OnCompleteWaitingPlayer(PS.CompleteWaitingPlayer evt) => CompleteWaitingPlayer();
        private void RequestRemoveDonut(IGS.RemoveDonutEvent evt) => RemoveDonut(evt.donut);
        private void CalledGameOver(IGS.CalledGameOver evt) => ChangeState (BattleState.End);
        
        #endregion Event Wrapper Methods
        private void CompleteWaitingPlayer()
        {
            InitializePlayerNicknames();
            ChangeState(BattleState.Start);
        }

        public void AddBaker(BakerObject baker)
        {
            if (_bakers[baker.Owner] != null)
            {
                Debug.LogWarning($"이미 {baker.Owner}의 마녀가 준비되었습니다.");
                return;
            }
            
            _bakers[baker.Owner] = baker;
        }
        
        public void AddDonut(DonutObject donut)
        {
            if (_donuts[donut.Owner].Count > 2)
            {
                Debug.LogWarning($"이미 소환된 {donut.Owner.ToString()} 도넛의 갯수가 3개입니다. 소환 규칙에 어긋납니다.");
                return;
            }

            if (_donuts[InGameOwner.Left].Contains(donut) || _donuts[InGameOwner.Right].Contains(donut))
            {
                string ownership = _donuts[InGameOwner.Left].Contains(donut) ? "Left" : "Right";
                Debug.LogWarning($"이미 생성된 {donut.Owner.ToString()} 도넛을 {ownership}에 추가하려고 합니다.");
                return;
            }

            DonutInstanceData targetData = _deckData[donut.Owner].waitingDonuts.Find(x => x.uid == donut.GetUid());
            
            _deckData[donut.Owner].waitingDonuts.Remove(targetData);
            _deckData[donut.Owner].fieldDonuts.Add(targetData);
            _donuts[donut.Owner].Add(donut);
            EventHub.Instance.RaiseEvent(new IGS.DonutSpawnedEvent(donut));
            EventHub.Instance.RaiseEvent(new PS.RequestChangeDonutList(donut.Owner, _deckData[donut.Owner].fieldDonuts, _deckData[donut.Owner].waitingDonuts));
            Debug.Log($"{donut.Owner.ToString()}에게 {donut.name} 도넛 추가");
        }

        private void RemoveDonut(DonutObject donut)
        {
            if (!_donuts[donut.Owner].Contains(donut))
                return;
            
            DonutInstanceData data = _deckData[donut.Owner].fieldDonuts.Find(x => x.uid == donut.GetUid());
            
            _deckData[donut.Owner].fieldDonuts.Remove(data);
            _donuts[donut.Owner].Remove(donut);
            PhotonManager.PunDestroy(donut.gameObject);
            EventHub.Instance.RaiseEvent(new AS.PlaySfxAudioEvent(AudioType.SFX_Fight_07, 1f));
            _deckData[donut.Owner].deathCount--;
            EventHub.Instance.RaiseEvent(new PS.RequestChangeDeathCount(donut.Owner, _deckData[donut.Owner].deathCount));
            EventHub.Instance.RaiseEvent(new PS.RequestChangeDonutList(donut.Owner, _deckData[donut.Owner].fieldDonuts, _deckData[donut.Owner].waitingDonuts));
        }
        
        public bool CheckIfOwnerHasDied(InGameOwner owner)
        {
            if (!_deckData.ContainsKey(owner))
            {
                Debug.LogWarning($"{owner}의 DeckData를 BattleController에서 찾을 수 없습니다.");
                return false;
            }

            if (_deckData[owner].deathCount > 0)
            {
                return false;
            }

            Debug.Log($"{owner}가 게임오버 되었습니다.");
            return true;
        }
        
        private void AddDeckData(InGameOwner owner, DeckData data) 
        {
            if (_deckData.TryGetValue(owner, out DeckData value) == false)
            {
                Debug.LogError($"덱 데이터 딕셔너리에 {owner}키가 존재하지 않습니다.");
                return;
            }

            if (value != null)
            {
                Debug.LogError($"{owner}의 덱 데이터가 이미 존재합니다.");
                return;
            }
            
            _deckData[owner] = data;
        }
        
        #region Public Method for MonoState

        public InGameOwner GetLocalPlayerOwner()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return InGameOwner.Left;
            }
            else
            {
                return InGameOwner.Right;
            }
        }

        public DeckData GetDeckData(InGameOwner owner) => _deckData[owner];
        public bool IsDeck() => _deckData.ContainsKey(InGameOwner.Left) && _deckData.ContainsKey(InGameOwner.Right);

        public List<DonutObject> GetDonutsByOwner(InGameOwner owner) => _donuts[owner].ToList();
        public InGameOwner GetTurnOwner() => _turnOwner;
        public InGameOwner GetOpponentOwner() => _turnOwner == InGameOwner.Left ? InGameOwner.Right : InGameOwner.Left;
        public void SetTurnOwner(InGameOwner owner) => _turnOwner = owner;
        public void ToggleTurnOwner() => _turnOwner = _turnOwner == InGameOwner.Left ? InGameOwner.Right : InGameOwner.Left;

        public void ChangeState(BattleState state) => ChangeState((int)state);

        #endregion Public Method for MonoState

        #region Public Method for skill and buff

        public static bool TryGetDonutsByOwner(InGameOwner owner, out List<DonutObject> donuts)
        {
            donuts = _instance?.GetDonutsByOwner(owner).ToList();
            return donuts != null;
        }

        public static bool TryGetOpponentDonutsByOwner(InGameOwner owner, out List<DonutObject> donuts)
        {
            donuts = _instance?.GetDonutsByOwner(owner == InGameOwner.Left ? InGameOwner.Right : InGameOwner.Left).ToList();
            return donuts != null;
        }

        public static bool TryGetDeathCountByOwner(InGameOwner owner, out int? deathCount)
        {
            deathCount = _instance?._deckData[owner]?.deathCount;
            return deathCount != null;
        }
        private void OnTryRandomObstacle(IGS.TryRandomObstacle evt)
        {
            int randomIndex = UnityEngine.Random.Range(0, maps.Count);
            GameObject randomMap = maps[randomIndex];

            if(randomIndex == 1)
            {
                int scale1 = UnityEngine.Random.Range(10, 21);
                int scale2 = UnityEngine.Random.Range(10, 21);
                int scale3 = UnityEngine.Random.Range(10, 21);
                int scale4 = UnityEngine.Random.Range(10, 21);

                jelly[0].transform.localScale = new Vector3(0.01f * scale1, 0.01f * scale1, 0.01f * scale1);
                jelly[1].transform.localScale = new Vector3(0.01f * scale2, 0.01f * scale2, 0.01f * scale2);
                jelly[2].transform.localScale = new Vector3(0.01f * scale3, 0.01f * scale3, 0.01f * scale3);
                jelly[3].transform.localScale = new Vector3(0.01f * scale4, 0.01f * scale4, 0.01f * scale4);
            }

            if(randomMap != null)
            {
                randomMap.transform.position =Vector3.zero;
            }
            else
            {
                Debug.LogError($"선택된 인덱스 {randomIndex}의 맵이 Null입니다.");
            }

            int ranNum = Random.Range(0, 3);
            for (int i = 0; i < ranNum; i++)
            {
                PhotonManager.PunInstantiate(obstacles.Shuffle()[0], new Vector3(Random.Range(-8, 8), 0, Random.Range(-6, 6)), Quaternion.identity);
            }
        }

        #endregion Public Method for skill and buff
    }
}
