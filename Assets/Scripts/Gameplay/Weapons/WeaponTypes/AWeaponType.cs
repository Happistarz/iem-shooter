using UnityEngine;

namespace Weapons
{
    public abstract class AWeaponType : ScriptableObject
    {
        public int Damage = 1;

        public abstract IWeaponEffect GetWeaponEffect();
    }
}