namespace Infrastructure {

    public interface IState
    {
        public void Enter();
        public void Execute();
        public void Exit();
    }
    public class StateMachine
    {
        private IState currentState;


        public void ChangeState(IState nextState)
        {
            if (currentState != null)
                currentState.Exit();

            currentState = nextState;
            currentState.Enter();
        }

        public void Update()
        {
            if (currentState != null)
                currentState.Execute();
        }
    }
}