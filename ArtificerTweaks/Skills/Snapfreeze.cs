using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HAT.Skills
{
    public class Snapfreeze : TweakBase
    {
        public static float damage;
        public static float lifetime;

        public override string Name => ": Utility : Snapfreeze";

        public override string SkillToken => "utility_ice";

        public override string DescText => "<style=cIsUtility>Freezing</style>. Create a barrier that hurts enemies for <style=cIsDamage>" + d(damage) + " damage</style>.";

        public override void Init()
        {
            lifetime = ConfigOption(0.35f, "Lifetime", "Vanilla is 0.3. Wall Count = 40 * lifetime");
            damage = ConfigOption(1.2f, "Damage", "Decimal. Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Mage.Weapon.PrepWall.OnEnter += PrepWall_OnEnter;
            Changes();
        }

        private void PrepWall_OnEnter(On.EntityStates.Mage.Weapon.PrepWall.orig_OnEnter orig, EntityStates.Mage.Weapon.PrepWall self)
        {
            EntityStates.Mage.Weapon.PrepWall.damageCoefficient = damage;
            orig(self);
        }

        private void Changes()
        {
            var iceWall = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageIcewallWalkerProjectile.prefab").WaitForCompletion();
            var projectileCharacterController = iceWall.GetComponent<ProjectileCharacterController>();

            projectileCharacterController.lifetime = lifetime;
        }
    }
}