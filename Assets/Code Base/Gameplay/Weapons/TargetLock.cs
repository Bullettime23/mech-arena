using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Common;


namespace Mecha
{
    /// <summary>
    /// Когда вражеский корабль входит в сектор прицеливания и остается в нем определенное время
    /// фиксирует цель и издает звуковой сигнал
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class TargetLock : MonoBehaviour
    {
        [SerializeField] private float m_Radius;
        [SerializeField] private float m_Sector;
        [SerializeField] private AudioClip m_Audio;

        [SerializeField] private float m_LockOnDelay;
        private float m_LockOnTimer;

        [SerializeField] private UnityEvent m_EventOnLock;
        public UnityEvent EventOnDeath => m_EventOnLock;

        private float m_AudioTimer;
        private readonly float m_AudioTimerDefault = 5;

        private Unit m_TargetUnit;
        private List<Unit> m_ShipsInRange = new List<Unit>();
        public Unit TargetUnit => m_TargetUnit;


        #region TargetLockSystem
        private void Start()
        {
            m_AudioTimer = m_AudioTimerDefault;
            m_LockOnTimer = m_LockOnDelay;
        }

        private void SetTarget()
        {
            m_TargetUnit = null;

            for (int i = 0; i < m_ShipsInRange.Count; i++)
            {
                // Направление от collision до transform
                Vector2 dir = transform.position - m_ShipsInRange[i].transform.position;

                float angle = 180f - Vector2.Angle(transform.up, dir);

                if (angle <= m_Sector)
                {
                    m_TargetUnit = m_ShipsInRange[i];
                    break;
                }

            }
        }

        private void Update()
        {
            var prevTarget = m_TargetUnit;

            m_LockOnTimer -= Time.deltaTime;
            //Выбирать цель из перечня раз в указанный промежуток времени
            if (m_LockOnTimer <= 0)
            {
                SetTarget();
                m_LockOnTimer = m_LockOnDelay;
            }

            //Когда новая цель, проиграть звуковой эффект
            if (m_TargetUnit != null && m_TargetUnit != prevTarget)
            {
                SoundFXManager.Instance.PlaySoundFXClip(m_Audio, transform, 0.5f);
                m_AudioTimer = m_AudioTimerDefault;
                m_EventOnLock.Invoke();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Unit ship = collision.transform.root.GetComponent<Unit>();

            if (ship != null)
                m_ShipsInRange.Add(ship);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Unit ship = collision.transform.root.GetComponent<Unit>();

            if (ship != null)
                m_ShipsInRange.Remove(ship);
        }
        #endregion


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector3 direction = transform.TransformDirection(Vector3.up) * 100;
            Gizmos.DrawRay(transform.position, direction);
        }

        private void OnValidate()
        {
            GetComponent<CircleCollider2D>().radius = m_Radius;
        }
#endif
    }
}