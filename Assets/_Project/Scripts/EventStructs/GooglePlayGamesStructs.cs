using System;

namespace _Project.Scripts.EventStructs
{
    public static class GooglePlayGamesStructs
    {
        /// <summary> Google Play Games 인증 요청 이벤트 </summary>
        public struct RequestAuthEvent : IEvent { }
        
        
        public struct AuthSuccessEvent : IEvent {}
        public struct AuthFailedEvent : IEvent {}


        public struct RequestServerSideAccess : IEvent
        {
            public Action<string> callback;
            public RequestServerSideAccess(Action<string> callback) => this.callback = callback;
        }
    }
}
