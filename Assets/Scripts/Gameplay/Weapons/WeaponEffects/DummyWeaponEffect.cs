using System;
using UnityEngine;

namespace Weapons
{
    public class DummyWeaponEffect : IWeaponEffect
    {
        public ActorComponent Owner { get; set; }
        public event Action OnShoot;

        public void Update(Vector3 origin, Vector3 direction)
        {
            // No effect
        }
    }
}