namespace _Project.Scripts.StateMachine.Native
{
    /// <summary>
    /// 상태 객체의 구현을 위한 Interface
    /// </summary>
    /// <typeparam name="T">구현할 상태를 사용하는 객체</typeparam>
    public interface IState<T> where T : class
    {
        public void Enter(T owner);
        public void Exit(T owner);
        public void Update(T owner);
    }
}
