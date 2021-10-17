// -----------------------------------------------------------------------
// <copyright file="MistakenCustomWeapon.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace Mistaken.API.CustomItems
{
    /// <inheritdoc/>
    public abstract class MistakenCustomWeapon : CustomWeapon, IMistakenCustomItem
    {
        /// <inheritdoc cref="CustomItem.Get(int)"/>
        public static CustomItem Get(MistakenCustomItems customItem)
            => Get((int)customItem) as MistakenCustomItem;

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
        /// Gets a value indicating whether changing should be allowed by default.
        /// Will not work if <see cref="OnChangingAttachments"/> is overriden and base function is not called.
        /// </summary>
        public virtual bool AllowChangingAttachments { get; } = true;

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Events.Handlers.CustomEvents.ChangingAttachments -= this.OnInternalChangingAttachments;
            Events.Handlers.CustomEvents.UnloadingFirearm -= this.OnInternalUnloadingFirearm;
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Events.Handlers.CustomEvents.ChangingAttachments += this.OnInternalChangingAttachments;
            Events.Handlers.CustomEvents.UnloadingFirearm += this.OnInternalUnloadingFirearm;
        }

        /// <inheritdoc cref="Events.Handlers.CustomEvents.ChangingAttachments"/>
        protected virtual void OnChangingAttachments(Events.EventArgs.ChangingAttachmentsEventArgs ev)
        {
            ev.IsAllowed = this.AllowChangingAttachments;
        }

        /// <inheritdoc cref="Events.Handlers.CustomEvents.UnloadingFirearm"/>
        protected virtual void OnUnloadingFirearm(Events.EventArgs.UnloadingFirearmEventArgs ev)
        {
        }

        private void OnInternalChangingAttachments(Events.EventArgs.ChangingAttachmentsEventArgs ev)
        {
            if (this.TrackedSerials.Contains(ev.Firearm.Serial))
                this.OnChangingAttachments(ev);
        }

        private void OnInternalUnloadingFirearm(Events.EventArgs.UnloadingFirearmEventArgs ev)
        {
            if (this.TrackedSerials.Contains(ev.Firearm.Serial))
                this.OnUnloadingFirearm(ev);
        }
    }
}
