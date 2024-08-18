﻿using FP2Lib.Player;
using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuShop
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuShop), "Start", MethodType.Normal)]
        private static void PatchMenuShopStart(ref MenuText ___playerName, SpriteRenderer ___playerSprite)
        {
            if (FPSaveManager.character >= (FPCharacterID)5)
            {
                ___playerName.GetComponent<TextMesh>().text = PlayerHandler.currentCharacter.Name;
                ___playerSprite.sprite = PlayerHandler.currentCharacter.livesIconAnim[0];
            }
        }
    }
}
