// -----------------------------------------------------------------------
// <copyright file="MistakenCustomWeapon.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using UnityEngine;

namespace Mistaken.API.CustomItems
{
    /// <inheritdoc/>
    public abstract class MistakenCustomWeapon : CustomWeapon, IMistakenCustomItem
    {
        /// <inheritdoc cref="CustomItem.Get(int)"/>
        public static CustomItem Get(MistakenCustomItems customItem)
            => Get((int)customItem) as MistakenCustomWeapon;

        /// <inheritdoc cref="CustomItem.TryGet(int, out Exiled.CustomItems.API.Features.CustomItem)"/>
        public static bool TryGet(MistakenCustomItems id, out MistakenCustomWeapon customItem)
        {
            customItem = null;
            if (!TryGet((int)id, out CustomItem customItem1))
                return false;

            customItem = customItem1 as MistakenCustomWeapon;
            return true;
        }

        /// <inheritdoc cref="CustomItem.TrySpawn(int, Vector3, out Pickup)"/>
        public static bool TrySpawn(MistakenCustomItems id, Vector3 position, out Pickup spawned)
            => TrySpawn((int)id, position, out spawned);

        /// <inheritdoc cref="CustomItem.TryGive(Player, int, bool)"/>
        public static bool TryGive(Player player, MistakenCustomItems id, bool displayMessage = true)
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
        /// Gets a value indicating whether changing should be allowed by default.
        /// Will not work if <see cref="OnChangingAttachments"/> is overriden and base function is not called.
        /// </summary>
        public virtual bool AllowChangingAttachments { get; } = true;

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
            Exiled.Events.Handlers.Item.ChangingAttachments += this.OnInternalChangingAttachments;
            Exiled.Events.Handlers.Player.UnloadingWeapon += this.OnInternalUnloadingWeapon;
            Exiled.Events.Handlers.Player.ChangingItem += this.OnInternalChangingItem;
            EventHandler.PlayingGunAudio += this.OnInternalPlayingGunAudio;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Item.ChangingAttachments -= this.OnInternalChangingAttachments;
            Exiled.Events.Handlers.Player.UnloadingWeapon -= this.OnInternalUnloadingWeapon;
            Exiled.Events.Handlers.Player.ChangingItem -= this.OnInternalChangingItem;
            EventHandler.PlayingGunAudio -= this.OnInternalPlayingGunAudio;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Item.ChangingAttachments"/>
        protected virtual void OnChangingAttachments(Exiled.Events.EventArgs.ChangingAttachmentsEventArgs ev)
        {
            ev.IsAllowed = this.AllowChangingAttachments;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.UnloadingWeapon"/>
        protected virtual void OnUnloadingWeapon(Exiled.Events.EventArgs.UnloadingWeaponEventArgs ev)
        {
        }

        /// <summary>
        /// Fired when item is deequpied.
        /// </summary>
        /// <param name="ev">EventArgs.</param>
        protected virtual void OnHiding(Exiled.Events.EventArgs.ChangingItemEventArgs ev)
        {
        }

        /// <summary>
        /// Fired when server is about to send GunAudioMessage.
        /// </summary>
        /// <param name="ev">EventArgs.</param>
        protected virtual void OnPlayingGunAudio(PlayingGunAudioEventArgs ev)
        {
        }

        /// <inheritdoc/>
        protected override void OnDropping(Exiled.Events.EventArgs.DroppingItemEventArgs ev)
        {
            this.IsEquiped = false;
            ev.Player.SetGUI($"CI_{this.Id}_HOLDING", PseudoGUIPosition.BOTTOM, null);
            base.OnDropping(ev);
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

        private void OnInternalChangingAttachments(Exiled.Events.EventArgs.ChangingAttachmentsEventArgs ev)
        {
            if (this.TrackedSerials.Contains(ev.Firearm.Serial))
                this.OnChangingAttachments(ev);
        }

        private void OnInternalUnloadingWeapon(Exiled.Events.EventArgs.UnloadingWeaponEventArgs ev)
        {
            if (this.TrackedSerials.Contains(ev.Firearm.Serial))
                this.OnUnloadingWeapon(ev);
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

        private void OnInternalPlayingGunAudio(PlayingGunAudioEventArgs ev)
        {
            if (this.Check(Item.Get(ev.Firearm)))
                this.OnPlayingGunAudio(ev);
        }
    }
}
