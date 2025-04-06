using System;

namespace NiGames.Essentials.StateMachine
{
    public readonly struct Transition<T> : IEquatable<Transition<T>> 
        where T : StateMachine<T>
    {
        public readonly State<T> From;
        public readonly State<T> To;
        public readonly Func<bool> Condition;
        public readonly int Priority;
        
        public Transition(State<T> to, Func<bool> condition, int priority = 0) : this()
        {
            To = to;
            Condition = condition;
            Priority = priority;
        }
        
        public Transition(State<T> from, State<T> to, Func<bool> condition, int priority = 0)
        {
            From = from;
            To = to;
            Condition = condition;
            Priority = priority;
        }
        
        public bool IsValid()
        {
            return To != null && Condition != null;
        }
        
        public bool IsValidAnyTransition()
        {
            return IsValid() && From == null;
        }
        
        public bool Equals(Transition<T> other)
        {
            return Equals(From, other.From) && Equals(To, other.To) && Equals(Condition, other.Condition) && Priority == other.Priority;
        }

        public override bool Equals(object obj)
        {
            return obj is Transition<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To, Condition, Priority);
        }
        
        public static bool operator ==(Transition<T> p1, Transition<T> p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Transition<T> p1, Transition<T> p2)
        {
            return !(p1 == p2);
        }
    }
}