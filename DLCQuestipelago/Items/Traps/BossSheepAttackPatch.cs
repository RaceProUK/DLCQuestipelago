﻿
using System;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using DLCLib;
using DLCLib.Character;
using DLCLib.DLC;
using DLCQuestipelago.Archipelago;
using HarmonyLib;

namespace DLCQuestipelago.Items.Traps
{
    [HarmonyPatch(typeof(ShopkeepBossNPC))]
    [HarmonyPatch("PerformAttack")]
    internal class BossSheepAttackPatch
    {
        private static ManualLogSource _log;
        private static ArchipelagoClient _archipelago;
        private static Random _random;

        public static void Initialize(ManualLogSource log, ArchipelagoClient archipelago)
        {
            _log = log;
            _archipelago = archipelago;
            _random = new Random(int.Parse(archipelago.SlotData.Seed));
        }

        //protected override void PerformAttack()
        private static bool Prefix(ShopkeepBossNPC __instance)
        {
            var numberOfSheepReceived = _archipelago.GetReceivedItemCount(ItemParser.ZOMBIE_SHEEP);
            var sheepChance = (double)numberOfSheepReceived / 300.0;

            if (_random.NextDouble() < sheepChance)
            {
                ItemParser.SpawnZombieSheepOnPlayer();
                return false; // don't run original logic
            }

            return true; // run original logic
        }
    }
}