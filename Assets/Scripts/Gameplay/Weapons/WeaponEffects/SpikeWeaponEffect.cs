using System;
using UnityEngine;
using Weapons;

public class SpikeWeaponEffect : IWeaponEffect
{
    public SpikeWeaponEffect(BulletComponent prefab, int multiShotCount, float speed, float rateOfFire)
    {
        BulletPrefab   = prefab;
        MultiShotCount = multiShotCount;
        Speed          = speed;
        RateOfFire     = rateOfFire;
    }

    private BulletComponent BulletPrefab   { get; }
    private int             MultiShotCount { get; }
    private float           Speed          { get; }
    private float           RateOfFire     { get; }

    public ActorComponent Owner { get; set; }
    public event Action   OnShoot;

    private float _nextShotDelay;

    public void Update(Vector3 origin, Vector3 direction)
    {
        var delayBetweenShots = 1.0f / RateOfFire;
        _nextShotDelay += Time.deltaTime;
        if (!(_nextShotDelay > delayBetweenShots) || direction == Vector3.zero) return;

        Shoot(origin);
        _nextShotDelay = 0;
    }

    private void Shoot(Vector3 origin)
    {
        for (var i = 0; i < MultiShotCount; i++)
        {
            var rotation = GetSpikeRotation(i);

            var pool   = Game.GetBulletPool(BulletPrefab);
            var bullet = pool.Get();
            bullet.prefab = BulletPrefab;

            bullet.Reset();
            bullet.transform.position = origin;
            bullet.transform.rotation = rotation;
            bullet.Velocity           = rotation * Vector3.forward * Speed;
            bullet.Owner              = Owner;
        }

        OnShoot?.Invoke();
    }

    private Quaternion GetSpikeRotation(int index)
    {
        var angle = 360f / MultiShotCount * index;

        var rotation = Quaternion.AngleAxis(angle, Vector3.up);
        return rotation;
    }
}