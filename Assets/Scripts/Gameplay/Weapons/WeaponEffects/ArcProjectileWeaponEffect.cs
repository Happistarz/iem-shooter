using UnityEngine;

namespace Weapons
{
    public class ArcProjectileWeaponEffect : IWeaponEffect
    {
        public ActorComponent Owner { get; set; }

        private readonly BulletComponent m_prefab;
        private readonly int m_multiShotCount;
        private readonly float m_speed;
        private readonly float m_rateOfFire;

        private float m_nextShotDelay = 0;

        public ArcProjectileWeaponEffect(BulletComponent prefab, int multiShotCount, float speed, float rateOfFire)
        {
            m_prefab = prefab;
            m_multiShotCount = multiShotCount;
            m_speed = speed;
            m_rateOfFire = rateOfFire;
        }

        public void Update(Vector3 origin, Vector3 direction)
        {
            float delayBetweenShots = 1.0f / m_rateOfFire;
            m_nextShotDelay += Time.deltaTime;
            if (m_nextShotDelay > delayBetweenShots && direction != Vector3.zero)
            {
                Shoot(origin, direction);
                m_nextShotDelay = 0;
            }
        }

        private void Shoot(Vector3 origin, Vector3 direction)
        {
            float arcAngle = 20;
            float angleOffset = arcAngle / (m_multiShotCount + 1);
            float angle = -0.5f * arcAngle + angleOffset;

            for (int i = 0; i < m_multiShotCount; i++)
            {
                BulletComponent bullet = GameObject.Instantiate<BulletComponent>(m_prefab);
                bullet.transform.position = origin;
                bullet.Velocity = Quaternion.AngleAxis(angle, Vector3.up) * direction * m_speed;
                bullet.Owner = Owner;
                angle += angleOffset;
            }
        }
    }
}