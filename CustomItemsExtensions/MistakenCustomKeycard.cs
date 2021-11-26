// -----------------------------------------------------------------------
// <copyright file="MistakenCustomKeycard.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.Events.EventArgs;
using Interactables.Interobjects.DoorUtils;

namespace Mistaken.API.CustomItems
{
    /// <inheritdoc/>
    public abstract class MistakenCustomKeycard : MistakenCustomItem, IMistakenCustomItem
    {
        /// <summary>
        /// Gets or sets Keycards custom permissions.
        /// Set to <see langword="null"/> to ignore and use base keycard's permissions.
        /// </summary>
        public virtual KeycardPermissions? KeycardPermissions { get; set; } = null;

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.InteractingDoor += this.OnInternalInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker += this.OnInternalInteractingLocker;
            Exiled.Events.Handlers.Player.UnlockingGenerator += this.OnInternalUnlockingGenerator;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Player.InteractingDoor -= this.OnInternalInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker -= this.OnInternalInteractingLocker;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= this.OnInternalUnlockingGenerator;
        }

        /// <summary>
        /// Fired when player is unlocking generator with custom item.
        /// </summary>
        /// <param name="ev">EventArgs.</param>
        protected virtual void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
        }

        /// <summary>
        /// Fired when player is interacting with locker using custom item.
        /// </summary>
        /// <param name="ev">EventArgs.</param>
        protected virtual void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
        }

        /// <summary>
        /// Fired when player is interacting with door using custom item.
        /// </summary>
        /// <param name="ev">EventArgs.</param>
        protected virtual void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
        }

        private void OnInternalUnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (!this.Check(ev.Player.CurrentItem))
                return;

            if (this.KeycardPermissions.HasValue)
                ev.IsAllowed = (this.KeycardPermissions & ev.Generator._requiredPermission) != 0;

            this.OnUnlockingGenerator(ev);
        }

        private void OnInternalInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (!this.Check(ev.Player.CurrentItem))
                return;

            if (this.KeycardPermissions.HasValue)
                ev.IsAllowed = (this.KeycardPermissions & ev.Chamber.RequiredPermissions) != 0;

            this.OnInteractingLocker(ev);
        }

        private void OnInternalInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!this.Check(ev.Player.CurrentItem))
                return;

            if (this.KeycardPermissions.HasValue)
                ev.IsAllowed = (this.KeycardPermissions & ev.Door.RequiredPermissions.RequiredPermissions) != 0;

            this.OnInteractingDoor(ev);
        }
    }
}
