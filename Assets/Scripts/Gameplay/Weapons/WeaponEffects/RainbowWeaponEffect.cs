using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Weapons;

public class RainbowWeaponEffect : IWeaponEffect
{
    public ActorComponent Owner { get; set; }

    private readonly BulletComponent m_prefab;
    private readonly float m_bulletSpeed;
    private readonly float m_rateOfFire;
    private readonly float m_rotationSpeed;
    private readonly Gradient m_gradient;

    private float m_nextShotDelay = 0;

    public RainbowWeaponEffect(BulletComponent prefab, float bulletSpeed, float rateOfFire, float rotationSpeed, Gradient gradient)
    {
        m_prefab = prefab;
        m_bulletSpeed = bulletSpeed;
        m_rateOfFire = rateOfFire;
        m_rotationSpeed = rotationSpeed;
        m_gradient = gradient;
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
            
            bullet.transform.localScale *= 2;
            
            MeshRenderer renderer = bullet.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                var gradientTime = (angle % 2 * Mathf.PI) / (2 * Mathf.PI);

                var material = renderer.material;
                material.color = m_gradient.Evaluate(gradientTime);
            }

            m_nextShotDelay = 0;
        }
    }

    public Vector3 Rotate(Vector3 v, float angleRadians)
    {
        Quaternion q = Quaternion.AngleAxis(Mathf.Rad2Deg * angleRadians, Vector3.up);
        return q * v;
    }
}