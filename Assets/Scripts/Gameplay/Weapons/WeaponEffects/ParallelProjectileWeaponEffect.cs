using System;
using UnityEngine;

namespace Weapons
{
    public class ParallelProjectileWeaponEffect : IWeaponEffect
    {
        public ActorComponent Owner { get; set; }
        public event Action OnShoot;

        private const float _OFFSET = 0.45f;

        private readonly BulletComponent _prefab;
        private readonly int _multiShotCount;
        private readonly float _speed;
        private readonly float _rateOfFire;

        private float _nextShotDelay;

        public ParallelProjectileWeaponEffect(BulletComponent prefab, int multiShotCount, float speed, float rateOfFire)
        {
            _prefab = prefab;
            _multiShotCount = multiShotCount;
            _speed = speed;
            _rateOfFire = rateOfFire;
        }

        public void Update(Vector3 origin, Vector3 direction)
        {
            var delayBetweenShots = 1.0f / _rateOfFire;
            _nextShotDelay += Time.deltaTime;
            if (!(_nextShotDelay > delayBetweenShots) || direction == Vector3.zero) return;
            
            Shoot(origin, direction);
            _nextShotDelay = 0;
        }

        private void Shoot(Vector3 origin, Vector3 direction)
        {
            var sideDirection = Vector3.Cross(Vector3.up, direction);
            var minOffset = _OFFSET * _multiShotCount / 2;
            var maxOffset = -minOffset;

            for (var i = 0; i < _multiShotCount; i++)
            {
                float offset = 0;
                if (_multiShotCount > 1)
                    offset = minOffset + (maxOffset - minOffset) * i / (float)(_multiShotCount - 1);

                var bullet = Game.BulletPrefabPool.Get();
                bullet.transform.position = origin + offset * sideDirection;
                bullet.Velocity = direction * _speed;
                bullet.Owner = Owner;
            }
            
            OnShoot?.Invoke();
        }
    }
}