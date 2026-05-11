using UnityEngine;
using UnityEngine.Audio;


namespace Mecha
{
    public enum TurretMode
    {
        Primary,
        Secondary
    }
    [CreateAssetMenu(fileName = "TurretProperties", menuName = "Scriptable objects/Turret properties")]
    public sealed class TurretProperties : ScriptableObject
    {
        public TurretMode Mode;
        public Projectile ProjectilePrefab;
        public float FireRate;
        public int EnergyUsage;
        public int AmmoUsage;
        public AudioResource LaunchSFX;
    }
}