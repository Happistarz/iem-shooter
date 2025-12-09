using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Weapons;

public class RainbowPulseWeaponEffect : IWeaponEffect
{
    public ActorComponent Owner { get; set; }

    private readonly BulletComponent m_prefab;
    private readonly float m_bulletSpeed;
    private readonly float m_rateOfFire;
    private readonly float m_rotationSpeed;
    private readonly float m_gradientDuration;
    private readonly Gradient m_gradient;

    private float m_nextShotDelay = 0;

    public RainbowPulseWeaponEffect(BulletComponent prefab, float bulletSpeed, float rateOfFire, float rotationSpeed, Gradient gradient, float gradientDuration)
    {
        m_prefab = prefab;
        m_bulletSpeed = bulletSpeed;
        m_rateOfFire = rateOfFire;
        m_rotationSpeed = rotationSpeed;
        m_gradient = gradient;
        m_gradientDuration = gradientDuration;
    }

    public void Update(Vector3 origin, Vector3 direction)
    {
        float delayBetweenShots = 1.0f / m_rateOfFire;
        m_nextShotDelay += Time.deltaTime;
        if (m_nextShotDelay > delayBetweenShots)
        {
            float angle = Time.time * m_rotationSpeed;
            Vector3 shootDirection = Rotate(Vector3.forward, angle);

            BulletComponent bullet = GameObject.Instantiate<BulletComponent>(m_prefab);
            bullet.transform.position = origin;
            bullet.Velocity = shootDirection * m_bulletSpeed;
            bullet.Owner = Owner;
            
            //Change projectile size
            bullet.transform.localScale *= 2;

            //Add color variations
            var colorGradientCmp = bullet.AddComponent<ColorGradientComponent>();
            colorGradientCmp.Gradient = m_gradient;
            colorGradientCmp.Duration = m_gradientDuration;
            colorGradientCmp.TimeOffset = angle;

            m_nextShotDelay = 0;
        }
    }

    public Vector3 Rotate(Vector3 v, float angleRadians)
    {
        Quaternion q = Quaternion.AngleAxis(Mathf.Rad2Deg * angleRadians, Vector3.up);
        return q * v;
    }
}