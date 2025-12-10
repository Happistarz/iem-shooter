using System;
using Unity.VisualScripting;
using UnityEngine;
using Weapons;
using Object = UnityEngine.Object;

public class RainbowPulseWeaponEffect : IWeaponEffect
{
    public ActorComponent Owner { get; set; }
    public event Action   OnShoot;

    private readonly BulletComponent _prefab;
    private readonly float           _bulletSpeed;
    private readonly float           _rateOfFire;
    private readonly float           _rotationSpeed;
    private readonly float           _gradientDuration;
    private readonly Gradient        _gradient;

    private float _nextShotDelay;

    public RainbowPulseWeaponEffect(BulletComponent prefab,   float bulletSpeed, float rateOfFire, float rotationSpeed,
                                    Gradient        gradient, float gradientDuration)
    {
        _prefab           = prefab;
        _bulletSpeed      = bulletSpeed;
        _rateOfFire       = rateOfFire;
        _rotationSpeed    = rotationSpeed;
        _gradient         = gradient;
        _gradientDuration = gradientDuration;
    }

    public void Update(Vector3 origin, Vector3 direction)
    {
        var delayBetweenShots = 1.0f / _rateOfFire;
        _nextShotDelay += Time.deltaTime;
        if (!(_nextShotDelay > delayBetweenShots)) return;

        var angle          = Time.time * _rotationSpeed;
        var shootDirection = Rotate(Vector3.forward, angle);

        var bullet = Object.Instantiate(_prefab);
        bullet.transform.position = origin;
        bullet.Velocity           = shootDirection * _bulletSpeed;
        bullet.Owner              = Owner;

        //Change projectile size
        bullet.transform.localScale *= 2;

        //Add color variations
        var colorGradientCmp = bullet.AddComponent<ColorGradientComponent>();
        colorGradientCmp.Gradient   = _gradient;
        colorGradientCmp.Duration   = _gradientDuration;
        colorGradientCmp.TimeOffset = angle;

        _nextShotDelay = 0;
        OnShoot?.Invoke();
    }

    private static Vector3 Rotate(Vector3 v, float angleRadians)
    {
        var q = Quaternion.AngleAxis(Mathf.Rad2Deg * angleRadians, Vector3.up);
        return q * v;
    }
}