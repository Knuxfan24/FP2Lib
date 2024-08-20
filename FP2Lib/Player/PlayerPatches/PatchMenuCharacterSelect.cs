﻿using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuCharacterSelect
    {
        internal static readonly MethodInfo m_getTotalWheelNumber = SymbolExtensions.GetMethodInfo(() => GetTotalWheelNumber());
        internal static int wheelcharas;

        internal static int GetTotalWheelNumber()
        {
            return wheelcharas;
        }

        public static PlayableChara GetPlayableCharaByWheelId(int id)
        {
            foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
            {
                if (chara.wheelId == id) return chara;
            }
            return null;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuCharacterSelect), "Start", MethodType.Normal)]
        static void PatchCharacterSelectStart(MenuCharacterSelect __instance, ref MenuCharacterWheel[] ___characterSprites, ref Sprite[] ___nameLabelSprites, ref MenuText[] ___infoText)
        {
            //Reset character number for the wheel
            wheelcharas = 3;

            //Warn the gamer on anything sus
            PlayerHandler.doWeHaveHolesInIds();

            //Extend arrays
            for (int i = 4; i < PlayerHandler.highestID; i++)
            {
                ___characterSprites = ___characterSprites.AddToArray(null);
                ___nameLabelSprites = ___nameLabelSprites.AddToArray(null);
                ___infoText[0].paragraph = ___infoText[0].paragraph.AddToArray("");
                ___infoText[1].paragraph = ___infoText[1].paragraph.AddToArray("");
                ___infoText[2].paragraph = ___infoText[2].paragraph.AddToArray("");
                ___infoText[3].paragraph = ___infoText[3].paragraph.AddToArray("");
                ___infoText[4].paragraph = ___infoText[4].paragraph.AddToArray("");
            }


            //Inject all characters
            foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
            {
                //No character file, skip them. Same for chars without adventure mode.
                if (chara.prefab == null || (!chara.enabledInAventure && FPSaveManager.gameMode == FPGameMode.ADVENTURE)) continue;

                GameObject charSelector = GameObject.Instantiate(chara.characterSelectPrefab);
                MenuCharacterWheel wheel = charSelector.GetComponent<MenuCharacterWheel>();
                wheelcharas++;
                chara.wheelId = wheelcharas;
                //Calculate offset for the wheel.
                //Maybe this will just work?
                //wheel.rotationOffset = 180 - (offset * chara.wheelId);

                wheel.parentObject = __instance;
                wheel.gameObject.transform.parent = __instance.transform;
                ___characterSprites[chara.wheelId] = wheel;
                ___nameLabelSprites[chara.wheelId] = chara.charSelectName;

                ___infoText[0].paragraph[chara.wheelId] = chara.characterType;
                ___infoText[1].paragraph[chara.wheelId] = chara.skill1;
                ___infoText[2].paragraph[chara.wheelId] = chara.skill2;
                ___infoText[3].paragraph[chara.wheelId] = chara.skill3;
                ___infoText[4].paragraph[chara.wheelId] = chara.skill4;
            }
            //Calculate offset
            int totalCharacters = wheelcharas + 1;
            int offset = 360 / totalCharacters;

            //Strip nulls
            ___characterSprites = ___characterSprites.Where(x => x != null).ToArray();
            ___nameLabelSprites = ___nameLabelSprites.Where(x => x != null).ToArray();

            //Set offset
            for (int i = 4; i <= wheelcharas; i++)
            {
                ___characterSprites[i].rotationOffset = 180 - (offset * i);
            }
        }

        //Bump the maximum number of characters
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuCharacterSelect), "State_SelectCharacter", MethodType.Normal)]
        static IEnumerable<CodeInstruction> CharacterSelectTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if ((codes[i].opcode == OpCodes.Ldc_I4_3 && codes[i - 1].opcode == OpCodes.Ldfld) || (codes[i].opcode == OpCodes.Ldc_I4_3 && codes[i - 1].opcode == OpCodes.Ldarg_0))
                {
                    codes[i] = new CodeInstruction(OpCodes.Call, m_getTotalWheelNumber);
                }
            }
            return codes;
        }


        //Patch digitframes to show live icon of selected character
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuCharacterSelect), "State_CharacterConfirm", MethodType.Normal)]
        static void PatchCharacterConfirm(MenuCharacterSelect __instance, ref FPHudDigit ___characterIcon)
        {
            //Since BikeCarol is not counted here, all id's need to be +1'd
            if (__instance.character >= 4)
            {
                ___characterIcon.digitFrames = ___characterIcon.digitFrames.AddToArray(GetPlayableCharaByWheelId(__instance.character).livesIconAnim[0]);
                ___characterIcon.SetDigitValue(16);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuCharacterSelect), "State_Go", MethodType.Normal)]
        static void PatchMenuCharacterSelectGo(int ___character)
        {
            if (___character >= 4)
            {
                PlayableChara chara = GetPlayableCharaByWheelId(___character);
                FPSaveManager.character = (FPCharacterID)(chara.id);
                PlayerHandler.currentCharacter = chara;
            }
        }
    }
}
