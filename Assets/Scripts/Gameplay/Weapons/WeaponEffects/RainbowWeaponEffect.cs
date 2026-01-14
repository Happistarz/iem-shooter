using System;
using UnityEngine;
using Weapons;

public class RainbowWeaponEffect : IWeaponEffect
{
    public ActorComponent Owner { get; set; }
    public event Action   OnShoot;

    private readonly BulletComponent _prefab;
    private readonly float           _bulletSpeed;
    private readonly float           _rateOfFire;
    private readonly float           _rotationSpeed;
    private readonly Gradient        _gradient;

    private float _nextShotDelay;

    public RainbowWeaponEffect(BulletComponent prefab, float bulletSpeed, float rateOfFire, float rotationSpeed,
                               Gradient        gradient)
    {
        _prefab        = prefab;
        _bulletSpeed   = bulletSpeed;
        _rateOfFire    = rateOfFire;
        _rotationSpeed = rotationSpeed;
        _gradient      = gradient;
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

        bullet.transform.localScale *= 2;

        var renderer = bullet.GetComponent<MeshRenderer>();
        if (renderer)
        {
            var gradientTime = angle % 2 * Mathf.PI / (2 * Mathf.PI);

            var material = renderer.material;
            material.color = _gradient.Evaluate(gradientTime);
        }

        _nextShotDelay = 0;
        OnShoot?.Invoke();
    }

    private static Vector3 Rotate(Vector3 v, float angleRadians)
    {
        var q = Quaternion.AngleAxis(Mathf.Rad2Deg * angleRadians, Vector3.up);
        return q * v;
    }
}