using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HIFUArtificerTweaks.Projectiles;
using HIFUArtificerTweaks.Skilldefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using R2API;
using R2API.ContentManagement;
using HarmonyLib;

namespace HIFUArtificerTweaks
{
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInDependency("com.Borbo.ArtificerExtended", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public const string PluginAuthor = "HIFU";
        public const string PluginName = "HIFUArtificerTweaks";
        public const string PluginVersion = "1.1.7";

        public static ConfigFile HATConfig;
        public static ConfigFile HATBackupConfig;
        public static bool aeLoaded;

        public static ConfigEntry<bool> enableAutoConfig { get; set; }
        public static ConfigEntry<string> latestVersion { get; set; }

        public static ManualLogSource HATLogger;

        public static ConfigEntry<float> flamewallDamage;
        public static ConfigEntry<float> flamewallSpeed;
        public static ConfigEntry<float> flamewallProcCoeff;

        public static AssetBundle hifuartificertweaks;

        public static bool _preVersioning = false;

        public void Awake()
        {
            HATLogger = Logger;
            HATConfig = Config;

            aeLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Borbo.ArtificerExtended");

            hifuartificertweaks = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("HIFUArtificerTweaks.dll", "hifuartificertweaks"));

            HATBackupConfig = new(Paths.ConfigPath + "\\" + PluginAuthor + "." + PluginName + ".Backup.cfg", true);
            HATBackupConfig.Bind(": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :");

            enableAutoConfig = HATConfig.Bind("Config", "Enable Auto Config Sync", true, "Disabling this would stop HIFUArtificerTweaks from syncing config whenever a new version is found.");
            _preVersioning = !((Dictionary<ConfigDefinition, string>)AccessTools.DeclaredPropertyGetter(typeof(ConfigFile), "OrphanedEntries").Invoke(HATConfig, null)).Keys.Any(x => x.Key == "Latest Version");
            latestVersion = HATConfig.Bind("Config", "Latest Version", PluginVersion, "DO NOT CHANGE THIS");
            if (enableAutoConfig.Value && (_preVersioning || (latestVersion.Value != PluginVersion)))
            {
                latestVersion.Value = PluginVersion;
                ConfigManager.VersionChanged = true;
                HATLogger.LogInfo("Config Autosync Enabled.");
            }

            flamewallDamage = Config.Bind(": Utility :: Flamewall", "Damage", 0.55f, "Decimal. Default is 0.65");
            flamewallSpeed = Config.Bind(": Utility :: Flamewall", "Speed Multiplier", 1.3f, "Default is 1.3");
            flamewallProcCoeff = Config.Bind(": Utility :: Flamewall", "Proc Coefficient", 0.15f, "Default is 0.15");

            WallOfInfernoProjectile.Create();
            FlamewallSD.Create();
            AddUtility.Create();

            IEnumerable<Type> enumerable = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(TweakBase))
                                           select type;

            HATLogger.LogInfo("==+----------------==TWEAKS==----------------+==");

            foreach (Type type in enumerable)
            {
                TweakBase based = (TweakBase)Activator.CreateInstance(type);
                if (ValidateTweak(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable2 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(MiscBase))
                                            select type;

            HATLogger.LogInfo("==+----------------==MISC==----------------+==");

            foreach (Type type in enumerable2)
            {
                MiscBase based = (MiscBase)Activator.CreateInstance(type);
                if (ValidateMisc(based))
                {
                    based.Init();
                }
            }
        }

        public bool ValidateTweak(TweakBase tb)
        {
            if (tb.isEnabled)
            {
                bool enabledfr = Config.Bind(tb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ValidateMisc(MiscBase mb)
        {
            if (mb.isEnabled)
            {
                bool enabledfr = Config.Bind(mb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr)
                {
                    return true;
                }
            }
            return false;
        }

        private void WITHINDESTRUCTIONMYFUCKINGBELOVED()
        {
        }
    }
}