using System;
using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        public    WeaponData     data;
        protected ActorComponent Actor;

        public event Action OnShoot;

        private float _lastFireTime;

        public virtual void Initialize(WeaponData weaponData, ActorComponent actor)
        {
            data  = weaponData;
            Actor = actor;
        }

        public virtual void UpdateWeapon(Vector3 origin, Vector3 direction)
        {
            if (!data || !Actor) return;

            if (!(Time.time - _lastFireTime >= 1.0f / data.rateOfFire)) return;
            if (direction == Vector3.zero) return;

            Shoot(origin, direction);
            OnShoot?.Invoke();
            _lastFireTime = Time.time;
        }

        protected abstract void Shoot(Vector3 origin, Vector3 direction);

        protected BulletComponent SpawnBullet(Vector3 position, Vector3 velocity, Quaternion rotation)
        {
            var pool   = Game.GetBulletPool(data.bulletPrefab);
            var bullet = pool.Get();
            bullet.prefab = data.bulletPrefab;
            bullet.Reset();

            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            bullet.Velocity           = velocity;
            bullet.Damage             = data.damage;
            bullet.Owner              = Actor;

            return bullet;
        }
    }
}