using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Weapons
{
    public class SweepWeaponEffect : IWeaponEffect
    {
        public ActorComponent Owner { get; set; }
        public event Action OnShoot;

        private readonly BulletComponent m_prefab;
        private readonly float m_bulletSpeed;
        private readonly float m_rateOfFire;
        private readonly float m_rotationSpeed;

        private float m_nextShotDelay;

        public SweepWeaponEffect(BulletComponent prefab, float bulletSpeed, float rateOfFire,
            float rotationSpeed)
        {
            m_prefab = prefab;
            m_bulletSpeed = bulletSpeed;
            m_rateOfFire = rateOfFire;
            m_rotationSpeed = rotationSpeed;
        }

        public void Update(Vector3 origin, Vector3 direction)
        {
            var delayBetweenShots = 1.0f / m_rateOfFire;
            m_nextShotDelay += Time.deltaTime;
            if (m_nextShotDelay > delayBetweenShots)
            {
                var angle = Time.time * m_rotationSpeed;
                var shootDirection = Rotate(Vector3.forward, angle);
            
                var bullet = Object.Instantiate(m_prefab);
                bullet.transform.position = origin;
                bullet.Velocity = shootDirection * m_bulletSpeed;
                bullet.Owner = Owner;

                m_nextShotDelay = 0;
                OnShoot?.Invoke();
            }
        }

        public Vector3 Rotate(Vector3 v, float angleRadians)
        {
            var q = Quaternion.AngleAxis(Mathf.Rad2Deg * angleRadians, Vector3.up);
            return q * v;
        }
    }
}