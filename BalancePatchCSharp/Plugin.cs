// These are your imports, mostly you'll be needing these 5 for every plugin. Some will need more.

using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using static Obeliskial_Essentials.Essentials;
using System;


// The Plugin csharp file is used to 


// Make sure all your files have the same namespace and this namespace matches the RootNamespace in the .csproj file
namespace UnofficialBalancePatch{
    // These are used to create the actual plugin. If you don't need Obeliskial Essentials for your mod, 
    // delete the BepInDependency and the associated code "RegisterMod()" below.

    // If you have other dependencies, such as obeliskial content, make sure to include them here.
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.stiffmeds.obeliskialessentials")] // this is the name of the .dll in the !libs folder.
    [BepInDependency("com.stiffmeds.obeliskialcontent")]
    [BepInProcess("AcrossTheObelisk.exe")] //Don't change this

    // If PluginInfo isn't working, you are either:
    // 1. Using BepInEx v6
    // 2. Have an issue with your csproj file (not loading the analyzer or BepInEx appropriately)
    // 3. You have an issue with your solution file (not referencing the correct csproj file)


    public class Plugin : BaseUnityPlugin
    {
        
        // If desired, you can create configs for users by creating a ConfigEntry object here, 
        // and then use config = Config.Bind() to set the title, default value, and description of the config.
        // It automatically creates the appropriate configs.
        
        // public static ConfigEntry<bool> SampleBooleanConfig { get; set; }
        // public static ConfigEntry<int> SampleIntegerConfig { get; set; }

        internal const int ModDate = 20241206; //int.Parse(DateTime.Today.ToString("yyyyMMdd"));
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        internal static ManualLogSource Log;
        public static string itemStem = "binbin_balances_";

        public static string debugBase = "Custom Descriptions - ";

        private void Awake()
        {

            // The Logger will allow you to print things to the LogOutput (found in the BepInEx directory)
            Log = Logger;
            Log.LogInfo($"{PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} has loaded!");
            
            // Sets the title, default values, and descriptions
            // SampleBooleanConfig = Config.Bind(new ConfigDefinition("Debug", "Name of Config"), true, new ConfigDescription("Description of Config"));
            // SampleIntegerConfig = Config.Bind(new ConfigDefinition("Debug", "Name of Config"), 3, new ConfigDescription("Description of Config)"));
            

            // Register with Obeliskial Essentials, delete this if you don't need it.
            RegisterMod(
                _name: PluginInfo.PLUGIN_NAME,
                _author: "binbin",
                _description: "Unofficial Balance Patch",
                _version: PluginInfo.PLUGIN_VERSION,
                _date: ModDate,
                _link: @"https://github.com/binbinmods/Unofficial-AtO-Balance-Patch"
            );
            
            // Custom Text for % Damage increase
            var damageTypeArray = Enum.GetValues(typeof(Enums.DamageType));
            foreach (Enums.DamageType damageType in damageTypeArray)
            {
                string dt = damageType.ToString().ToLower();
                medsTexts[$"item{dt}Damages"] = "<space=.3><size=+.1><sprite name="+dt+"></size> damage  {0}";
                LogDebug("Setting medsText for itemDamageType: " + dt);
            }            

            
            // Custom Text for Items
            medsTexts[itemStem + "surprisebox"] = "At the start of your third turn, gain a significant random buff.";
            medsTexts[itemStem + "surpriseboxrare"] = "At the start of your third turn, gain a significant random buff.";
            medsTexts[itemStem + "surprisegiftbox"] = "At the start of your third turn, all heroes gain a significant random buff.";
            medsTexts[itemStem + "surprisegiftboxrare"] = "At the start of your third turn, all heroes gain a significant random buff.";

            medsTexts[itemStem + "bbbportablewallofflames"] = "<size=+.1><sprite name=thorns></size> charges +1 \n Thorns on this character deal <space=.3><size=+.1><sprite name=fire></size> damage.";
            medsTexts[itemStem + "bbbportablewallofflamesrare"] = "<size=+.1><sprite name=thorns></size> charges +2 \n Thorns on this character deal <space=.3><size=+.1><sprite name=fire></size> damage.";
            
            medsTexts[itemStem + "bbbtreefellingaxe"] = "All damage +1 \n <size=+.1><sprite name=bleed></size> charges +1 \n Bleed on enemies cannot be prevented by Buffer or Immunities.";
            medsTexts[itemStem + "bbbtreefellingaxerare"] = "All damage +2 \n <size=+.1><sprite name=bleed></size> charges +2 \n Bleed on enemies cannot be prevented by Buffer or Immunities.";
            
            medsTexts[itemStem + "bbbslimepoison"] = "All damage +1 \n <size=+.1><sprite name=poison></size> charges +1 \n Poison on enemies cannot be prevented by Buffer or Immunities, nor can it be Dispelled unless specified.";
            medsTexts[itemStem + "bbbslimepoisonrare"] = "All damage +2 \n <size=+.1><sprite name=poison></size> charges +2 \n Poison on enemies cannot be prevented by Buffer or Immunities, nor can it be Dispelled unless specified.";

            medsTexts[itemStem + "bbbcloakofthorns"] = "All resistances +5% \n  <size=+.1><sprite name=mitigate></size> on heroes increases Thorns damage by 15% per charge.";
            medsTexts[itemStem + "bbbcloakofthornsrare"] = "All resistances +5% \n  <size=+.1><sprite name=mitigate></size> on heroes increases Thorns damage by 25% per charge.";

            // apply patches
            harmony.PatchAll();
        }

        internal static void LogDebug(string msg)
        {
            Log.LogDebug(debugBase + msg);
        }
        internal static void LogInfo(string msg)
        {
            Log.LogInfo(debugBase + msg);
        }
        internal static void LogError(string msg)
        {
            Log.LogError(debugBase + msg);
        }
    }
}