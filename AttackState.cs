
namespace Enemy.FSM
{
	public class AttackState : EnemyState
	{
		public AttackState(EnemyFSM fsm) : base(fsm) { }

		public override void OnEnter()
		{
			if (fsm.Ener.CurrentAttackData.isRootAnimation)
			{
				fsm.NavAgent.updatePosition = false;
				fsm.isOnMenualAnimatorMove = true;
			}

			fsm.Tr.LookAtIgnoreYAxis(fsm.TargetTr.position);
			fsm.Anim.SetBool(fsm.animHash_IsAttack, true);
			fsm.Anim.SetTrigger(fsm.animHashes_Attack[fsm.Ener.CurrentAttackData.index]);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			int currentAnimHash = fsm.Anim.GetCurrentAnimatorStateInfo(0).shortNameHash;
			int currentAttackHash = fsm.animHashes_Attack[fsm.Ener.CurrentAttackData.index];

			if (currentAnimHash == currentAttackHash)
			{
				if (fsm.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
					&& !fsm.Anim.IsInTransition(0))
				{
					fsm.ChangeState(EnemyFSM.EStateType.IDLE);
				}
			}
		}


		public override void OnExit()
		{
			fsm.NavAgent.updatePosition = true;
			fsm.isOnMenualAnimatorMove = false;

			fsm.Ener.SetAttackData();
			fsm.Anim.SetBool(fsm.animHash_IsAttack, false);
		}
	}
}