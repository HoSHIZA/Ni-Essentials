using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace NiGames.Essentials.StateMachine
{
    /// <summary>
    /// An abstract class for creating a state machine.
    /// </summary>
    /// <example>
    /// class ExampleStateMachine : StateMachine<![CDATA[<ExampleStateMachine>]]>
    /// </example>
    /// <typeparam name="T">The type of the state machine.</typeparam>
    [PublicAPI]
    public abstract class StateMachine<T> where T : StateMachine<T>
    {
        public State<T> CurrentState { get; private set; }
        
        public event Action<Type> OnStateChanged;
        
        private Dictionary<Type, State<T>> _states;
        private List<Transition<T>> _transitions = new();
        
        private State<T> _initialState;
        private bool _init;
        
        public Type CurrentStateType => CurrentState.GetType();

        /// <summary>
        /// Sets up the state machine with an array of states, and initializes the current state.
        /// </summary>
        /// <param name="states">An array of states to set up the state machine.</param>
        /// <param name="initUseFirstState">If true, initializes the current state with the first state in the array.</param>
        protected void Init(State<T>[] states, bool initUseFirstState = false)
        {
            if (_init) return;
            
            _states = new Dictionary<Type, State<T>>(states.Length);
            
            foreach (var state in states)
            {
                state.SetStateMachine(this as T);
                
                TryAddState(state);
            }
            
            if (initUseFirstState && _states.Count > 0)
            {
                SetInitialState(_states.First().Value.GetType());

                _init = true;
            }
        }

        /// <summary>
        /// Changes the current state of the state machine to the specified type of state.
        /// </summary>
        [MethodImpl(256)]
        public bool ChangeState<TState>() where TState : State<T>
        {
            return ChangeState(typeof(TState));
        }
        
        /// <summary>
        /// Changes the current state of the state machine to the specified type of state.
        /// </summary>
        public bool ChangeState(Type type)
        {
            var state = GetState(type);
            
            if (state == null) return false;
            if (state == CurrentState) return false;
            
            CurrentState?.Exit();
            CurrentState = state;
            
            OnStateChanged?.Invoke(CurrentState.GetType());
            
            CurrentState.Enter();
            
            return true;
        }
        
        /// <summary>
        /// Returns a value indicating whether the state machine has a state of the specified type.
        /// </summary>
        public bool HasState<TState>() where TState : State<T>
        {
            return HasState(typeof(TState));
        }
        
        /// <summary>
        /// Returns a value indicating whether the state machine has a state of the specified type.
        /// </summary>
        public bool HasState(Type type)
        {
            return _states.ContainsKey(type);
        }
        
        public bool StateBelongsThisStateMachine(Type type)
        {
            return type.GetGenericArguments().Contains(typeof(StateMachine<T>));
        }
        
        /// <summary>
        /// Updates the state machine, executing the Update method of the current state.
        /// </summary>
        public void Update()
        {
            CurrentState.Update();
        }

        /// <summary>
        /// Updates the state machine, executing the FixedUpdate method of the current state.
        /// </summary>
        public void PhysicsUpdate()
        {
            CurrentState.PhysicsUpdate();
        }
        
        /// <summary>
        /// Attempts to add a state to the state machine.
        /// </summary>
        public bool TryAddState(State<T> state)
        {
            return _states.TryAdd(state.GetType(), state);
        }
        
        /// <summary>
        /// Add 'state to state' transition to the state machine.
        /// </summary>
        public void AddTransition<TFrom, TTo>(Func<bool> condition)
            where TFrom : State<T>
            where TTo : State<T>
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            
            var stateFrom = GetState(typeof(TFrom));
            var stateTo = GetState(typeof(TTo));
            
            _transitions.Add(new Transition<T>(stateFrom, stateTo, condition));
        }
        
        /// <summary>
        /// Add 'any to state' transition to the state machine.
        /// </summary>
        public void AddAnyTransition<TTo>(TTo to, Func<bool> condition)
            where TTo : State<T>
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            
            var stateTo = GetState<TTo>();
            
            _transitions.Add(new Transition<T>(null, stateTo, condition));
        }
        
        /// <summary>
        /// Sets the initial state of the state machine to the specified state type.
        /// </summary>
        /// <remarks>
        /// This method can only be called during the setup phase of the state machine, before any state changes
        /// have occurred. If called after setup is complete, this method has no effect.
        /// </remarks>
        [MethodImpl(256)]
        protected void SetInitialState<TState>() where TState : State<T>
        {
            SetInitialState(typeof(TState));
        }
        
        /// <summary>
        /// Sets the initial state of the state machine to the specified state type.
        /// </summary>
        /// <remarks>
        /// This method can only be called during the setup phase of the state machine, before any state changes
        /// have occurred. If called after setup is complete, this method has no effect.
        /// </remarks>
        protected void SetInitialState(Type type)
        {
            if (_init) return;
            
            _initialState = GetState(type);
            
            ChangeState(_initialState.GetType());
            
            _init = true;
        }

        /// <summary>
        /// Sets the state corresponding to the active transition.
        /// </summary>
        protected void SetStateByTransition()
        {
            Transition<T>? bestTransition = null;
            
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

            ChangeState(bestTransition.Value.To.GetType());
        }
        
        [MethodImpl(256)]
        private State<T> GetState<TState>()
        {
            return GetState(typeof(TState));
        }
        
        private State<T> GetState(Type type)
        {
            return _states.GetValueOrDefault(type);
        }
    }
}