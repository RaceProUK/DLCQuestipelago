﻿using System;
using System.Collections.Generic;
using Archipelago.Gifting.Net;
using Archipelago.MultiClient.Net;
using BepInEx.Logging;
using DLCQuestipelago.Archipelago;

namespace DLCQuestipelago.Gifting
{
    public class GiftHandler
    {
        private static readonly string[] parsableTraits = new[]
        {
            "Zombie", "Sheep", GiftFlag.Animal, GiftFlag.Monster
        };

        private readonly ManualLogSource _log;
        private readonly ArchipelagoSession _session;
        private readonly GiftingService _giftService;
        private readonly GiftSender _giftSender;
        private readonly GiftReceiver _giftReceiver;

        public GiftSender Sender => _giftSender;

        public GiftHandler(ManualLogSource log, ArchipelagoClient client)
        {
            _log = log;
            _session = client.Session;
            _giftService = new GiftingService(_session);
            _giftSender = new GiftSender(_log, client, _giftService);
            _giftReceiver = new GiftReceiver(_log, _giftService);
        }

        public void OpenGiftBox()
        {
            _giftService.OpenGiftBox(false, parsableTraits);
            _giftService.SubscribeToNewGifts(NewGiftNotification);
        }

        public void CloseGiftBox()
        {
            _giftService.CloseGiftBox();
        }

        public void NewGiftNotification(Dictionary<Guid, Gift> gifts)
        {
            _giftReceiver.ReceiveNewGifts(gifts);
        }
    }
}