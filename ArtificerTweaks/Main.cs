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

namespace HIFUArtificerTweaks
{
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public const string PluginAuthor = "HIFU";
        public const string PluginName = "HIFUArtificerTweaks";
        public const string PluginVersion = "1.1.2";

        public static ConfigFile HATConfig;
        public static ManualLogSource HATLogger;

        public static ConfigEntry<float> flamewallDamage;
        public static ConfigEntry<float> flamewallSpeed;
        public static ConfigEntry<float> flamewallProcCoeff;

        public static AssetBundle hifuartificertweaks;

        public void Awake()
        {
            HATLogger = Logger;
            HATConfig = Config;

            hifuartificertweaks = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("HIFUArtificerTweaks.dll", "hifuartificertweaks"));

            flamewallDamage = Config.Bind(": Utility :: Flamewall", "Damage", 0.65f, "Decimal. Default is 0.65");
            flamewallSpeed = Config.Bind(": Utility :: Flamewall", "Speed Multiplier", 1.3f, "Default is 1.3");
            flamewallProcCoeff = Config.Bind(": Utility :: Flamewall", "Proc Coefficient", 0.15f, "Default is 0.15");

            WallOfInfernoProjectile.Create();
            WallOfInfernoSD.Create();
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