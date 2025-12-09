using UnityEngine;

namespace Weapons
{
    public abstract class AWeaponType : ScriptableObject
    {
        public abstract IWeaponEffect GetWeaponEffect();
    }
}