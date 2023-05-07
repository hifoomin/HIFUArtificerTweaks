namespace HIFUArtificerTweaks.Skills
{
    public class ChargedNanoBomb : TweakBase
    {
        public static float minDamage;
        public static float maxDamage;
        public static float castTime;
        public override string Name => ": Secondary :: Charged Nano-Bomb";

        public override string SkillToken => "secondary_lightning";

        public override string DescText => "<style=cIsDamage>Stunning</style>. Charge up a <style=cIsDamage>exploding</style> nano-bomb that deals <style=cIsDamage>" + d(minDamage) + "-" + d(maxDamage) + "</style> damage.";

        public override void Init()
        {
            minDamage = ConfigOption(4f, "Minimum Damage", "Decimal. Vanilla is 4");
            maxDamage = ConfigOption(20f, "Maximum Damage", "Decimal. Vanilla is 20");
            castTime = ConfigOption(2f, "Max Charge Time", "Vanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Mage.Weapon.BaseChargeBombState.OnEnter += BaseChargeBombState_OnEnter;
            On.EntityStates.Mage.Weapon.BaseThrowBombState.OnEnter += BaseThrowBombState_OnEnter;
        }

        private void BaseChargeBombState_OnEnter(On.EntityStates.Mage.Weapon.BaseChargeBombState.orig_OnEnter orig, EntityStates.Mage.Weapon.BaseChargeBombState self)
        {
            if (self is EntityStates.Mage.Weapon.ChargeNovabomb)
            {
                self.baseDuration = castTime;
            }
            orig(self);
        }

        private void BaseThrowBombState_OnEnter(On.EntityStates.Mage.Weapon.BaseThrowBombState.orig_OnEnter orig, EntityStates.Mage.Weapon.BaseThrowBombState self)
        {
            if (self is EntityStates.Mage.Weapon.ThrowNovabomb)
            {
                self.minDamageCoefficient = minDamage;
                self.maxDamageCoefficient = maxDamage;
            }
            orig(self);
        }
    }
}