using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mecha
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        private const float m_CameraEulerAroundZ = 45;
        private Quaternion cameraAngleMultiplier = Quaternion.AngleAxis(m_CameraEulerAroundZ, Vector3.forward);

        [SerializeField] private float m_MoveSpeed = 1f;
        [Range(0.1f, 1f)]
        [SerializeField] private float m_RotationSpeed = 1f;

        private CharacterController m_CharacterController;
        private Vector2 m_MoveDirection;

        private PlayerInput m_PlayerInput;
        private void ActionMove(InputAction.CallbackContext context)
        {
            m_MoveDirection = context.ReadValue<Vector2>();
            if (m_MoveDirection != Vector2.zero)
            {
                m_LastMoveDirection = m_MoveDirection;
            }
        }

        [Header("Dash")]

        [SerializeField] private float m_DashSpeed = 5000;
        [SerializeField] private float m_DashDuration = 0.5f;
        [SerializeField] private float m_DashMovementSlow = 10;

        private Vector2 m_LastMoveDirection;
        private bool m_IsDashing;
        private void ActionJump(InputAction.CallbackContext context)
        {
            m_IsDashing = true;
            // Нужно быстро переместить модель в направлении движения на фикисрованное расстояние
            // Когда игрок нажимает на дэш, я запускаю корутину, которая двигает игрока в с большой скоростью в течение заданного времени
            StartCoroutine(ApplyDash());
        }

        private IEnumerator ApplyDash()
        {
            Vector2 angledDash =  cameraAngleMultiplier * m_LastMoveDirection * m_DashSpeed;
            m_CharacterController.SimpleMove(new Vector3(angledDash.x, 0, angledDash.y));

            yield return new WaitForSeconds(m_DashDuration);
            m_IsDashing = false;
            // Затем управление возвращается движущемуся игроку
        }


        #region Unity Actions
        // Update is called once per frame
        void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_PlayerInput = GetComponent<PlayerInput>();
            m_PlayerInput.actions["Move"].performed += ActionMove;
            m_PlayerInput.actions["Move"].canceled += ActionMove;

            m_PlayerInput.actions["Jump"].performed += ActionJump;
        }

        private Vector3 m_MoveDirectionXZ = Vector3.zero;
        [Range(0, 1)]
        private float m_RotationDone;
        private void FixedUpdate()
        {
            if (m_MoveDirection != Vector2.zero)
            {

                Vector2 positionDelta = cameraAngleMultiplier * m_MoveDirection * m_MoveSpeed * Time.fixedDeltaTime;

                if (m_IsDashing) positionDelta /= m_DashMovementSlow;
                m_MoveDirectionXZ.x = positionDelta.x;
                m_MoveDirectionXZ.z = positionDelta.y;

                if (transform.rotation != Quaternion.LookRotation(m_MoveDirectionXZ))
                {
                    transform.rotation = Quaternion.LookRotation(m_MoveDirectionXZ * Time.fixedDeltaTime * m_RotationSpeed, Vector3.up);
                    return;
                }
                m_CharacterController.SimpleMove(m_MoveDirectionXZ);
            }
        }
        private void OnDestroy()
        {
            m_PlayerInput.actions["Move"].performed -= ActionMove;
            m_PlayerInput.actions["Move"].canceled -= ActionMove;

            m_PlayerInput.actions["Jump"].performed -= ActionJump;
        }
        #endregion
    }

}
