using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Weapons;

public class PlayerComponent : ActorComponent
{
    public IEnumerable<AWeaponType> ActiveWeapons => _activeWeapons;

    private List<AWeaponType> _activeWeapons;
    private List<IWeaponEffect> _weaponEffects;

    private MovementComponent _movementComponent;
    private BounceAnimationComponent _bounceAnimation;

    public void Start()
    {
        _movementComponent = GetComponent<MovementComponent>();
        _bounceAnimation = GetComponent<BounceAnimationComponent>();

        _activeWeapons = new List<AWeaponType>();
        if (Game.Data.StartingWeapon != null)
            _activeWeapons.Add(Game.Data.StartingWeapon);

        _weaponEffects = new List<IWeaponEffect>();
        foreach (var weaponEffect in _activeWeapons.Select(weapon => weapon.GetWeaponEffect()))
        {
            weaponEffect.Owner = this;
            weaponEffect.OnShoot += OnWeaponShoot;
            _weaponEffects.Add(weaponEffect);
        }
    }

    private void OnWeaponShoot()
    {
        if (_bounceAnimation != null)
            _bounceAnimation.TriggerBounce();
    }

    public void Update()
    {
        var inputHorizontal = 0.0f;
        if (Input.GetKey(KeyCode.RightArrow)) inputHorizontal++;
        if (Input.GetKey(KeyCode.LeftArrow)) inputHorizontal--;

        var inputVertical = 0.0f;
        if (Input.GetKey(KeyCode.UpArrow)) inputVertical++;
        if (Input.GetKey(KeyCode.DownArrow)) inputVertical--;

        var moveDirection = new Vector3(inputHorizontal, 0, inputVertical).normalized;
        _movementComponent.SetMovementDirection(moveDirection);

        var shootDirection = Vector3.zero;
        var closestEnemy = Game.GetClosestEnemy(transform.position);
        if (closestEnemy != null)
            shootDirection = (closestEnemy.transform.position - transform.position).normalized;

        foreach (var weaponEffect in _weaponEffects)
            weaponEffect.Update(transform.position, shootDirection);
    }

    public void ReplaceWeapon(AWeaponType oldWeapon, AWeaponType newWeapon)
    {
        _activeWeapons.Remove(oldWeapon);
        _activeWeapons.Add(newWeapon);

        _weaponEffects = new List<IWeaponEffect>();
        foreach (var weapon in _activeWeapons)
        {
            var weaponEffect = weapon.GetWeaponEffect();
            weaponEffect.Owner = this;
            weaponEffect.OnShoot += OnWeaponShoot;
            _weaponEffects.Add(weaponEffect);
        }
    }
}