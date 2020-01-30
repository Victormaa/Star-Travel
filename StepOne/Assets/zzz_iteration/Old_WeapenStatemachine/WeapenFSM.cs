namespace WeapenStateMachine
{
    public class WeapenFSM<Weapen>
    {
        public WeapenState<Weapen> CurrentState { get; private set; }
        public Weapen Owner;

        public WeapenFSM(Weapen _owner)
        {
            Owner = _owner;
            CurrentState = null;
        }

        public void ReplaceState(WeapenState<Weapen> _newstate)
        {
            if(CurrentState != null)
            {
                CurrentState.OnExit(Owner);
            }
            CurrentState = _newstate;
            CurrentState.OnEnter(Owner);
        }

        public void UpdateState()
        {
            if(CurrentState != null)
            {
                CurrentState.OnUpdate(Owner);
            }
        }

        
        public void ExitState()
        {
            if (CurrentState != null)
            {
                CurrentState.OnExit(Owner);
            }
            return;
        }
        

        
    }

    public abstract class WeapenState<Weapen>
    {
        public abstract void OnEnter(Weapen _other);
        public abstract void OnUpdate(Weapen _other);
        public abstract void OnExit(Weapen _other);
        public abstract void Shoot();
    }

}

