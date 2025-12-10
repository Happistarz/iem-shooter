using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Weapons
{
    public class ParallelProjectileWeaponEffect : IWeaponEffect
    {
        public ActorComponent Owner { get; set; }
        public event Action OnShoot;

        public const float Offset = 0.45f;

        private readonly BulletComponent m_prefab;
        private readonly int m_multiShotCount;
        private readonly float m_speed;
        private readonly float m_rateOfFire;

        private float m_nextShotDelay;

        public ParallelProjectileWeaponEffect(BulletComponent prefab, int multiShotCount, float speed, float rateOfFire)
        {
            m_prefab = prefab;
            m_multiShotCount = multiShotCount;
            m_speed = speed;
            m_rateOfFire = rateOfFire;
        }

        public void Update(Vector3 origin, Vector3 direction)
        {
            var delayBetweenShots = 1.0f / m_rateOfFire;
            m_nextShotDelay += Time.deltaTime;
            if (m_nextShotDelay > delayBetweenShots && direction != Vector3.zero)
            {
                Shoot(origin, direction);
                m_nextShotDelay = 0;
            }
        }

        private void Shoot(Vector3 origin, Vector3 direction)
        {
            var sideDirection = Vector3.Cross(Vector3.up, direction);
            var minOffset = Offset * m_multiShotCount / 2;
            var maxOffset = -minOffset;

            for (var i = 0; i < m_multiShotCount; i++)
            {
                float offset = 0;
                if (m_multiShotCount > 1)
                    offset = minOffset + (maxOffset - minOffset) * i / (float)(m_multiShotCount - 1);

                var bullet = Object.Instantiate(m_prefab);
                bullet.transform.position = origin + offset * sideDirection;
                bullet.Velocity = direction * m_speed;
                bullet.Owner = Owner;
            }
            
            OnShoot?.Invoke();
        }
    }
}