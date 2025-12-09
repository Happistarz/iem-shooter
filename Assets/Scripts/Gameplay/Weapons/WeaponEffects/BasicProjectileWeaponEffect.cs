using UnityEngine;

namespace Weapons
{
    public class BasicProjectileWeaponEffect : IWeaponEffect
    {
        public ActorComponent Owner { get; set; }

        private readonly BulletComponent m_prefab;
        private readonly float m_speed;
        private readonly float m_rateOfFire;

        private float m_nextShotDelay = 0;

        public BasicProjectileWeaponEffect(BulletComponent prefab, float speed, float rateOfFire)
        {
            m_prefab = prefab;
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
            BulletComponent bullet = GameObject.Instantiate<BulletComponent>(m_prefab);
            bullet.transform.position = origin;
            bullet.Velocity = direction * m_speed;
            bullet.Owner = Owner;
        }
    }
}