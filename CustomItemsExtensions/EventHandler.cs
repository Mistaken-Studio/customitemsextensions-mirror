// -----------------------------------------------------------------------
// <copyright file="EventHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.Events.Extensions;
using static Exiled.Events.Events;

namespace Mistaken.API.CustomItems
{
    internal static class EventHandler
    {
        public static event CustomEventHandler<PlayingGunAudioEventArgs> PlayingGunAudio;

        public static void OnPlayingGunAudio(PlayingGunAudioEventArgs ev) => PlayingGunAudio.InvokeSafely(ev);
    }
}
