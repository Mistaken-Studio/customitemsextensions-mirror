// -----------------------------------------------------------------------
// <copyright file="MistakenCustomItem.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.EventArgs;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using UnityEngine;

namespace Mistaken.API.CustomItems
{
    /// <inheritdoc/>
    public abstract class MistakenCustomItem : CustomItem, IMistakenCustomItem
    {
        /// <inheritdoc cref="CustomItem.Get(int)"/>
        public static CustomItem Get(MistakenCustomItems customItem)
            => Get((int)customItem);

        /// <inheritdoc cref="CustomItem.TryGet(int, out Exiled.CustomItems.API.Features.CustomItem)"/>
        public static bool TryGet(MistakenCustomItems id, out MistakenCustomItem customItem)
        {
            customItem = null;
            if (!TryGet((int)id, out CustomItem customItem1))
                return false;
            customItem = customItem1 as MistakenCustomItem;
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
            this.ClearGui(ev.Player);
        }

        /// <inheritdoc/>
        protected override void OnDropping(DroppingItemEventArgs ev)
        {
            // this.IsEquiped = false;
            // ev.Player.SetGUI($"CI_{this.Id}_HOLDING", PseudoGUIPosition.BOTTOM, null);
            this.ClearGui(ev.Player);
            base.OnDropping(ev);
        }

        protected override void OnOwnerChangingRole(OwnerChangingRoleEventArgs ev)
        {
            this.ClearGui(ev.Player);
            base.OnOwnerChangingRole(ev);
        }

        protected override void OnOwnerDying(OwnerDyingEventArgs ev)
        {
            this.ClearGui(ev.Target);
            base.OnOwnerDying(ev);
        }

        protected override void OnOwnerEscaping(OwnerEscapingEventArgs ev)
        {
            this.ClearGui(ev.Player);
            base.OnOwnerEscaping(ev);
        }

        protected override void OnOwnerHandcuffing(OwnerHandcuffingEventArgs ev)
        {
            this.ClearGui(ev.Target);
            base.OnOwnerHandcuffing(ev);
        }

        /// <inheritdoc/>
        protected override void ShowSelectedMessage(Player player)
        {
            if (this.DisplayName is null)
                base.ShowSelectedMessage(player);
        }

        /// <inheritdoc/>
        protected override void ShowPickedUpMessage(Player player)
        {
            if (this.DisplayName is null)
                base.ShowPickedUpMessage(player);
        }

        private void ClearGui(Player player)
        {
            this.IsEquiped = false;
            player.SetGUI($"CI_{this.Id}_HOLDING", PseudoGUIPosition.BOTTOM, null);
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
