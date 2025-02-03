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

    // [HarmonyPatch] //DO NOT REMOVE/CHANGE

    public class BalanceFunctions
    {
        // To create a patch, you need to declare either a prefix or a postfix. 
        // Prefixes are executed before the original code, postfixes are executed after
        // Then you need to tell Harmony which method to patch.

        public static string NumFormatItem(int num, bool plus = false, bool percent = false)
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

        public static void BinbinNormalizeDescription(ref CardData __instance, StringBuilder stringBuilder)
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
            __instance.DescriptionNormalized = descriptionNormalized;
            Traverse.Create(__instance).Field("descriptionNormalized").SetValue(descriptionNormalized);


        }

        public static void HandleDamagePercentDescription(ref StringBuilder stringBuilder, ItemData itemData, Enums.DamageType damageType, float percentIncrease)
        {
            if (itemData.Id == "battleaxerare")
            {
                LogDebug(" battleaxerare");
            }

            if (damageType == Enums.DamageType.None || damageType == Enums.DamageType.All || percentIncrease == 0f)
                return;

            // LogDebug("itemAllDamages text string - " + Texts.Instance.GetText("itemAllDamages"));
            string dt = damageType.ToString().ToLower();
            // string dt = "All";
            int percentDamageIncrease = Functions.FuncRoundToInt(itemData.DamagePercentBonusValue);
            LogDebug("damage type - " + dt);
            string damageTypeText = "item" + dt + "Damages";
            LogDebug("medsText - " + medsTexts[damageTypeText]);
            LogDebug("GetText - " + Texts.Instance.GetText(damageTypeText));
            // stringBuilder.Append(string.Format(medsTexts[damageTypeText], (object)NumFormatItem(percentDamageIncrease, true, true)));

            // this should use Texts.Instance.GetText(damageTypeText)) for translation. Don't know how to get it working. Need to add MedsTexts to Texts
            string toAdd = string.Format(medsTexts[damageTypeText], (object)NumFormatItem(percentDamageIncrease, true, true)) + "\n";

            // Adds it either to the start of the stringbuilder or immediately after
            // string searchPhrase = $"<space=.3><size=+.1><sprite name={dt}></size> damage ";
            // int insertIndex = stringBuilder.ToString().IndexOf(searchPhrase);
            // if (insertIndex != -1)
            // {
            //     stringBuilder.Insert(insertIndex + searchPhrase.Length, toAdd);
            // }
            // else
            // {
            //     stringBuilder.Insert(0,toAdd);
            // }
            stringBuilder.Insert(0, toAdd);

            // "<space=.3><size=+.1><sprite name=fire></size> damage  <nobr><color=#263ABC><size=+.1>+3</color></size></nobr>"
            // stringBuilder.Append(string.Format(Texts.Instance.GetText("itemAllDamages"), (object)NumFormatItem(percentDamageIncrease, true, true)));
            // stringBuilder.Append("\n");
        }

        public static void HandleAllDamagePercentDescriptions(ref CardData __instance)
        {
            StringBuilder stringBuilder1 = new();
            if (__instance.Item == null)
                return;

            ItemData itemData = __instance.Item;

            if (itemData.DamagePercentBonus == Enums.DamageType.None && itemData.DamagePercentBonus2 == Enums.DamageType.None && itemData.DamagePercentBonus3 == Enums.DamageType.None)
                return;

            if (__instance.Id == "burningorbrare" || __instance.Id == "frozenorbrare")
            {
                LogDebug("Setting Description for " + __instance.Id);
                // LogDebug("Original CardDescription - " + Globals.Instance.CardsDescriptionNormalized[__instance.Id]);
                // LogDebug("");
            }

            stringBuilder1.Append(Globals.Instance.CardsDescriptionNormalized[__instance.Id]);
            HandleDamagePercentDescription(ref stringBuilder1, itemData, __instance.Item.DamagePercentBonus, __instance.Item.DamagePercentBonusValue);
            HandleDamagePercentDescription(ref stringBuilder1, itemData, __instance.Item.DamagePercentBonus2, __instance.Item.DamagePercentBonusValue2);
            HandleDamagePercentDescription(ref stringBuilder1, itemData, __instance.Item.DamagePercentBonus3, __instance.Item.DamagePercentBonusValue3);
            // stringBuilder1.Append("\n Testing\n");
            BinbinNormalizeDescription(ref __instance, stringBuilder1);
        }

                

        public static void UpdateMaxMadnessChargesByItem(ref AuraCurseData __result, Character characterOfInterest, string itemID)
        {
            LogDebug("UpdateChargesByItem: " + itemID);

            if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemID+"rare", AppliesTo.Heroes) )
            {
                ItemData itemData = Globals.Instance.GetItemData(itemID+"rare");
                if (itemData == null)
                    return;

                if (__result.MaxCharges != -1)
                {
                    __result.MaxCharges += itemData.AuracurseCustomModValue1;
                }
                if (__result.MaxMadnessCharges != -1)
                {
                    __result.MaxMadnessCharges += itemData.AuracurseCustomModValue1;
                }
            } else if (IfCharacterHas(characterOfInterest, CharacterHas.Item, itemID, AppliesTo.Heroes) )
            {
                ItemData itemData = Globals.Instance.GetItemData(itemID);
                if (itemData == null)
                    return;

                if (__result.MaxCharges != -1)
                {
                    __result.MaxCharges += itemData.AuracurseCustomModValue1;
                }
                if (__result.MaxMadnessCharges != -1)
                {
                    __result.MaxMadnessCharges += itemData.AuracurseCustomModValue1;
                }
            }

        }

        

        public static string GetSpriteText(string sprite)
        {
            return $"<size=+.1><sprite name={sprite}></size>";
        }

        
        public static string ColorTextArray(string type, params string[] text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<nobr>");
            switch (type)
            {
                case "":
                    int num = 0;
                    foreach (string str in text)
                    {
                        if (num > 0)
                            stringBuilder.Append(" ");
                        stringBuilder.Append(str);
                        ++num;
                    }
                    if (type != "")
                        stringBuilder.Append("</color>");
                    stringBuilder.Append("</nobr> ");
                    return stringBuilder.ToString();
                case "damage":
                    stringBuilder.Append("<color=#B00A00>");
                    goto case "";
                case "heal":
                    stringBuilder.Append("<color=#1E650F>");
                    goto case "";
                case "aura":
                    stringBuilder.Append("<color=#263ABC>");
                    goto case "";
                case "curse":
                    stringBuilder.Append("<color=#720070>");
                    goto case "";
                case "system":
                    stringBuilder.Append("<color=#5E3016>");
                    goto case "";
                default:
                    stringBuilder.Append("<color=#5E3016");
                    stringBuilder.Append(">");
                    goto case "";
            }
        }

        
    }
}