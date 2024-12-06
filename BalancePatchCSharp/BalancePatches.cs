using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using static Obeliskial_Essentials.Essentials;
using System;
using static UnofficialBalancePatch.CustomFunctions;
using static UnofficialBalancePatch.Plugin;

// Make sure your namespace is the same everywhere
namespace UnofficialBalancePatch
{

    [HarmonyPatch] //DO NOT REMOVE/CHANGE

    public class BalancePatches
    {
        // To create a patch, you need to declare either a prefix or a postfix. 
        // Prefixes are executed before the original code, postfixes are executed after
        // Then you need to tell Harmony which method to patch.

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Item), nameof(Item.surprisebox))]
        public static bool surpriseboxPrefix(Item __instance, Character theCharacter, bool isRare, string itemName)
        {
            LogDebug("supriseboxPrefix");
            if (!((UnityEngine.Object)MatchManager.Instance != (UnityEngine.Object)null))
                return false;
            int randomIntRange = MatchManager.Instance.GetRandomIntRange(0, 9, "item");
            int charges = 4;
            string auracurse;
            switch (randomIntRange)
            {
                case 0:
                    charges = isRare ? 6 : 3;
                    auracurse = "fast";
                    break;
                case 1:
                    charges = isRare ? 10 : 5;
                    auracurse = "powerful";
                    break;
                case 2:
                    charges = isRare ? 8 : 4;
                    auracurse = "bless";
                    break;
                case 3:
                    charges = isRare ? 8 : 4;
                    auracurse = "sharp";
                    break;
                case 4:
                    charges = isRare ? 6 : 3;
                    auracurse = "fortify";
                    break;
                case 5:
                    charges = isRare ? 4 : 2;
                    auracurse = "evasion";
                    break;
                case 6:
                    charges = isRare ? 10 : 5;
                    auracurse = "regeneration";
                    break;
                case 7:
                    charges = isRare ? 4 : 2;
                    auracurse = "mitigate";
                    break;
                default:
                    charges = isRare ? 8 : 4;
                    auracurse = "vitality";
                    break;
            }
            theCharacter.SetAuraTrait(theCharacter, auracurse, charges);
            theCharacter.HeroItem.ScrollCombatText(itemName, Enums.CombatScrollEffectType.Accesory);
            return false;

        }

        // Postfixes work mostly the same
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Item), nameof(Item.surprisegiftbox))]
        public static bool surprisegiftboxPrefix(Item __instance, Character theCharacter, bool isRare, string itemName)
        {
            LogInfo("surprisegiftbox prefix");

            if (!((UnityEngine.Object)MatchManager.Instance != (UnityEngine.Object)null))
                return false;
            int randomIntRange = MatchManager.Instance.GetRandomIntRange(0, 9, "item");
            int charges = 4;
            string auracurse;
            switch (randomIntRange)
            {
                case 0:
                    charges = isRare ? 6 : 3;
                    auracurse = "fast";
                    break;
                case 1:
                    charges = isRare ? 10 : 5;
                    auracurse = "powerful";
                    break;
                case 2:
                    charges = isRare ? 8 : 4;
                    auracurse = "bless";
                    break;
                case 3:
                    charges = isRare ? 8 : 4;
                    auracurse = "sharp";
                    break;
                case 4:
                    charges = isRare ? 6 : 3;
                    auracurse = "fortify";
                    break;
                case 5:
                    charges = isRare ? 4 : 2;
                    auracurse = "evasion";
                    break;
                case 6:
                    charges = isRare ? 10 : 5;
                    auracurse = "regeneration";
                    break;
                case 7:
                    charges = isRare ? 4 : 2;
                    auracurse = "mitigate";
                    break;
                default:
                    charges = isRare ? 8 : 4;
                    auracurse = "vitality";
                    break;
            }
            Hero[] teamHero = MatchManager.Instance.GetTeamHero();
            for (int index = 0; index < teamHero.Length; ++index)
            {
                if (teamHero[index] != null && (UnityEngine.Object)teamHero[index].HeroData != (UnityEngine.Object)null && teamHero[index].Alive)
                    teamHero[index].SetAuraTrait(theCharacter, auracurse, charges);
            }
            theCharacter.HeroItem.ScrollCombatText(itemName, Enums.CombatScrollEffectType.Accesory);
            return false;

        }


    }
}