// -----------------------------------------------------------------------
// <copyright file="PlayingGunAudioEventArgs.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using InventorySystem.Items.Firearms;

namespace Mistaken.API.CustomItems
{
    /// <summary>
    /// EventArgs for GunAudioMessage.
    /// </summary>
    public class PlayingGunAudioEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayingGunAudioEventArgs"/> class.
        /// </summary>
        /// <param name="firearm">Firearm.</param>
        /// <param name="clipId">Dlip id.</param>
        /// <param name="isAllowed">Is allowed.</param>
        public PlayingGunAudioEventArgs(Firearm firearm, byte clipId, bool isAllowed = true)
        {
            this.Firearm = firearm;
            this.ClipId = clipId;
            this.IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets firearm to get clip id from.
        /// </summary>
        public Firearm Firearm { get; set; }

        /// <summary>
        /// Gets or sets clip id of a gun.
        /// </summary>
        public byte ClipId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to send a GunAudioMessage.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
