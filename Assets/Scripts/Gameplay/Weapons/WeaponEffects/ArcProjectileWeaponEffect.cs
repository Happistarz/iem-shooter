using System;
using UnityEngine;

namespace Weapons
{
    public class ArcProjectileWeaponEffect : IWeaponEffect
    {
        public ActorComponent Owner { get; set; }
        public event Action   OnShoot;

        private readonly BulletComponent _prefab;
        private readonly int             _multiShotCount;
        private readonly float           _speed;
        private readonly float           _rateOfFire;

        private float _nextShotDelay;

        public ArcProjectileWeaponEffect(BulletComponent prefab, int multiShotCount, float speed, float rateOfFire)
        {
            _prefab         = prefab;
            _multiShotCount = multiShotCount;
            _speed          = speed;
            _rateOfFire     = rateOfFire;
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
            const float ARC_ANGLE = 20;

            var angleOffset = ARC_ANGLE / (_multiShotCount + 1);
            var angle       = -0.5f * ARC_ANGLE + angleOffset;

            for (var i = 0; i < _multiShotCount; i++)
            {
                var pool   = Game.GetBulletPool(_prefab);
                var bullet = pool.Get();
                bullet.prefab = _prefab;

                bullet.Reset();
                bullet.transform.position = origin;

                bullet.Velocity = Quaternion.AngleAxis(angle, Vector3.up) * direction * _speed;

                bullet.Owner =  Owner;
                angle        += angleOffset;
            }

            OnShoot?.Invoke();
        }
    }
}