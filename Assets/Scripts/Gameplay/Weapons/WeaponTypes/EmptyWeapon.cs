using UnityEngine;

namespace Weapons
{
    //Do Nothing
    [CreateAssetMenu(menuName = "Weapons/Empty")]
    public class EmptyWeapon : AWeaponType
    {
        public override IWeaponEffect GetWeaponEffect()
        {
            return new DummyWeaponEffect();
        }
    }
}