﻿using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    class PatchZLBaseballFlyer
    {

        private static void SetBaseballSprite(SpriteRenderer spriteRenderer)
        {
            spriteRenderer.sprite = PlayerHandler.currentCharacter.zaoBaseballSprite;
        }

        //Set our own custom sprite for ZL Baseball passenger
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(ZLBaseballFlyer), "State_Target", MethodType.Normal)]
        static IEnumerable<CodeInstruction> ZLBaseballTargetTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label entryLabel = il.DefineLabel();
            Label exitLabel = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_2)
                {
                    exitLabel = (Label)codes[i + 1].operand;
                    codes[i + 1].operand = entryLabel;
                }
            }
            CodeInstruction entry = new CodeInstruction(OpCodes.Ldarg_0);
            entry.labels.Add(entryLabel);
            codes.Add(entry);
            codes.Add(CodeInstruction.LoadField(typeof(ZLBaseballFlyer), "spriteRenderer"));
            codes.Add(CodeInstruction.Call(typeof(PatchZLBaseballFlyer), nameof(SetBaseballSprite)));
            codes.Add(new CodeInstruction(OpCodes.Br, exitLabel));
            return codes;
        }
    }
}
