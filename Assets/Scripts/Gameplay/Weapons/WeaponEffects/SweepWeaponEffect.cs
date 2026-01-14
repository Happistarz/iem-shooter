using System;
using UnityEngine;

namespace Weapons
{
    public class SweepWeaponEffect : IWeaponEffect
    {
        public ActorComponent Owner { get; set; }
        public event Action OnShoot;

        private readonly BulletComponent _prefab;
        private readonly float _bulletSpeed;
        private readonly float _rateOfFire;
        private readonly float _rotationSpeed;

        private float _nextShotDelay;

        public SweepWeaponEffect(BulletComponent prefab, float bulletSpeed, float rateOfFire,
            float rotationSpeed)
        {
            _prefab = prefab;
            _bulletSpeed = bulletSpeed;
            _rateOfFire = rateOfFire;
            _rotationSpeed = rotationSpeed;
        }

        public void Update(Vector3 origin, Vector3 direction)
        {
            var delayBetweenShots = 1.0f / _rateOfFire;
            _nextShotDelay += Time.deltaTime;
            if (!(_nextShotDelay > delayBetweenShots)) return;
            
            var angle          = Time.time * _rotationSpeed;
            var shootDirection = Rotate(Vector3.forward, angle);

            var pool   = Game.GetBulletPool(_prefab);
            var bullet = pool.Get();
            bullet.prefab = _prefab;
            
            bullet.Reset();
            bullet.transform.position = origin;
            bullet.Velocity           = shootDirection * _bulletSpeed;
            bullet.Owner              = Owner;

            _nextShotDelay = 0;
            OnShoot?.Invoke();
        }

        private static Vector3 Rotate(Vector3 v, float angleRadians)
        {
            var q = Quaternion.AngleAxis(Mathf.Rad2Deg * angleRadians, Vector3.up);
            return q * v;
        }
    }
}