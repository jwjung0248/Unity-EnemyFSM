using UnityEngine;

namespace Enemy.FSM
{
	public class PatrolState : EnemyState
	{
		Vector3[] waypoints = new Vector3[]
		{
			new Vector3(-8f, 0f, -5f),
			new Vector3(-11f, 0f, -13f),
			new Vector3(-12f, 0f, 2f),
			new Vector3(-1f, 0f, -3f)
		};

		int waypointIndex = 0;

		public PatrolState(EnemyFSM fsm) : base(fsm) { }

		public override void OnEnter()
		{
			fsm.Anim.SetBool(fsm.animHash_IsWalk, true);
			
			fsm.NavAgent.speed = fsm.Ener.OriginData.moveSpeed;
			fsm.NavAgent.ResetPath();
			fsm.NavAgent.SetDestination(waypoints[waypointIndex]);
		}

		public override void OnUpdate()
		{
			if (fsm.NavAgent.remainingDistance <= fsm.NavAgent.stoppingDistance)
			{
				waypointIndex++;

				if(waypointIndex >= waypoints.Length)
				{
					waypointIndex = 0;
				}

				fsm.ChangeState(EnemyFSM.EStateType.IDLE);
			}
			else
			{
				float distance = fsm.Tr.position.DistanceIgnoreYAxis(fsm.TargetTr.position);

				if (distance < fsm.Ener.OriginData.detectionDistance)
				{
					if (distance > fsm.Ener.CurrentAttackData.range)
					{
						fsm.ChangeState(EnemyFSM.EStateType.CHASE);

					}
					else if (distance <= fsm.Ener.CurrentAttackData.range)
					{
						fsm.ChangeState(EnemyFSM.EStateType.ATTACK);
					}
				}
			}
		}

		public override void OnExit()
		{
			fsm.Anim.SetBool(fsm.animHash_IsWalk, false);
			fsm.NavAgent.ResetPath();
		}
	}
}