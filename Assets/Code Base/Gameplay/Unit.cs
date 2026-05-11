using Common;
using UnityEngine;

namespace Mecha
{
    public class Unit : DestructibleBase
    {

        [Header("Weapons")]
        [SerializeField] private Turret m_Turret;

        #region Weapons
        public bool DrawEnergy(int energy)
        {
            return true;
        }
        public bool DrawAmmo(int ammo)
        {
            return true;
        }
        public void PointTurretToTarget(Vector3 target)
        {
            m_Turret.transform.LookAt(target);
        }

        public void Fire()
        {
            m_Turret.Fire();
        }

        #endregion
    }
}