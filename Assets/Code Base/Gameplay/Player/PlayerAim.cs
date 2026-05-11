using UnityEngine;
using UnityEngine.InputSystem;

namespace Mecha
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerAim : MonoBehaviour
    {
        #region Attack

        [SerializeField] private LayerMask m_LayerMask;

        private Unit m_Unit;
        private Camera m_Camera;
        private InputAction m_AttackAction;

        public Vector3 PointerPosition;
        public Vector3 PointerPositionToWorld;
        private void ActionAttack(InputAction.CallbackContext context)
        {
            m_Unit.Fire();
        }

        #endregion

        #region Unity Events
        private void Start()
        {
            m_Camera = Camera.main;
            m_Unit = GetComponent<Unit>();
            m_AttackAction = GetComponent<PlayerInput>().actions["Attack"];
            m_AttackAction.started += ActionAttack;
        }

        private void Update()
        {
            PointerPosition = Mouse.current.position.ReadValue();
            Ray pointerRay = m_Camera.ScreenPointToRay(PointerPosition);
            if (Physics.Raycast(pointerRay, out RaycastHit hitData,  m_Camera.farClipPlane, m_LayerMask))
            {
                PointerPositionToWorld = hitData.point;
            }

            // Турель всегда должна смотреть всегда в направлении курсора, параллельно поверхности ( Придется доработать, если будет ландшафт)

            Vector3 target = new Vector3(PointerPositionToWorld.x, m_Unit.transform.position.y, PointerPositionToWorld.z);

            m_Unit.PointTurretToTarget(target);
        }

        #endregion


#if UNITY_EDITOR

        private Color gizmoColor = Color.indianRed;
        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(PointerPositionToWorld, 1f);
        }
#endif
    }
}