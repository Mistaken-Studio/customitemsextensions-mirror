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
        public override PluginPriority Priority => PluginPriority.Default;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(5, 0, 0);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();
            Mistaken.Events.Handlers.CustomEvents.LoadedPlugins += this.CustomEvents_LoadedPlugins;
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();
            Mistaken.Events.Handlers.CustomEvents.LoadedPlugins -= this.CustomEvents_LoadedPlugins;
            this.UnRegister();
        }

        private static readonly List<CustomItem> Registered = new List<CustomItem>();

        private void CustomEvents_LoadedPlugins() => this.Register();

        private void Register()
        {
            var toRegister = Exiled.Loader.Loader.Plugins.Where(x => x.Config.IsEnabled).SelectMany(x => x.Assembly.GetTypes()).Where(x => !x.IsAbstract && x.IsClass).Where(x => x.GetInterface(nameof(IMistakenCustomItem)) != null);
            Registered.AddRange(CustomItem.RegisterItems(toRegister));
            foreach (var item in Registered)
                Log.Debug($"Successfully registered {item.Name} ({item.Id})", this.Config.VerbouseOutput);

            if (Registered.Count < toRegister.Count())
                Log.Warn($"Successfully registered {Registered.Count}/{toRegister.Count()} CustomItems!");
        }

        private void UnRegister()
        {
            short unregisteredCount = 0;
            foreach (var item in CustomItem.UnregisterItems(Registered))
            {
                Log.Debug($"Successfully unregistered {item.Name} ({item.Id})", this.Config.VerbouseOutput);
                Registered.Remove(item);
                unregisteredCount++;
            }

            if (Registered.Count > 0)
                Log.Warn($"Successfully unregistered {Registered.Count}/{unregisteredCount} CustomItems!");
        }
    }
}
