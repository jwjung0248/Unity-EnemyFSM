
namespace Enemy.FSM
{
	public class IdleState : EnemyState
	{
		public IdleState(EnemyFSM fsm) : base(fsm) { }

		public override void OnEnter() 
		{

		}

		public override void OnUpdate() 
		{
			base.OnUpdate();

			if (fsm.TargetTr)
			{
				float distance = fsm.Tr.position.DistanceIgnoreYAxis(fsm.TargetTr.position);

				if (distance < fsm.Ener.OriginData.detectionDistance)
				{
					if (distance < fsm.Ener.CurrentAttackData.range)
					{
						fsm.ChangeState(EnemyFSM.EStateType.ATTACK);
					}
					else
					{
						fsm.ChangeState(EnemyFSM.EStateType.CHASE);
					}
				}
				else
				{
					if (fsm.BehaviorAttr.HasAttribute(EBehaviorAttribute.Patrol))
					{
						fsm.ChangeState(EnemyFSM.EStateType.PATROL);
					}
				}
			}
		}

		public override void OnExit()
		{
		}
	}
}
