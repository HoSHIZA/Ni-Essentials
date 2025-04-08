using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NiGames.Essentials.Collections;

namespace NiGames.Essentials.StateMachine
{
    /// <summary>
    /// An abstract class for creating a state machine.
    /// </summary>
    /// <example>
    /// class ExampleStateMachine : StateMachine<![CDATA[<ExampleStateMachine>]]>
    /// </example>
    /// <typeparam name="T">The type of the state machine.</typeparam>
    public abstract class StateMachine<T> : IStateMachine
        where T : StateMachine<T>
    {
        public event Action<StateChangeData> OnStateChanged;
        
        public State<T> CurrentState { get; private set; }
        public byte CurrentStateId => _statesMapping.GetValueOrDefault(CurrentStateType);
        public Type CurrentStateType => CurrentState.GetType();
        
        private Dictionary<Type, State<T>> _states;
        private BidirectionalDictionary<Type, byte> _statesMapping;
        
        private State<T> _initialState;
        private bool _init;
        
        /// <summary>
        /// Sets up the state machine with an array of states, and initializes the current state.
        /// </summary>
        /// <param name="states">An array of states to set up the state machine.</param>
        /// <param name="initUseFirstState">If true, initializes the current state with the first state in the array.</param>
        protected void Init(State<T>[] states, bool initUseFirstState = false)
        {
            if (_init) return;
            
            _states = new Dictionary<Type, State<T>>(states.Length);
            _statesMapping = new BidirectionalDictionary<Type, byte>(states.Length);

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
        public bool ChangeState<TState>() where TState : State<T>
        {
            var state = GetStateByType(typeof(TState));
            
            return SetStateInternal(state);
        }
        
        /// <summary>
        /// Changes the current state of the state machine to the specified type of state.
        /// </summary>
        public bool ChangeState<TState, TContext>(TContext context) 
            where TState : State<T, TContext>
            where TContext : IStateContext
        {
            var state = GetStateByType(typeof(TState));
            
            return SetStateInternal(state, context);
        }
        
        /// <summary>
        /// Changes the current state of the state machine to the specified type of state.
        /// </summary>
        public bool ChangeStateByType(Type type, IStateContext context = null)
        {
            var state = GetStateByType(type);

            return SetStateInternal(state, context);
        }

        /// <summary>
        /// Changes the current state of the state machine to the specified type of state by id.
        /// </summary>
        public bool ChangeStateById(byte stateId, IStateContext context = null)
        {
            var state = GetStateById(stateId);
            
            return SetStateInternal(state, context);
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
        
        /// <summary>
        /// Returns a value indicating whether the specified state belongs to the current state machine
        /// </summary>
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
            if (!_states.TryAdd(state.GetType(), state)) return false;
            
            var id = (byte)_statesMapping.Count;
            var type = state.GetType();
            
            _statesMapping.Add(type, id);
            
            return true;
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
            
            _initialState = GetStateByType(type);
            
            ChangeStateByType(_initialState.GetType());
            
            _init = true;
        }
        
        /// <summary>
        /// Sets the initial state of the state machine to the specified state type by id.
        /// </summary>
        /// <remarks>
        /// This method can only be called during the setup phase of the state machine, before any state changes
        /// have occurred. If called after setup is complete, this method has no effect.
        /// </remarks>
        protected void SetInitialStateById(byte stateId)
        {
            if (_init) return;
            
            _initialState = GetStateById(stateId);
            
            ChangeStateByType(_initialState.GetType());
            
            _init = true;
        }
        
        protected State<T> GetState<TState>()
        {
            return GetStateByType(typeof(TState));
        }
        
        protected State<T> GetStateByType(Type type)
        {
            return _states.GetValueOrDefault(type);
        }
        
        protected State<T> GetStateById(byte stateId)
        {
            var type = _statesMapping.GetKeyOrDefault(stateId);
            
            return type == null ? null : _states.GetValueOrDefault(type);
        }
        
        protected virtual bool SetStateInternal(State<T> state, IStateContext context = null)
        {
            if (state == null) return false;
            if (state == CurrentState) return false;
            
            CurrentState?.Exit();
            CurrentState = state;
            
            var stateType = state.GetType();
            var stateId = _statesMapping.GetValueOrDefault(stateType);
            var changeData = new StateChangeData(stateType, stateId);
            
            OnStateChanged?.Invoke(changeData);
            
            if (context == null)
            {
                CurrentState.Enter();
            }
            else
            {
                CurrentState.Enter(context);
            }

            return true;
        }
    }
}