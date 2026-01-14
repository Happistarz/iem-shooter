using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    public abstract class WeaponData : ScriptableObject
    {
        [Header("Weapon Stats")]
        public BulletComponent bulletPrefab;

        public int   damage     = 1;
        public float rateOfFire = 1.0f;
        public float speed      = 10.0f;

        public abstract WeaponBehaviour AttachWeapon(GameObject owner, ActorComponent actor);
    }
}