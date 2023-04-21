﻿using System;
using System.Diagnostics;
using Awardments;
using BepInEx.Logging;
using HarmonyLib;

namespace DLCQuestipelago.Locations
{
    [HarmonyPatch(typeof(AwardmentUtil))]
    [HarmonyPatch("AwardDiedInPitAwardment")]
    public static class NotExactlyNoblePatch
    {
        private const string ACHIEVEMENT_NAME = $"Not Exactly Noble";
        private static ManualLogSource _log;
        private static LocationChecker _locationChecker;

        public static void Initialize(ManualLogSource log, LocationChecker locationChecker)
        {
            _log = log;
            _locationChecker = locationChecker;
        }

        //internal static void AwardDiedInPitAwardment()
        public static void Postfix()
        {
            try
            {
                _log.LogInfo(ACHIEVEMENT_NAME);
                _locationChecker.AddCheckedLocation(ACHIEVEMENT_NAME);
            }
            catch (Exception ex)
            {
                _log.LogError($"Failed in {nameof(NotExactlyNoblePatch)}.{nameof(Postfix)}:\n\t{ex}");
                Debugger.Break();
                return;
            }
        }
    }
}
