using R2API;

namespace HIFUArtificerTweaks
{
    public abstract class TweakBase
    {
        public abstract string Name { get; }
        public abstract string SkillToken { get; }
        public abstract string DescText { get; }
        public virtual bool isEnabled { get; } = true;
        public bool done = false;

        public T ConfigOption<T>(T value, string name, string description)
        {
            var config = Main.HATConfig.Bind<T>(Name, name, value, description);
            ConfigManager.HandleConfig<T>(config, Main.HATBackupConfig, name);
            if (!done)
            {
                ConfigManager.HandleConfig<T>(Main.flamewallDamage, Main.HATBackupConfig, Main.flamewallDamage.Definition.Key);
                ConfigManager.HandleConfig<T>(Main.flamewallSpeed, Main.HATBackupConfig, Main.flamewallSpeed.Definition.Key);
                ConfigManager.HandleConfig<T>(Main.flamewallProcCoeff, Main.HATBackupConfig, Main.flamewallProcCoeff.Definition.Key);
                done = true;
            }

            return config.Value;
        }

        public abstract void Hooks();

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        public virtual void Init()
        {
            Hooks();
            string descriptionToken = "MAGE_" + SkillToken.ToUpper() + "_DESCRIPTION";
            LanguageAPI.Add(descriptionToken, DescText);
            Main.HATLogger.LogInfo("Added " + Name);
        }
    }
}