// -----------------------------------------------------------------------
// <copyright file="ServerSendAudioMessagePatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HarmonyLib;
using InventorySystem.Items.Firearms;

namespace Mistaken.API.CustomItems
{
    [HarmonyPatch(typeof(FirearmExtensions), nameof(FirearmExtensions.ServerSendAudioMessage))]
    internal static class ServerSendAudioMessagePatch
    {
        public static bool Prefix(Firearm firearm, byte clipId)
        {
            var ev = new PlayingGunAudioEventArgs(firearm, clipId);
            EventHandler.OnPlayingGunAudio(ev);
            return ev.IsAllowed;
        }
    }
}
