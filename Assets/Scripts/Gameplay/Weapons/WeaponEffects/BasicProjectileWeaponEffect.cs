using System;
using UnityEngine;

namespace Weapons
{
    public class BasicProjectileWeaponEffect : IWeaponEffect
    {
        public ActorComponent Owner { get; set; }
        public event Action OnShoot;

        private readonly BulletComponent _prefab;
        private readonly float _speed;
        private readonly float _rateOfFire;

        private float _nextShotDelay;

        public BasicProjectileWeaponEffect(BulletComponent prefab, float speed, float rateOfFire)
        {
            _prefab = prefab;
            _speed = speed;
            _rateOfFire = rateOfFire;
        }

        public void Update(Vector3 origin, Vector3 direction)
        {
            var delayBetweenShots = 1.0f / _rateOfFire;
            _nextShotDelay += Time.deltaTime;
            if (_nextShotDelay > delayBetweenShots && direction != Vector3.zero)
            {
                Shoot(origin, direction);
                _nextShotDelay = 0;
            }
        }

        private void Shoot(Vector3 origin, Vector3 direction)
        {
            var bullet = Game.BulletPrefabPool.Get();
            bullet.transform.position = origin;
            bullet.Velocity = direction * _speed;
            bullet.Owner = Owner;
            
            OnShoot?.Invoke();
        }
    }
}