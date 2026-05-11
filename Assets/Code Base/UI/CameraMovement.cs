using System;
using Infrastructure;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mecha
{
    /// <summary>
    /// Камера перемещается будет перемещаться перед игроком, перед его указателем мыши
    /// Отдалять можно через Size
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraMovement : Singleton<CameraMovement>
    {
        [SerializeField] private float m_Speed;
        [SerializeField] private float m_SpeedAngular;
        [SerializeField] private GameObject m_Player;
        [SerializeField] private PlayerInput m_PlayerInput;


        private InputAction m_CameraRotationAction;


        #region Unity Events
        private void Update()
        {
            // TODO: Камера теряет игрока, если вращать её одновременно с движением
            //if (m_CurrentRotation != 0)
            //{
            //    SetRotatedCameraPosition(m_CurrentRotation * m_SpeedAngular *  Time.deltaTime);
            //}

            //TODO: Можно перемещать камеру только тогда, когда движется игрок или игрок шевелит курсором
            Vector3 nextPosition = CalculateNextCameraPosition();
            if (nextPosition != m_Camera.transform.position)
            {
                m_Camera.transform.position = nextPosition;
            }
        }

        private void Start()
        {
            m_Camera = GetComponent<Camera>();
            m_InitialPosition = m_Camera.transform.position;
            //m_CameraRotationAction = m_PlayerInput.actions["Camera turn"];

            //m_CameraRotationAction.performed += OnRotateCamera;
            //m_CameraRotationAction.canceled += OnRotateCamera;
        }

        //private void OnDestroy()
        //{
        //    m_CameraRotationAction.performed -= OnRotateCamera;
        //}
        #endregion


        private Camera m_Camera;
        private Vector3 m_InitialPosition;

        private Vector3 CalculateNextCameraPosition()
        {
            return m_Player.transform.position + m_InitialPosition;
        }



        // Player rotates the camera
        /*
         Чем располагаю:
        Позиция камеры
        Поворот камеры
        Приближение камеры
        Положение модельки игрока

        Камера должна располагаться на постоянном расстоянии от игрока. Начальное положение есть
        Нужно получить конечное положение по углу поворота игроком, положению модельки, и начальному положению
         */

        private float m_CurrentRotation;
        private void OnRotateCamera(InputAction.CallbackContext context)
        {
            m_CurrentRotation = context.ReadValue<Vector2>().x;
        }

        private void SetRotatedCameraPosition(float eulerAngle)
        {
            Vector3 direction = m_Camera.transform.position - m_Player.transform.position;
            Vector3 cameraPosition = Quaternion.AngleAxis(eulerAngle, Vector3.up) * direction;
            m_Camera.transform.rotation = Quaternion.LookRotation(m_Player.transform.position - cameraPosition, Vector3.up);
            m_InitialPosition = cameraPosition;
        }

        /*
        [SerializeField] private PlayerInput m_PlayerInput;
        [SerializeField] private DefaultCamera m_DefaultCamera;
        [SerializeField] private MouseSensitivity m_MouseSensitivity;
        [SerializeField] private CameraAngle m_CameraAngle;

        private CameraRotation m_CameraRotation;

        private Vector2 m_CameraTurn;
        private Vector2 m_MouseLook;
        private Vector2 m_MouseWheel;

        private InputAction m_InputWASD;
        private InputAction m_InputMouseWheel;
        private InputAction m_InputCameraRotation;

        private Vector3 m_MovePosition;
        private CameraRotation m_RotationTarget;
        private bool m_SholdMove = false;

        private void Look(InputAction.CallbackContext context)
        {
            m_MouseLook = context.ReadValue<Vector2>();
        }

        private void Zoom(InputAction.CallbackContext context)
        {
            m_MouseWheel = context.ReadValue<Vector2>();
        }

        private void CameraMove(InputAction.CallbackContext context)
        {
            m_Rotation = context.ReadValue<Vector2>();
        }

        public void FocusOn(GameObject target)
        {
            m_MovePosition = target.transform.position + new Vector3(m_DefaultCamera.x, m_DefaultCamera.y, m_DefaultCamera.z);
            m_RotationTarget = m_DefaultCamera.rotation;
            m_SholdMove = true;
        }

        public void FocusOn(Vector3 position)
        {
            m_MovePosition = position + new Vector3(m_DefaultCamera.x, m_DefaultCamera.y, m_DefaultCamera.z);
            m_RotationTarget = m_DefaultCamera.rotation;
            m_SholdMove = true;
        }

        private void Start()
        {
            m_Camera = GetComponent<Camera>();

            m_InputCameraRotation = m_PlayerInput.actions["Look"];
            m_InputMouseWheel = m_PlayerInput.actions["CameraZoom"];
            m_InputWASD = m_PlayerInput.actions["Move"];

            m_InputWASD.performed += CameraMove;
            m_InputWASD.canceled += CameraStop;
            m_InputCameraRotation.performed += Look;
            m_InputMouseWheel.performed += Zoom;
        }

        private void OnDestroy()
        {
            m_InputCameraRotation.performed -= Look;
            m_InputWASD.performed -= CameraMove;
            m_InputWASD.canceled -= CameraStop;
            m_InputMouseWheel.performed -= Zoom;
        }


        // Update is called once per frame
        void Update()
        {
            if (m_SholdMove)
            {
                MoveCameraToPostion();
                return;
            }

            // Инверсия
            m_Camera.orthographicSize -= m_MouseWheel.y * m_MouseSensitivity.zoom * Time.deltaTime;

            m_Camera.orthographicSize = Math.Clamp(m_Camera.orthographicSize, 10f, 20.0f);


            m_Camera.transform.position += m_Camera.transform.rotation * new Vector3(m_WASD.x, m_WASD.y, 0) * m_Speed * Time.deltaTime;

            if (m_MouseLook.x != 0 || m_MouseLook.y != 0)
            {
                m_CameraRotation.Yaw += m_MouseLook.x * m_MouseSensitivity.horizontal * Time.deltaTime;
                // Инверсия
                m_CameraRotation.Pitch -= m_MouseLook.y * m_MouseSensitivity.vertical * Time.deltaTime;

                m_CameraRotation.Pitch = Mathf.Clamp(m_CameraRotation.Pitch, m_CameraAngle.min, m_CameraAngle.max);

                m_Camera.transform.eulerAngles = new Vector3(m_CameraRotation.Pitch, m_CameraRotation.Yaw);
            }

        }

        private void MoveCameraToPostion()
        {
            if ((m_MovePosition - m_Camera.transform.position).magnitude <= 0.5)
            {
                m_Camera.transform.position = m_MovePosition;
            }
            if (m_Camera.transform.position == m_MovePosition)
            {
                m_SholdMove = false;
                return;
            }
            m_Camera.transform.position = Vector3.Slerp(m_Camera.transform.position, m_MovePosition, m_Speed / 2 * Time.deltaTime);
            m_Camera.orthographicSize = Mathf.SmoothStep(m_Camera.orthographicSize, m_DefaultCamera.orthographicSize, m_MouseSensitivity.zoom * Time.deltaTime);
            m_Camera.transform.eulerAngles = new Vector3(m_RotationTarget.Pitch, m_RotationTarget.Yaw);
            m_CameraRotation = m_RotationTarget;
        }
        */
    }

    [Serializable]
    public struct MouseSensitivity
    {
        public float horizontal;
        public float vertical;
        public float zoom;
    }

    [Serializable]
    public struct CameraRotation
    {
        // Around X
        public float Pitch;
        // Around Y
        public float Yaw;
        // No Roll around Z
    }

    [Serializable]
    public struct DefaultCamera
    {
        public float x;
        public float y;
        public float z;
        public float orthographicSize;

        public CameraRotation rotation;
    }

    [Serializable]
    public struct CameraAngle
    {
        public float min;
        public float max;
    }

}