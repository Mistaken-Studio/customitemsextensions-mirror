// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using HarmonyLib;

namespace Mistaken.API.CustomItems
{
    /// <inheritdoc/>
    internal class PluginHandler : Plugin<Config>
    {
        /// <inheritdoc/>
        public override string Author => "Mistaken Devs";

        /// <inheritdoc/>
        public override string Name => "Custom Items Extensions";

        /// <inheritdoc/>
        public override string Prefix => "MCustomItemsExt";

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.Default + 2;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(5, 0, 0);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();
            this.harmony = new Harmony("com.customitemsextensions.patch");
            this.harmony.PatchAll();
            Mistaken.Events.Handlers.CustomEvents.LoadedPlugins += this.Register;
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();
            this.harmony.UnpatchAll();
            Mistaken.Events.Handlers.CustomEvents.LoadedPlugins -= this.Register;
            this.UnRegister();
        }

        private static readonly List<CustomItem> Registered = new List<CustomItem>();

        private Harmony harmony;

        private void Register()
        {
            Registered.AddRange(Extensions.RegisterItems());
            foreach (var item in Registered)
                Log.Debug($"Successfully registered {item.Name} ({item.Id})", this.Config.VerbouseOutput);
        }

        private void UnRegister()
        {
            foreach (var item in CustomItem.UnregisterItems(Registered))
            {
                Log.Debug($"Successfully unregistered {item.Name} ({item.Id})", this.Config.VerbouseOutput);
                Registered.Remove(item);
            }
        }
    }
}
