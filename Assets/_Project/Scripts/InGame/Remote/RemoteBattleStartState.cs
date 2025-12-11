using _Project.Scripts.EventStructs;
using PS = _Project.Scripts.EventStructs.PhotonStructs;

namespace _Project.Scripts.InGame.Remote
{
    public class RemoteBattleStartState : MonoState
    {
        public override int index => (int)BattleState.Start;

        protected override void OnEnable()
        {
            EventHub.Instance?.RaiseEvent(new PS.RequestSetUserProfile());
            EventHub.Instance?.RaiseEvent(new AudioStruct.PlayBackAudioEvent(AudioType.BGM_06, .5f));
        }
    }
}
