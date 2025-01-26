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

        public static List<string> cardsWithCustomDescriptions = ["surprisebox", "surpriseboxrare", "surprisegiftbox", "surprisegiftboxrare","bbbtreefellingaxe","bbbtreefellingaxerare","bbbcloakofthorns","bbbcloakofthornsrare","bbbportablewallofflames","bbbportablewallofflamesrare","bbbslimepoison","bbbslimepoisonrare"];
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardData), nameof(CardData.SetDescriptionNew))]
        public static void SetDescriptionNewPostfix(ref CardData __instance, bool forceDescription = false, Character character = null, bool includeInSearch = true)
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
                // LogDebug("Acquired Description - " + __instance.Id);
                stringBuilder1.Append(Functions.FormatStringCard(Texts.Instance.GetText(descriptionId)));
                BinbinNormalizeDescription(ref __instance, stringBuilder1);
                return;
            }
            
            HandleAllDamagePercentDescriptions(ref __instance);

            string currentDescription = Globals.Instance.CardsDescriptionNormalized[__instance.Id];
            stringBuilder1.Append(currentDescription);

            if(__instance.Item!=null && __instance.Item.Activation==Enums.EventActivation.BeginTurnCardsDealt&&__instance.Item.ExactRound>=2){
                LogDebug($"Attempting to alter description for {__instance.Id}");
                // LogDebug($"Current description {__instance.Id}: {stringBuilder1}");
                stringBuilder1.Replace("Every turn",$"On turn {__instance.Item.ExactRound}");
                // LogDebug($"Description {__instance.Id} after replace: {stringBuilder1}");
            }
            if(__instance.Item!=null && __instance.Item.Activation==Enums.EventActivation.BeginRound&&__instance.Item.ExactRound>=2){
                LogDebug($"Attempting to alter description for {__instance.Id}");
                // LogDebug($"Current description {__instance.Id}: {stringBuilder1}");
                stringBuilder1.Replace("Every round",$"On round {__instance.Item.ExactRound}");
                // LogDebug($"Description {__instance.Id} after replace: {stringBuilder1}");
            }

            BinbinNormalizeDescription(ref __instance, stringBuilder1);

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
            if (itemData.Id=="battleaxerare")
            {
                LogDebug(" battleaxerare");
            }

            if (damageType == Enums.DamageType.None|| damageType == Enums.DamageType.All || percentIncrease == 0f)
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
            stringBuilder.Insert(0,toAdd);

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

            if (itemData.DamagePercentBonus==Enums.DamageType.None &&itemData.DamagePercentBonus2==Enums.DamageType.None &&itemData.DamagePercentBonus3==Enums.DamageType.None )
                return;

            if (__instance.Id=="burningorbrare"||__instance.Id=="frozenorbrare")
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AtOManager), nameof(AtOManager.GlobalAuraCurseModificationByTraitsAndItems))]
        public static void GlobalAuraCurseModificationByTraitsAndItemsPostfixGeneral(ref AtOManager __instance, ref AuraCurseData __result, string _type, string _acId, Character _characterCaster, Character _characterTarget)
        {
            Character characterOfInterest = _type == "set" ? _characterTarget : _characterCaster;
            string itemID;
            LogDebug("GACM for Balance Patch");

            // string[] items = ["bloodstone","bronzegear","bucket","crusaderhelmet","heartamulet","ironkanabo","lunaring","neverfrost","ringoffire","shieldofthorns","solring","thepolluter","thornyring","topazring","venomamulet","yggdrasilroot","yoggercleaver"];
            // foreach(string item in items)
            // {
            //     LogDebug("GACM item: "+item);
            //     ItemData itemData = Globals.Instance.GetItemData(item);
            //     ItemData itemDataRare = Globals.Instance.GetItemData(item+"rare");
            //     if ((itemData.AuracurseCustomAC.Id ==_acId||itemDataRare.AuracurseCustomAC.Id==_acId)&&itemData!=null&&itemDataRare!=null)
            //     {
            //         LogDebug($"GACM inside conditions: {item}");
            //         UpdateChargesByItem(ref __result, characterOfInterest,itemData);
            //     }
            //     LogDebug($"End GACM");
            // }

            switch (_acId)
            {
                case "bleed":
                    itemID = "bloodstone";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    itemID = "yoggercleaver";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);

                    if(IfCharacterHas(characterOfInterest,CharacterHas.Item,"bbbtreefellingaxe",AppliesTo.ThisHero)||IfCharacterHas(characterOfInterest,CharacterHas.Item,"bbbtreefellingaxerare",AppliesTo.ThisHero))
                    {
                        __result.Preventable = false;
                    }

                    break;
                case "bless":
                    itemID = "topazring";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    break;        
                case "block":
                    itemID = "crusaderhelmet";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    break;
                case "burn":
                    itemID = "solring";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    itemID = "ringoffire";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    break;       
                case "chill":
                    itemID = "lunaring";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    itemID = "neverfrost";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    break;       
                case "crack":
                    itemID = "bronzegear";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    itemID = "ironkanabo";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    break;       
                case "poison":
                    itemID = "thepolluter";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    itemID = "venomamulet";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);

                    if(IfCharacterHas(characterOfInterest,CharacterHas.Item,"bbbslimepoison",AppliesTo.ThisHero)||IfCharacterHas(characterOfInterest,CharacterHas.Item,"bbbslimepoisonrare",AppliesTo.ThisHero))
                    {
                        __result.Preventable = false;
                        // __result.Removable = false;
                    }
                    break; 
                case "sight":
                    itemID = "eeriering";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    break;
                case "thorns":
                    itemID = "corruptedplateb";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    itemID = "shieldofthorns";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    itemID = "thornyring";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    itemID = "yggdrasilroot";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    itemID = "bbbthehedgehog";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    itemID = "bbbphalanx";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    itemID = "heartofthorns";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    
                    if(IfCharacterHas(characterOfInterest,CharacterHas.Item,"bbbportablewallofflames",AppliesTo.ThisHero)||IfCharacterHas(characterOfInterest,CharacterHas.Item,"bbbportablewallofflamesrare",AppliesTo.ThisHero))
                    {
                        __result.DamageReflectedType = Enums.DamageType.Fire;
                    }
                    
                    break;
                case "vitality":
                    itemID = "heartamulet";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    break;     
                case "wet":
                    itemID = "bucket";
                    UpdateMaxMadnessChargesByItem(ref __result,characterOfInterest,itemID);
                    break;       
            }

        }

        public static void UpdateMaxMadnessChargesByItem(ref AuraCurseData __result, Character characterOfInterest, ItemData itemData)
        {
            // Updates Max Madness Charges
            if (itemData==null)
                return;
         
            
            string itemID = itemData.Id;
            LogDebug("UpdateChargesByItem: " + itemID);    
            if(IfCharacterHas(characterOfInterest,CharacterHas.Item,itemID,AppliesTo.Heroes)||IfCharacterHas(characterOfInterest,CharacterHas.Item,itemID+"rare",AppliesTo.Heroes))
                {
                    if (__result.MaxCharges!=-1)
                    {
                        __result.MaxCharges+=itemData.AuracurseCustomModValue1;
                    }
                    if(__result.MaxMadnessCharges!=-1)
                    {
                        __result.MaxMadnessCharges += itemData.AuracurseCustomModValue1;
                    }
                }

        }

        public static void UpdateMaxMadnessChargesByItem(ref AuraCurseData __result, Character characterOfInterest, string itemID)
        {
            LogDebug("UpdateChargesByItem: " + itemID);    
            
            if(IfCharacterHas(characterOfInterest,CharacterHas.Item,itemID,AppliesTo.Heroes)||IfCharacterHas(characterOfInterest,CharacterHas.Item,itemID+"rare",AppliesTo.Heroes))
                {
                    ItemData itemData = Globals.Instance.GetItemData(itemID);
                    if (itemData==null)
                        return;

                    if (__result.MaxCharges!=-1)
                    {
                        __result.MaxCharges+=itemData.AuracurseCustomModValue1;
                    }                        
                    if(__result.MaxMadnessCharges!=-1)
                    {
                        __result.MaxMadnessCharges += itemData.AuracurseCustomModValue1;
                    }
                }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), nameof(Character.IndirectDamage))]
        public static void IndirectDamagePostfix(
            ref Character __instance,            
            Enums.DamageType damageType,
            ref int damage,
            AudioClip sound = null,
            string effect = "",
            string sourceCharacterName = "",
            string sourceCharacterId = "")
        {
            // bbbcloakofthorns: Increases thorns damage by 20% per charge.
            PLog("IndirectDamagePostfix");
            if (MatchManager.Instance==null)
            {
                return;
            }

            Character sourceCharacter = MatchManager.Instance.GetCharacterById(sourceCharacterId);
            
            if(AtOManager.Instance.TeamHaveItem("bbbcloakofthornsrare")&&IsLivingHero(sourceCharacter)&&sourceCharacter.HasEffect("mitigate")&&effect=="thorns")
            {
                int n_bless = sourceCharacter.GetAuraCharges("mitigate");
                float multiplier = 1+0.25f*n_bless;
                damage *= Mathf.RoundToInt(damage*multiplier);
            }
            else if(AtOManager.Instance.TeamHaveItem("bbbcloakofthorns")&&IsLivingHero(sourceCharacter)&&sourceCharacter.HasEffect("mitigate")&&effect=="thorns")
            {
                int n_bless = sourceCharacter.GetAuraCharges("mitigate");
                float multiplier = 1+0.15f*n_bless;
                damage *= Mathf.RoundToInt(damage*multiplier);
            }

        }
    }
}