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
        public override PluginPriority Priority => PluginPriority.Default;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(4, 1, 2);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();
            this.harmony = new Harmony("com.customitemsextensions.patch");
            this.harmony.PatchAll();
            Mistaken.Events.Handlers.CustomEvents.LoadedPlugins += this.CustomEvents_LoadedPlugins;
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();
            this.harmony.UnpatchAll();
            Mistaken.Events.Handlers.CustomEvents.LoadedPlugins -= this.CustomEvents_LoadedPlugins;
            this.UnRegister();
        }

        private static readonly List<CustomItem> Registered = new List<CustomItem>();

        private Harmony harmony;

        private void CustomEvents_LoadedPlugins() => this.Register();

        private void Register()
        {
            foreach (var type in Exiled.Loader.Loader.Plugins.Where(x => x.Config.IsEnabled).SelectMany(x => x.Assembly.GetTypes()).Where(x => !x.IsAbstract && x.IsClass))
            {
                if (type.GetInterfaces().Any(x => x == typeof(IMistakenCustomItem)))
                {
                    var role = Activator.CreateInstance(type, true) as CustomItem;
                    if (role.TryRegister())
                    {
                        Log.Debug($"Successfully registered {role.Name} ({role.Id})", this.Config.VerbouseOutput);
                        Registered.Add(role);
                    }
                    else
                        Log.Warn($"Failed to register {role.Name} ({role.Id})");
                }
            }
        }

        private void UnRegister()
        {
            foreach (var role in Registered.ToArray())
            {
                if (role.TryUnregister())
                {
                    Log.Debug($"Successfully unregistered {role.Name} ({role.Id})", this.Config.VerbouseOutput);
                    Registered.Remove(role);
                }
                else
                    Log.Warn($"Failed to unregister {role.Name} ({role.Id})");
            }
        }
    }
}
