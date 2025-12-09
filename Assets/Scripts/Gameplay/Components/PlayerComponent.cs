using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class PlayerComponent : ActorComponent
{
    public IEnumerable<AWeaponType> ActiveWeapons => m_activeWeapons;

    private List<AWeaponType> m_activeWeapons;
    private List<IWeaponEffect> m_weaponEffects;

    private MovementComponent m_movementComponent;

    public void Start()
    {
        m_movementComponent = GetComponent<MovementComponent>();

        m_activeWeapons = new List<AWeaponType>();
        if (Game.Data.StartingWeapon != null)
            m_activeWeapons.Add(Game.Data.StartingWeapon);

        m_weaponEffects = new List<IWeaponEffect>();
        foreach (var weapon in m_activeWeapons)
        {
            IWeaponEffect weaponEffect = weapon.GetWeaponEffect();
            weaponEffect.Owner = this;
            m_weaponEffects.Add(weaponEffect);
        }
    }

    public void Update()
    {
        float inputHorizontal = 0.0f;
        if (Input.GetKey(KeyCode.RightArrow)) inputHorizontal++;
        if (Input.GetKey(KeyCode.LeftArrow)) inputHorizontal--;

        float inputVertical = 0.0f;
        if (Input.GetKey(KeyCode.UpArrow)) inputVertical++;
        if (Input.GetKey(KeyCode.DownArrow)) inputVertical--;

        Vector3 moveDirection = new Vector3(inputHorizontal, 0, inputVertical).normalized;
        m_movementComponent.SetMovementDirection(moveDirection);

        Vector3 shootDirection = Vector3.zero;
        EnemyComponent closestEnemy = Game.GetClosestEnemy(transform.position);
        if (closestEnemy != null)
            shootDirection = (closestEnemy.transform.position - transform.position).normalized;

        foreach (var weaponEffect in m_weaponEffects)
            weaponEffect.Update(transform.position, shootDirection);
    }

    public void ReplaceWeapon(AWeaponType oldWeapon, AWeaponType newWeapon)
    {
        m_activeWeapons.Remove(oldWeapon);
        m_activeWeapons.Add(newWeapon);

        m_weaponEffects = new List<IWeaponEffect>();
        foreach (var weapon in m_activeWeapons)
            m_weaponEffects.Add(weapon.GetWeaponEffect());
    }
}