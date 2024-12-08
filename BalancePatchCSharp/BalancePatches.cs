using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using static Obeliskial_Essentials.Essentials;
using System;
using static UnofficialBalancePatch.CustomFunctions;
using static UnofficialBalancePatch.Plugin;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

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
            LogInfo("executing supriseboxPrefix");
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
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Item), nameof(Item.surprisegiftbox))]
        public static bool surprisegiftboxPrefix(Item __instance, Character theCharacter, bool isRare, string itemName)
        {
            LogInfo("executing surprisegiftbox prefix");

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

        public static List<string> cardsWithCustomDescriptions = ["surprisebox", "surpriseboxrare", "surprisegiftbox", "surprisegiftboxrare"];
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardData), nameof(CardData.SetDescriptionNew))]
        public static void SetDescriptionNewPostfix(CardData __instance, bool forceDescription = false, Character character = null, bool includeInSearch = true)
        {
            // LogInfo("executing SetDescriptionNewPostfix");
            if (__instance == null)
            {
                LogDebug("Null Card");
                return;
            }
                


            StringBuilder stringBuilder1 = new StringBuilder();

            if (cardsWithCustomDescriptions.Contains(__instance.Id))
            {
                LogDebug("Creating description for " + __instance.Id);
                // string str1 = "<line-height=15%><br></line-height>";
                // string str2 = "<color=#444><size=-.15>";
                // string str3 = "</size></color>";
                // string str4 = "<color=#5E3016><size=-.15>";
                // int num1 = 0;

                string descriptionId = itemStem + __instance.Id;
                // if (descriptionId != "")
                stringBuilder1.Append(Functions.FormatStringCard(Texts.Instance.GetText(descriptionId)));
                BinbinNormalizeDescription(__instance, stringBuilder1);
                return;
            }

            if (__instance.Item == null)
                return;
            ItemData itemData = __instance.Item;

            HandleDamagePercentDescription(stringBuilder1, itemData, __instance.Item.DamagePercentBonus, __instance.Item.DamagePercentBonusValue);
            HandleDamagePercentDescription(stringBuilder1, itemData, __instance.Item.DamagePercentBonus2, __instance.Item.DamagePercentBonusValue2);
            HandleDamagePercentDescription(stringBuilder1, itemData, __instance.Item.DamagePercentBonus3, __instance.Item.DamagePercentBonusValue3);
            BinbinNormalizeDescription(__instance, stringBuilder1);
            
        }

        private static string NumFormatItem(int num, bool plus = false, bool percent = false)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(" <nobr>");
            if (num > 0)
            {
                stringBuilder.Append("<color=#263ABC><size=+.1>");
                if (plus)
                    stringBuilder.Append("+");
            }
            else
            {
                stringBuilder.Append("<color=#720070><size=+.1>");
                if (plus)
                    stringBuilder.Append("-");
            }
            stringBuilder.Append(Mathf.Abs(num));
            if (percent)
                stringBuilder.Append("%");
            stringBuilder.Append("</color></size></nobr>");
            return stringBuilder.ToString();
        }

        public static void BinbinNormalizeDescription(CardData __instance, StringBuilder stringBuilder)
        {
            stringBuilder.Replace("<c>", "<color=#5E3016>");
            stringBuilder.Replace("</c>", "</color>");
            stringBuilder.Replace("<nb>", "<nobr>");
            stringBuilder.Replace("</nb>", "</nobr>");
            stringBuilder.Replace("<br1>", "<br><line-height=15%><br></line-height>");
            stringBuilder.Replace("<br2>", "<br><line-height=30%><br></line-height>");
            stringBuilder.Replace("<br3>", "<br><line-height=50%><br></line-height>");
            string descriptionNormalized = stringBuilder.ToString();
            descriptionNormalized = Regex.Replace(descriptionNormalized, "[,][ ]*(<(.*?)>)*(.)", (MatchEvaluator)(m => m.ToString().ToLower()));
            descriptionNormalized = Regex.Replace(descriptionNormalized, "<br>\\w", (MatchEvaluator)(m => m.ToString().ToUpper()));
            Globals.Instance.CardsDescriptionNormalized[__instance.Id] = stringBuilder.ToString();
        }

        public static void HandleDamagePercentDescription(StringBuilder stringBuilder, ItemData itemData, Enums.DamageType damageType, float percentIncrease)
        {
            if (damageType == Enums.DamageType.None || percentIncrease == 0f)
                return;

            LogDebug("Custom Descriptions - itemAllDamages text string - " + Texts.Instance.GetText("itemAllDamages"));
            string dt = nameof(damageType);
            int percentDamageIncrease = Functions.FuncRoundToInt(itemData.DamagePercentBonusValue);
            LogDebug("Custom Descriptions - damage type - " + dt);
            string damageTypeText = "item" + dt + "Damages";
            // stringBuilder.Append(string.Format(Texts.Instance.GetText(damageTypeText), (object)NumFormatItem(percentDamageIncrease, true, true)));
            stringBuilder.Append(string.Format(Texts.Instance.GetText("itemAllDamages"), (object)NumFormatItem(percentDamageIncrease, true, true)));
            stringBuilder.Append("\n");
        }


    }
}