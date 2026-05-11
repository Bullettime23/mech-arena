using Common;
using UnityEngine;


namespace Mecha
{
    public class Projectile : ProjectileBase
    {
        protected override void OnHit(DestructibleBase destr)
        {
            //Абстрактный метод переопределяется
            //if (m_Parent == Player.Instance.ActiveShip)
            //{
            //    if (destr is Destructible && destr.CurrentHitPoints <= 0)

            //        Player.Instance.AddScore(((Destructible)destr).ScoreValue);

            //    if (destr is SpaceShip && destr.CurrentHitPoints <= 0)
            //        Player.Instance.AddKill();
            //}
        }


        protected override void OnProjectileLifetimeEnd(Collider collider, Vector3 point)
        {
            if (m_ImpactEffectPrefab != null)
            {
                //ExplosionController.Instance.PlayEffect(m_ImpactEffectPrefab, transform);
            }
            Destroy(gameObject, 0);
        }
    }
}