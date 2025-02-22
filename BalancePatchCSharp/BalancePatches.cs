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
using static UnofficialBalancePatch.BalanceFunctions;

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

        public static List<string> cardsWithCustomDescriptions = ["surprisebox", "surpriseboxrare", "surprisegiftbox", "surprisegiftboxrare", "bbbtreefellingaxe", "bbbtreefellingaxerare", "bbbcloakofthorns", "bbbcloakofthornsrare", "bbbportablewallofflames", "bbbportablewallofflamesrare", "bbbslimepoison", "bbbslimepoisonrare", "bbbscrollofpetimmortality", "bbbscrollofpetimmortalityrare"];
        public static List<string> cardsToAppendDescription = ["bbbrustedshield", "bbbrustedshieldrare"];
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
                // LogDebug($"Current description {__instance.Id}: {stringBuilder1}");

                string descriptionId = itemStem + __instance.Id;
                stringBuilder1.Append(Functions.FormatStringCard(Texts.Instance.GetText(descriptionId)));
                BinbinNormalizeDescription(ref __instance, stringBuilder1);
                return;
            }

            // Handles things like +10% Slashing Damage
            HandleAllDamagePercentDescriptions(ref __instance);


            if (!Globals.Instance.CardsDescriptionNormalized.ContainsKey(__instance.Id))
            {
                LogError($"missing card Id {__instance.Id}");
                return;
            }
            string currentDescription = Globals.Instance.CardsDescriptionNormalized[__instance.Id];
            stringBuilder1.Append(currentDescription);

            // Handles things like On Round 2 or On Turn 2
            if (__instance.Item != null && __instance.Item.Activation == Enums.EventActivation.BeginTurnCardsDealt && __instance.Item.ExactRound >= 2)
            {
                LogDebug($"Attempting to alter description for {__instance.Id}");
                LogDebug($"Current description {__instance.Id}: {stringBuilder1}");
                stringBuilder1.Replace("Every turn", $"On turn {__instance.Item.ExactRound}");
            }
            if (__instance.Item != null && __instance.Item.Activation == Enums.EventActivation.BeginRound && __instance.Item.ExactRound >= 2)
            {
                LogDebug($"Attempting to alter description for {__instance.Id}");
                // LogDebug($"Current description {__instance.Id}: {stringBuilder1}");
                stringBuilder1.Replace("Every round", $"On round {__instance.Item.ExactRound}");
            }

            // Handles "Heal Sides"
            if (__instance.HealSides != 0)
            {
                LogDebug($"Current description for {__instance.Id}: {stringBuilder1}");
                string healSprite = SpriteText("heal");
                string healAmount = ColorTextArray("heal", NumFormatItem(__instance.HealSides, plus: false));

                stringBuilder1.Append($"Heal sides {healAmount} {healSprite}");

            }

            if (__instance.EnergyRecharge != 0 && __instance.TargetSide == Enums.CardTargetSide.Enemy)
            {
                LogDebug($"Current description for {__instance.Id}: {stringBuilder1}");
                string energySprite = SpriteText("energy");
                // stringBuilder1.Replace($"Grant {energySprite}", $"Gain {energySprite}");
                stringBuilder1.Replace($"Grant", $"Gain");
            }

            if (__instance.Item != null && __instance.Item.CardNum > 1 && __instance.Item.CardToGainList.Count < 1)
            {
                LogDebug($"Current description for {__instance.Id}: {stringBuilder1}");
                stringBuilder1.Replace($"cast card", $"Cast card {__instance.Item.CardNum}");
            }

            if ((__instance.SpecialAuraCurseName1 != null && __instance.SpecialAuraCurseName1.Id == "stealthbonus") || (__instance.SpecialAuraCurseNameGlobal != null && __instance.SpecialAuraCurseNameGlobal.Id == "stealthbonus"))
            {
                LogDebug($"Current description for {__instance.Id}: {stringBuilder1}");
                stringBuilder1.Replace($"<sprite name=>", $"<sprite name=stealth>");
            }

            if (__instance.DamageSides > 0 && __instance.DamageSpecialValueGlobal)
            {
                LogDebug($"Current description for {__instance.Id}: {stringBuilder1}");

                Enums.DamageType damageType = __instance.DamageType;
                string oldText = "Target sides <nobr><color=#B00A00><size=+.1>1</size>"; //ColorTextArray("damage", "1", SpriteText(Enum.GetName(typeof(Enums.DamageType), damageType)));
                string newText = "Target sides <nobr><color=#B00A00><size=+.1>X</size>"; // ColorTextArray("damage", "X", SpriteText(Enum.GetName(typeof(Enums.DamageType), damageType)));
                stringBuilder1.Replace(oldText, newText);
            }

            if (__instance.DamageSides2 > 0 && __instance.Damage2SpecialValueGlobal)
            {
                LogDebug($"Current description for {__instance.Id}: {stringBuilder1}");

                Enums.DamageType damageType = __instance.DamageType2;
                string oldText = "Target sides <nobr><color=#B00A00><size=+.1>1</size>"; //ColorTextArray("damage", "1", SpriteText(Enum.GetName(typeof(Enums.DamageType), damageType)));
                string newText = "Target sides <nobr><color=#B00A00><size=+.1>X</size>"; // ColorTextArray("damage", "X", SpriteText(Enum.GetName(typeof(Enums.DamageType), damageType)));
                stringBuilder1.Replace(oldText, newText);
            }


            AppendDescriptionsToCards(__instance, ref stringBuilder1);

            BinbinNormalizeDescription(ref __instance, stringBuilder1);

        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(AtOManager), nameof(AtOManager.GlobalAuraCurseModificationByTraitsAndItems))]
        public static void GlobalAuraCurseModificationByTraitsAndItemsPostfixGeneral(ref AtOManager __instance, ref AuraCurseData __result, string _type, string _acId, Character _characterCaster, Character _characterTarget)
        {
            Character characterOfInterest = _type == "set" ? _characterTarget : _characterCaster;
            // Character notCharacterOfInterest = _type == "set" ? _characterCaster : _characterTarget;
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
            if (characterOfInterest==null || !characterOfInterest.Alive)
            {
                return;
            }

            switch (_acId)
            {
                
                case "bleed":
                    itemID = "bloodstone";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    itemID = "yoggercleaver";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);

                    if (IfCharacterHas(characterOfInterest, CharacterHas.Item, "bbbtreefellingaxe", AppliesTo.Monsters) || IfCharacterHas(characterOfInterest, CharacterHas.Item, "bbbtreefellingaxerare", AppliesTo.Monsters))
                    {
                        __result.Preventable = false;
                    }

                    break;
                case "bless":
                    itemID = "topazring";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    break;
                case "block":
                    itemID = "crusaderhelmet";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    break;
                case "burn":
                    itemID = "solring";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    itemID = "ringoffire";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);                    
                    break;
                case "chill":
                    itemID = "lunaring";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    itemID = "neverfrost";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    break;
                case "crack":
                    itemID = "bronzegear";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    itemID = "ironkanabo";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);                    

                    break;
                case "dark":
                    itemID = "blackpyramid";
                    if(IfCharacterHas(characterOfInterest,CharacterHas.Item, itemID+"rare",AppliesTo.Monsters))
                    {
                        __result.ExplodeAtStacks = 34;
                    }                    
                    else if(IfCharacterHas(characterOfInterest,CharacterHas.Item, itemID,AppliesTo.Monsters))
                    {
                        __result.ExplodeAtStacks = 30;
                    }
                    break;    
                case "mark":
                    itemID = "hellblade";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);                    
                    break;
                case "poison":
                    itemID = "thepolluter";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    itemID = "venomamulet";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);

                    if (IfCharacterHas(characterOfInterest, CharacterHas.Item, "bbbslimepoison", AppliesTo.Monsters) || IfCharacterHas(characterOfInterest, CharacterHas.Item, "bbbslimepoisonrare", AppliesTo.Monsters))
                    {
                        __result.Preventable = false;
                        // __result.Removable = false;
                    }
                    break;
                case "powerful":
                    itemID = "mysticstaff";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);                    
                    break;
                case "sight":
                    itemID = "eeriering";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    break;
                case "thorns":
                    itemID = "corruptedplateb";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    itemID = "shieldofthorns";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    itemID = "thornyring";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    itemID = "yggdrasilroot";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    itemID = "bbbthehedgehog";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    itemID = "bbbphalanx";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    itemID = "heartofthorns";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);

                    if (IfCharacterHas(characterOfInterest, CharacterHas.Item, "bbbportablewallofflames", AppliesTo.ThisHero) || IfCharacterHas(characterOfInterest, CharacterHas.Item, "bbbportablewallofflamesrare", AppliesTo.ThisHero))
                    {
                        __result.DamageReflectedType = Enums.DamageType.Fire;
                    }

                    break;
                case "vitality":
                    itemID = "heartamulet";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    itemID = "bbbsausagelinknecklace";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    break;
                case "wet":
                    itemID = "bucket";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    itemID = "waterskin";
                    UpdateMaxMadnessChargesByItem(ref __result, characterOfInterest, itemID);
                    break;
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
            if (MatchManager.Instance == null)
            {
                return;
            }

            Character sourceCharacter = MatchManager.Instance.GetCharacterById(sourceCharacterId);

            if (AtOManager.Instance.TeamHaveItem("bbbcloakofthornsrare") && IsLivingHero(sourceCharacter) && sourceCharacter.HasEffect("mitigate") && effect == "thorns")
            {
                int nMitigate = sourceCharacter.GetAuraCharges("mitigate");
                float multiplier = 1 + 0.25f * nMitigate;
                damage = Mathf.RoundToInt(damage * multiplier);
            }
            else if (AtOManager.Instance.TeamHaveItem("bbbcloakofthorns") && IsLivingHero(sourceCharacter) && sourceCharacter.HasEffect("mitigate") && effect == "thorns")
            {
                int nMitigate = sourceCharacter.GetAuraCharges("mitigate");
                float multiplier = 1 + 0.15f * nMitigate;
                damage = Mathf.RoundToInt(damage * multiplier);
            }
        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(MatchManager), nameof(MatchManager.DestroyedItemInThisTurn))]

        public static bool DestroyedItemInThisTurnPrefix(MatchManager __instance, int _charIndex, string _cardId)
        {
            LogDebug("DestroyedItemInThisTurnPrefix");
            Hero targetHero = MatchManager.Instance.GetHero(_charIndex);
            if (targetHero == null) { return true; }
            if (targetHero.HaveItem("bbbscrollofpetimmortality"))
            {
                LogDebug("Protecting Pet!");
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MatchManager), nameof(MatchManager.CreatePet))]

        public static bool CreatePet(MatchManager __instance,
            CardData cardPet,
            GameObject charGO,
            Hero _hero,
            NPC _npc,
            bool _fromEnchant = false,
            int _enchantIndex = -1)

        {
            LogDebug("CreatePet");

            if (cardPet == null) { return true; }
            // CardData tombstone = Globals.Instance.GetCardData("tombstone", false);
            if (cardPet.Id == "tombstone" && _hero.HaveItem("bbbscrollofpetimmortality"))
            {
                LogDebug("Protecting Pet!");
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Item), nameof(Item.DoItem))]

        public static void DoItemPrefix(
            Enums.EventActivation _theEvent,
            CardData _cardData,
            string _item,
            Character _character,
            Character _target,
            int _auxInt,
            string _auxString,
            int order,
            CardData castedCard,
            ref bool onlyCheckItemActivation)
        {
            {
                LogDebug("DoItemPrefix");
                if(MatchManager.Instance==null) {return;}
                List<string> turn3Items = ["surprisebox", "surpriseboxrare", "surprisegiftbox", "surprisegiftboxrare"];
                if (turn3Items.Contains(_item))
                {
                    LogDebug("DoItemPrefix - found surpriseboxes");
                }

                if(MatchManager.Instance.GetCurrentRound()!=3 && turn3Items.Contains(_item))
                {
                    LogDebug("DoItemPrefix - disabling surpriseboxes");
                    onlyCheckItemActivation = true;
                }
                else
                {
                    // onlyCheckItemActivation = false;
                }                
            }
        }


            [HarmonyPrefix]
            [HarmonyPatch(typeof(Item), "DoItemData")]

            public static void DoItemDataPrefix(
                ref Item __instance,
                Character target,
                string itemName,
                int auxInt,
                CardData cardItem,
                ref string itemType,
                ItemData itemData,
                Character character,
                int order,
                string castedCardId = "",
                Enums.EventActivation theEvent = Enums.EventActivation.None
            )
            {
                LogDebug("DoItemDataPrefix");
                if (itemData != null && (itemData.Id == "bbbfirestarter" || itemData.Id == "bbbfirestarterrare"))
                {
                    LogDebug("Changing Firestarter");
                    itemData.CardToGainType = Enums.CardType.Fire_Spell;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(MatchManager), nameof(MatchManager.CastCardAction))]

            public static void CastCardActionPostfix(
                MatchManager __instance,
                CardData _cardActive,
                Transform targetTransformCast,
                CardItem theCardItem,
                string _uniqueCastId,
                bool _automatic = false,
                CardData _card = null,
                int _cardIterationTotal = 1)
            {
                LogDebug("CastCardActionPostfix");
                if (_cardActive != null)
                {

                    LogDebug($"Casted Card - {_cardActive.Id}");
                    Hero heroActive = __instance.GetHeroHeroActive();
                    if (_cardActive != null && _cardActive.EnergyRecharge > 0 && IsLivingHero(heroActive) && _cardActive.TargetSide == Enums.CardTargetSide.Enemy)
                    {
                        LogDebug($"Energy Recharge - giving energy - {_cardActive.EnergyRecharge}");
                        int energyToGain = _cardActive.EffectRepeat != 0 ? _cardActive.EnergyRecharge * _cardActive.EffectRepeat : _cardActive.EnergyRecharge;
                        heroActive.ModifyEnergy(energyToGain, true);
                    }
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Character), nameof(Character.DamageReflected))]

            public static void DamageReflectedPostfix(ref Character __instance, Hero theCasterHero, NPC theCasterNPC)
            {
                if (IsLivingHero(__instance) || theCasterHero == null)
                    return;

                LogDebug("DamageReflectedPostfix");
                AuraCurseData acData = GetAuraCurseData("thorns");
                if (acData == null || acData.DamageReflectedPerStack <= 0 || theCasterNPC == null)
                {
                    LogDebug("DamageReflectedPostfix - Null thorns data");
                    return;
                }
                if (IfCharacterHas(__instance, CharacterHas.Item, "bbbrustedshieldrare", AppliesTo.ThisHero))
                {
                    LogDebug("DamageReflectedPostfix - Applying bbbrustedshieldrare Poison");

                    theCasterNPC.SetAura(__instance, GetAuraCurseData("poison"), Functions.FuncRoundToInt((float)__instance.GetAuraCharges("thorns") * 0.75f));
                }
                else if (IfCharacterHas(__instance, CharacterHas.Item, "bbbrustedshield", AppliesTo.ThisHero) && __instance.HasEffect("rust"))
                {
                    LogDebug("DamageReflectedPostfix - Applying bbbrustedshield Poison");
                    theCasterNPC.SetAura(__instance, GetAuraCurseData("poison"), Functions.FuncRoundToInt((float)__instance.GetAuraCharges("thorns") * 0.5f));
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Functions), nameof(Functions.GetRandomCardIdByTypeAndRandomRarity))]

            public static bool GetRandomCardIdByTypeAndRandomRarityPrefix(ref string __result, Enums.CardType _cardType)
            {
                LogDebug("GetRandomCardIdByTypeAndRandomRarityPrefix");
                CardData cardData = Globals.Instance.GetCardData(Globals.Instance.CardListByType[_cardType][MatchManager.Instance.GetRandomIntRange(0, Globals.Instance.CardListByType[_cardType].Count)], false);
                __result = Functions.GetCardByRarity(MatchManager.Instance.GetRandomIntRange(0, 100), cardData);
                return false;
            }

        }
    }