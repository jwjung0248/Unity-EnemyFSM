using System;
using UnityEngine;
using UnityEngine.AI;
using CustomTools;

namespace Enemy.FSM
{
	public class EnemyFSM : MonoBehaviour
	{
		public enum EStateType
		{
			NONE,
			SPAWN,
			IDLE,
			CHASE,
			PATROL,
			ATTACK,
			HIT,
			DIE
		}

		#region 프로퍼티
		public Transform Tr { get; private set; }
		public Transform TargetTr { get; private set; }
		public Enemy Ener { get; private set; }
		public EBehaviorAttribute BehaviorAttr { get; private set; }
		public Animator Anim { get; private set; }
		public NavMeshAgent NavAgent { get; private set; }
		#endregion

		#region 애니메이터 필수 파라메터 해쉬 값
		[NonSerialized] public readonly int animHash_IsIdle = Animator.StringToHash("isIdle");
		[NonSerialized] public readonly int animHash_IsWalk = Animator.StringToHash("isWalk");
		[NonSerialized] public readonly int animHash_IsRun = Animator.StringToHash("isRun");
		[NonSerialized] public readonly int animHash_IsAttack = Animator.StringToHash("isAttack");
		[NonSerialized] public readonly int animHash_Hit = Animator.StringToHash("Hit");
		[NonSerialized] public readonly int animHash_Die = Animator.StringToHash("Die");
		[NonSerialized] public int[] animHashes_Attack;
		#endregion //애니메이터 파라메터 해쉬 값

		#region public 멤버
		public bool isOnMenualAnimatorMove = false;
		#endregion //public 멤버

		#region private 멤버
		private EnemyState currentState;
		private bool isChangeOver = false;
		private EnumDictionary<EStateType, EnemyState> stateDict = new EnumDictionary<EStateType, EnemyState>();
		#endregion

		#region 유니티 이벤트 메서드
		void Awake()
		{
			//! 컴포넌트 및 데이터 캐싱
			// {
			Tr = transform;
			Ener = GetComponent<Enemy>();
			NavAgent = GetComponent<NavMeshAgent>();
			Anim = GetComponent<Animator>();

			animHashes_Attack = new int[Ener.OriginData.atkDataList.Count];

			for (int i = 0; i < animHashes_Attack.Length; ++i)
			{
				animHashes_Attack[i] = Animator.StringToHash("Attack" + (i + 1).ToString());
			}
			// }

			//! 공통 상태 캐싱
			stateDict.Add(EStateType.IDLE, new IdleState(this));
			stateDict.Add(EStateType.ATTACK, new AttackState(this));
			stateDict.Add(EStateType.HIT, new HitState(this));
			stateDict.Add(EStateType.DIE, new DieState(this));

			//! 특수 상태 캐싱
			InitBehavior();
		}

		private void Start()
		{
			TargetTr = CGameSceneManager.Instance.player.transform;

			//! 플레이어 죽음(게임종료) 이벤트 등록
			CGameSceneManager.Instance.OnPlayerDie += () =>
			{
				TargetTr = null;
				ChangeState(EStateType.IDLE);
			};
		}

		private void OnEnable()
		{
			NavAgent.speed = Ener.OriginData.moveSpeed;
			NavAgent.enabled = true;

			// 상태 시작.
			currentState = stateDict[EStateType.IDLE];
			currentState.OnEnter();
			//isChangeOver = true;
		}
		private void Update()
		{
			currentState.OnUpdate();
		}

		private void OnAnimatorMove()
		{
			if (isOnMenualAnimatorMove)
			{
				var pos = Anim.rootPosition;

				pos.y = NavAgent.nextPosition.y;
				Tr.position = pos;
				NavAgent.nextPosition = Tr.position;
			}
		}
		#endregion     // 유니티 이벤트 메서드

		private void InitBehavior()
		{
			BehaviorAttr = Ener.OriginData.behaviorAttribute;

			if (BehaviorAttr.HasAttribute(EBehaviorAttribute.Chase)) stateDict.Add(EStateType.CHASE, new ChaseState(this));
			if (BehaviorAttr.HasAttribute(EBehaviorAttribute.Patrol)) stateDict.Add(EStateType.PATROL, new PatrolState(this));
			//if (BehaviorAttr.HasAttribute(EBehaviorAttribute.Prowl)) stateDict.Add(EStateType.PROWL, new ProwlState(this));
			//if (BehaviorAttr.HasAttribute(EBehaviorAttribute.Runaway)) stateDict.Add(EStateType.RUNAWAY, new RunAwayState(this));
		}

		public void ChangeState(EStateType newStateType)
		{
			currentState.OnExit();

			EnemyState newState = stateDict[newStateType];
			currentState = newState;

			currentState.OnEnter();
		}

		private void OnGUI()
		{
			// State 표시
			var pos = Tr.position.SetY(Tr.position.y + 2.5f);
			var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
			screenPos.y = Screen.height - screenPos.y;
			screenPos.x -= 300f / 2f;

			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.white;
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 40;
			style.alignment = TextAnchor.MiddleCenter;
			
			GUI.Label(new Rect(screenPos, new Vector2(300f, 100f)), currentState.GetType().Name, style);
		}

		private void OnDrawGizmosSelected()
		{
			// 감지 거리 표시
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(Tr.position, Ener.OriginData.detectionDistance);

			// 현재 공격 범위 표시
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(Tr.position, Ener.CurrentAttackData.range);
		}
	}
}

