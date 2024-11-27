namespace NiGames.Essentials.StateMachine
{
    /// <summary>
    /// Base class for states used in a state machine.
    /// </summary>
    /// <typeparam name="T">The type of state machine this state is for.</typeparam>
    public abstract class State<T> where T : StateMachine<T>
    {
        /// <summary>
        /// The state machine this state is a part of.
        /// </summary>
        protected T StateMachine { get; private set; }
        
        /// <summary>
        /// Sets the state machine for this state.
        /// </summary>
        /// <param name="stateMachine">The state machine to set.</param>
        protected internal void SetStateMachine(T stateMachine)
        {
            if (StateMachine != null) return;
            
            StateMachine = stateMachine;
        }
        
        /// <summary>
        /// Called when this state is entered.
        /// </summary>
        public virtual void Enter(IStateContext context) 
        {
            Enter();
        }
        
        /// <summary>
        /// Called when this state is entered.
        /// </summary>
        public abstract void Enter();
        
        /// <summary>
        /// Called when this state is exited.
        /// </summary>
        public abstract void Exit();
        
        /// <summary>
        /// Called every frame to update this state.
        /// </summary>
        public virtual void Update() { }
        
        /// <summary>
        /// Called every physics update to update this state.
        /// </summary>
        public virtual void PhysicsUpdate() { }
        
        /// <summary>
        /// Called every frame after Update to update this state.
        /// </summary>
        public virtual void LateUpdate() { }
    }

    /// <summary>
    /// Base class for states used in a state machine. 
    /// </summary>
    /// <typeparam name="T">The type of state machine this state is for.</typeparam>
    /// <typeparam name="TContext">Context.</typeparam>
    public abstract class State<T, TContext> : State<T> 
        where T : StateMachine<T>
        where TContext : IStateContext
    {
        public sealed override void Enter()
        {
            Enter(default);
        }
        
        public sealed override void Enter(IStateContext context)
        {
            Enter(context is TContext ctx ? ctx : default);
        }
        
        protected abstract void Enter(TContext context);
    }
}