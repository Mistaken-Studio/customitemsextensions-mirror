﻿// -----------------------------------------------------------------------
// <copyright file="MistakenCustomGrenade.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace Mistaken.API.CustomItems
{
    /// <inheritdoc/>
    public abstract class MistakenCustomGrenade : CustomGrenade, IMistakenCustomItem
    {
        /// <inheritdoc cref="CustomItem.Get(int)"/>
        public static CustomItem Get(MistakenCustomItems customItem)
            => Get((int)customItem) as MistakenCustomGrenade;

        /// <inheritdoc cref="CustomItem.TryGet(int, out Exiled.CustomItems.API.Features.CustomItem)"/>
        public static bool TryGet(MistakenCustomItems id, out MistakenCustomGrenade customItem)
        {
            customItem = null;
            if (!TryGet((int)id, out CustomItem customItem1))
                return false;
            customItem = customItem1 as MistakenCustomGrenade;
            return true;
        }

        /// <inheritdoc cref="CustomItem.TrySpawn(int, Vector3, out Pickup)"/>
        public static bool TrySpawn(MistakenCustomItems id, Vector3 position, out Pickup spawned)
            => TrySpawn((int)id, position, out spawned);

        /// <inheritdoc cref="CustomItem.TryGive(Exiled.API.Features.Player, int, bool)"/>
        public static bool TryGive(Exiled.API.Features.Player player, MistakenCustomItems id, bool displayMessage = true)
            => TryGive(player, (int)id, displayMessage);

        /// <inheritdoc/>
        public abstract MistakenCustomItems CustomItem { get; }

        /// <inheritdoc/>
        public override uint Id
        {
            get => (uint)this.CustomItem;
            set => _ = value;
        }
    }
}
