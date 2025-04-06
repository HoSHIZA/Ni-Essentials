using System;
using System.Collections.Generic;

namespace NiGames.Essentials.StateMachine
{
    public abstract class TransitionStateMachine<T> : StateMachine<TransitionStateMachine<T>> 
        where T : TransitionStateMachine<T>
    {
        private readonly List<Transition<TransitionStateMachine<T>>> _transitions = new();
        
        /// <summary>
        /// Sets the state corresponding to the active transition.
        /// </summary>
        protected void SetStateByTransition()
        {
            Transition<TransitionStateMachine<T>>? bestTransition = null;
            
            foreach (var transition in _transitions)
            {
                if (!transition.IsValid()) continue;
                
                var isAny = transition.From == null;
                
                if (!isAny && transition.From != CurrentState) continue;
                if (!transition.Condition.Invoke()) continue;
                
                if (bestTransition == null || (isAny
                        ? transition.Priority >= bestTransition.Value.Priority
                        : transition.Priority > bestTransition.Value.Priority))
                {
                    bestTransition = transition;
                }
            }
            
            if (!bestTransition.HasValue) return;
            if (!bestTransition.Value.IsValid()) return;
            if (bestTransition.Value.To == CurrentState) return;

            ChangeStateByType(bestTransition.Value.To.GetType());
        }
        
        /// <summary>
        /// Add 'state to state' transition to the state machine.
        /// </summary>
        public void AddTransition<TFrom, TTo>(Func<bool> condition)
            where TFrom : State<TransitionStateMachine<T>>
            where TTo : State<TransitionStateMachine<T>>
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            
            var stateFrom = GetStateByType(typeof(TFrom));
            var stateTo = GetStateByType(typeof(TTo));
            
            _transitions.Add(new Transition<TransitionStateMachine<T>>(stateFrom, stateTo, condition));
        }
        
        /// <summary>
        /// Add 'any to state' transition to the state machine.
        /// </summary>
        public void AddAnyTransition<TTo>(TTo to, Func<bool> condition)
            where TTo : State<TransitionStateMachine<T>>
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            
            var stateTo = GetState<TTo>();
            
            _transitions.Add(new Transition<TransitionStateMachine<T>>(null, stateTo, condition));
        }
    }
}