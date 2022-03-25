
namespace Enemy.FSM
{
	public abstract class EnemyState
	{
		protected EnemyFSM fsm;

		protected EnemyState(EnemyFSM fsm)
		{
			this.fsm = fsm;
		}

		public abstract void OnEnter();
		public virtual void OnUpdate() { }
		public abstract void OnExit();
	}
}