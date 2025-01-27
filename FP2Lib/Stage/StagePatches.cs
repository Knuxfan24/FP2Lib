﻿using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Stage
{
    internal class StagePatches
    {
        //Patch World Map "Go to level" menu. This *actualy* handles whole logic for sending you to right level.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMapConfirm), "Start", MethodType.Normal)]
        static void PatchMenuWorldMapConfirm(ref string[] ___hubSceneToLoad, ref string[] ___sceneToLoad, ref FPHubNPC[] ___shopkeepers, ref GameObject[] ___shopMenus, ref Sprite[] ___stageIcon, ref Sprite[] ___hubIcon)
        {
            //Don't run code if there is nothing to add.
            if (StageHandler.Stages.Count > 0)
            {
                //Load in custom stages.
                foreach (CustomStage stage in StageHandler.Stages.Values)
                {
                    //Only registered stages should be loaded.
                    if (stage.id != 0)
                    {
                        //Extend hub array.
                        if (stage.isHUB)
                        {
                            for (int i = ___hubSceneToLoad.Length; i < stage.id + 1; i++)
                            {
                                ___hubSceneToLoad = ___hubSceneToLoad.AddToArray(string.Empty);
                            }
                            for (int i = ___shopkeepers.Length; i < stage.id + 1; i++)
                            {
                                ___shopkeepers = ___shopkeepers.AddToArray(null);
                            }
                            for (int i = ___shopMenus.Length; i < stage.id + 1; i++)
                            {
                                ___shopMenus = ___shopMenus.AddToArray(null);
                            }
                            for (int i = ___hubIcon.Length; i < stage.id + 1; i++)
                            {
                                ___hubIcon = ___hubIcon.AddToArray(null);
                            }                         
                        }
                        //Normal stages
                        else
                        {
                            for (int i = ___sceneToLoad.Length; i < stage.id + 1; i++)
                            {
                                ___sceneToLoad = ___sceneToLoad.AddToArray(null);
                            }
                            for (int i = ___stageIcon.Length; i < stage.id + 1; i++)
                            {
                                ___stageIcon = ___stageIcon.AddToArray(null);
                            }
                        }
                        //Add the stages
                        if (stage.registered)
                        {
                            if (stage.isHUB)
                            {
                                ___hubSceneToLoad[stage.id] = stage.sceneName;
                                ___shopkeepers[stage.id] = stage.shopkeeper;
                                ___shopMenus[stage.id] = stage.quickShop;
                                ___hubIcon[stage.id] = stage.preview;
                            }
                            else
                            {
                                ___sceneToLoad[stage.id] = stage.sceneName;
                                ___stageIcon[stage.id] = stage.preview;
                            }
                        }
                    }
                }
            }
        }

        //Extend time records array if needed
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "Awake", MethodType.Normal)]
        static void PatchFPSaveManagerAwake(ref int[] ___timeRecord)
        {
            //Don't run code if there is nothing to add.
            if (StageHandler.Stages.Count > 0)
            {
                foreach (CustomStage stage in StageHandler.Stages.Values)
                {
                    if (stage.id > ___timeRecord.Length)
                    {
                        FPSaveManager.ExpandIntArray(___timeRecord, stage.id + 1);
                    }
                }
            }
        }

        //Add stage name
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetStageName", MethodType.Normal)]
        static void PatchGetStageName(int stage, ref string __result)
        {
            if (stage > 32 && __result.IsNullOrWhiteSpace())
            {
                CustomStage customStage = StageHandler.getCustomStageByRuntimeId(stage);
                if (customStage != null)
                    __result = customStage.name;
            }
        }

        //Add hub name
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetHubName", MethodType.Normal)]
        static void PatchGetHubName(int hub, ref string __result)
        {
            if (hub > 14 && __result.IsNullOrWhiteSpace())
            {
                CustomStage customStage = StageHandler.getCustomHubByRuntimeId(hub);
                if (customStage != null)
                    __result = customStage.name;
            }
        }
    }
}
