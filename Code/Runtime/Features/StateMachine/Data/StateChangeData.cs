using System;

namespace NiGames.Essentials.StateMachine
{
    public readonly struct StateChangeData
    {
        public readonly Type StateType;
        public readonly byte StateId;
        
        public StateChangeData(Type stateType, byte stateId)
        {
            StateType = stateType;
            StateId = stateId;
        }
    }
}