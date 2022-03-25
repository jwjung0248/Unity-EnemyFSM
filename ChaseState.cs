
namespace Enemy.FSM
{
	public class ChaseState : EnemyState
	{
		public ChaseState(EnemyFSM fsm) : base(fsm) { }

		public override void OnEnter()
		{
			fsm.Anim.SetBool(fsm.animHash_IsRun, true);

			fsm.NavAgent.speed = fsm.Ener.OriginData.moveSpeed * 2f;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			float distance = fsm.Tr.position.DistanceIgnoreYAxis(fsm.TargetTr.position);

			if (distance < fsm.Ener.CurrentAttackData.range)
			{
				fsm.ChangeState(EnemyFSM.EStateType.ATTACK);
			}
			else if(distance > fsm.Ener.OriginData.detectionDistance)
			{
				fsm.ChangeState(EnemyFSM.EStateType.IDLE);
			}
			else
			{
				fsm.NavAgent.ResetPath();
				fsm.NavAgent.SetDestination(fsm.TargetTr.position);
			}
		}

		public override void OnExit()
		{
			fsm.Anim.SetBool(fsm.animHash_IsRun, false);
			fsm.NavAgent.ResetPath();
		}
	}
}