namespace _Project.Scripts.EventStructs
{
    /// <summary>
    /// <see cref="EventHub"/>에 등록할 함수 타입을 제한하기 위한 인터페이스
    /// </summary>
    public interface IEvent
    {}
    public struct DonutMovementStopped : IEvent
    {
        //누가 발사를 시작했는지(현재 내 턴이었는지)
        public bool launchedByMyTurn;
        public string stoppedDonutName;
        public DonutMovementStopped(bool turn, string name)
        {
            this.launchedByMyTurn = turn;
            this.stoppedDonutName = name;
        }
    }
}
