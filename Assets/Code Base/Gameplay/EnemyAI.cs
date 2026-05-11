using Common;
using Infrastructure;
using Mehca;
using UnityEngine;
using UnityEngine.AI;

namespace Mecha
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Unit))]
    public class EnemyAI : MonoBehaviour
    {

        [SerializeField] private float m_ShootingRange;
        [SerializeField] private float maxDistance = 300f;
        [SerializeField] private Vector3 boxSizeMultiplier = Vector3.one * 0.5f;
        [SerializeField] private Collider col;

        public float PathUpdateTime = 1f;

        // Можно вынести в enemyProps scriptable object

        public float AimingTime = 0.5f;

        private RaycastHit hit;
        public bool IsCanLandShot;


        [HideInInspector] public Unit Player;
        [HideInInspector] public StateMachine UnitStateMachine;
        [HideInInspector] public NavMeshAgent Agent;
        [HideInInspector] public Unit SelfUnit;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Agent = GetComponent<NavMeshAgent>();
            SelfUnit = GetComponent<Unit>();
            UnitStateMachine = new StateMachine();
            UnitStateMachine.ChangeState(new SearchPlayerState(this));
            Player = PlayerSingleton.Instance.GetComponent<Unit>();
        }

        private void Update()
        {
            UnitStateMachine.Update();
        }


        void FixedUpdate()
        {
            Vector3 halfExtents = Vector3.Scale(transform.localScale, boxSizeMultiplier);
            Vector3 origin = col.bounds.center;
            Vector3 direction = (Player.transform.position - transform.position).normalized;

            //Test to see if there is a hit using a BoxCast
            //Calculate using the center of the GameObject's Collider(could also just use the GameObject's position), half the GameObject's size, the direction, the GameObject's rotation, and the maximum distance as variables.
            //Also fetch the hit data
            bool hitDetected = Physics.BoxCast(
                origin,
                halfExtents,
                direction,
                out hit,
                transform.rotation,
                maxDistance);

            if (hitDetected) { 
                print($"Can hit  {hit.transform?.parent?.name}");
                IsCanLandShot = hitDetected && hit.transform?.parent?.gameObject == Player.gameObject;
                return;
            }

            IsCanLandShot = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 directionNormalized = (Player.transform.position - transform.position).normalized;
            if (IsCanLandShot)
            {
                //Draw a Ray forward from GameObject toward the hit
                Gizmos.DrawRay(transform.position, directionNormalized * hit.distance);
                //Draw a cube that extends to where the hit exists
                Gizmos.DrawWireCube(transform.position + directionNormalized * hit.distance, transform.localScale);
            }
            else
            {
                //Draw a Ray forward from GameObject toward the maximum distance
                Gizmos.DrawRay(transform.position, directionNormalized * maxDistance);
                //Draw a cube at the maximum distance
                Gizmos.DrawWireCube(transform.position + directionNormalized * maxDistance, transform.localScale);
            }
        }
#endif
    }

    public class SearchPlayerState : IState
    {
        private EnemyAI m_Owner;

        private Timer m_PathUpdateTimer;

        public SearchPlayerState(EnemyAI owner)
        {
            m_Owner = owner;
        }

        public void Enter()
        {
            // Входя в положение, инициализирует таймер
            m_PathUpdateTimer = new Timer(m_Owner.PathUpdateTime);
        }

        public void Execute()
        {
       
            // По таймеру обновляет положение игрока и идет к нему, пока игрок не будет в зоне видимости и радиусе выстрела
            m_PathUpdateTimer.RemoveTime(Time.deltaTime);
            if (m_PathUpdateTimer.IsFinish)
            {
                
                m_Owner.Agent.SetDestination(m_Owner.Player.transform.position);
                m_Owner.Agent.isStopped = false;

                if (m_Owner.IsCanLandShot)
                {
                    m_Owner.UnitStateMachine.ChangeState(new ShootingState(m_Owner));
                    return;
                }
                m_PathUpdateTimer.Start(m_Owner.PathUpdateTime);
            }
        }

        public void Exit()
        {
            // Приготовиться к выстрелу!
            m_Owner.Agent.isStopped = true;
            Debug.Log("Exiting Search state");
           
        }
    }

    public class ShootingState : IState
    {
        private EnemyAI m_Owner;

        private Timer m_AimTimer;

        public ShootingState(EnemyAI owner)
        {
            m_Owner = owner;
        }


        public void Enter()
        {
            m_AimTimer = new Timer(m_Owner.AimingTime);
        }

        public void Execute()
        {
            m_Owner.SelfUnit.PointTurretToTarget(m_Owner.Player.transform.position);

            m_AimTimer.RemoveTime(Time.deltaTime);

            if (m_AimTimer.IsFinish)
            {
                m_Owner.SelfUnit.Fire();
                m_Owner.UnitStateMachine.ChangeState(new SearchPlayerState(m_Owner));
                Exit();
            }
        }

        public void Exit()
        {
            // Вернуться к поиску цели
            Debug.Log("Exiting Firing state");
        }
    }
}
