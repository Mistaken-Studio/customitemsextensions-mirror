// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomItems.API.Features;
using HarmonyLib;
using Mistaken.API.Extensions;
using Mistaken.Updater.API.Config;

namespace Mistaken.API.CustomItems
{
    internal sealed class PluginHandler : Plugin<Config>, IAutoUpdateablePlugin
    {
        public override string Author => "Mistaken Devs";

        public override string Name => "Custom Items Extensions";

        public override string Prefix => "MCustomItemsExt";

        public override PluginPriority Priority => PluginPriority.Default + 2;

        public override Version RequiredExiledVersion => new(5, 2, 2);

        public AutoUpdateConfig AutoUpdateConfig => new()
        {
            Type = SourceType.GITLAB,
            Url = "https://git.mistaken.pl/api/v4/projects/66",
        };

        public override void OnEnabled()
        {
            _harmony.PatchAll();

            Mistaken.Events.Handlers.CustomEvents.LoadedPlugins += this.Register;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            _harmony.UnpatchAll();

            Mistaken.Events.Handlers.CustomEvents.LoadedPlugins -= this.Register;
            this.UnRegister();

            base.OnDisabled();
        }

        private static readonly List<CustomItem> _registered = new();

        private static readonly Harmony _harmony = new("com.customitemsextensions.patch");

        private void Register()
        {
            _registered.AddRange(this.RegisterItems());
            foreach (var item in _registered)
                Log.Debug($"Successfully registered {item.Name} ({item.Id})", this.Config.VerboseOutput);
        }

        private void UnRegister()
        {
            foreach (var item in CustomItem.UnregisterItems(_registered))
            {
                Log.Debug($"Successfully unregistered {item.Name} ({item.Id})", this.Config.VerboseOutput);
                _registered.Remove(item);
            }
        }

        private IEnumerable<CustomItem> RegisterItems()
        {
            List<CustomItem> registeredItems = new();
            foreach (Type type in Exiled.Loader.Loader.Plugins.Where(x => x.Config.IsEnabled).SelectMany(x => x.Assembly.GetLoadableTypes()).Where(x => !x.IsAbstract && x.IsClass).Where(x => x.GetInterface(nameof(IMistakenCustomItem)) != null))
            {
                if (!type.IsSubclassOf(typeof(CustomItem)) || type.GetCustomAttribute(typeof(CustomItemAttribute)) is null)
                    continue;

                foreach (var attribute in (Attribute[])type.GetCustomAttributes(typeof(CustomItemAttribute), true))
                {
                    try
                    {
                        CustomItem customItem = (CustomItem)Activator.CreateInstance(type);
                        customItem.Type = ((CustomItemAttribute)attribute).ItemType;
                        customItem.GetType().GetMethod("TryRegister", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(customItem, null);
                        registeredItems.Add(customItem);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }
            }

            return registeredItems;
        }
    }
}
