// -----------------------------------------------------------------------
// <copyright file="MistakenCustomGrenade.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
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

        /// <summary>
        /// Gets a value indicating whether item is equipped.
        /// </summary>
        public bool IsEquiped { get; private set; }

        /// <summary>
        /// Gets display name shown on player's GUI.
        /// </summary>
        public virtual string DisplayName { get; }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.ChangingItem += this.OnInternalChangingItem;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Player.ChangingItem -= this.OnInternalChangingItem;
        }

        /// <summary>
        /// Fired when item is deequpied.
        /// </summary>
        /// <param name="ev">EventArgs.</param>
        protected virtual void OnHiding(ChangingItemEventArgs ev)
        {
        }

        /// <inheritdoc/>
        protected override void OnThrowing(ThrowingItemEventArgs ev)
        {
            this.IsEquiped = false;
            ev.Player.SetGUI($"CI_{this.Id}_HOLDING", PseudoGUIPosition.BOTTOM, null);
            base.OnThrowing(ev);
        }

        /// <inheritdoc/>
        protected override void OnDropping(DroppingItemEventArgs ev)
        {
            this.IsEquiped = false;
            ev.Player.SetGUI($"CI_{this.Id}_HOLDING", PseudoGUIPosition.BOTTOM, null);
            base.OnDropping(ev);
        }

        private void OnInternalChangingItem(Exiled.Events.EventArgs.ChangingItemEventArgs ev)
        {
            if (this.Check(ev.NewItem))
            {
                this.IsEquiped = true;
                if (this.DisplayName != null)
                    ev.Player.SetGUI($"CI_{this.Id}_HOLDING", PseudoGUIPosition.BOTTOM, $"<color=yellow>Trzymasz</color> {this.DisplayName}");
            }
            else
            {
                this.IsEquiped = false;
                this.OnHiding(ev);
                ev.Player.SetGUI($"CI_{this.Id}_HOLDING", PseudoGUIPosition.BOTTOM, null);
            }
        }
    }
}
