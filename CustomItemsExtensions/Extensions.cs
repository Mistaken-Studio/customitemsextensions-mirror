// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace Mistaken.API.CustomItems
{
    /// <summary>
    /// Custom Items Extensions.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Checks if <paramref name="item"/> is custom item made by Mistaken Devs and has it <paramref name="itemType"/>.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <param name="itemType">Item Id.</param>
        /// <returns>Result.</returns>
        public static bool IsCustomItem(this Item item, MistakenCustomItems itemType)
        {
            if (!CustomItem.TryGet(item, out var customItem))
                return false;
            if (!(customItem is IMistakenCustomItem mistakenItem))
                return false;
            if (mistakenItem.CustomItem == itemType)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if <paramref name="pickup"/> is custom item made by Mistaken Devs and has it <paramref name="itemType"/>.
        /// </summary>
        /// <param name="pickup">Item to check.</param>
        /// <param name="itemType">Item Id.</param>
        /// <returns>Result.</returns>
        public static bool IsCustomItem(this Pickup pickup, MistakenCustomItems itemType)
        {
            if (!CustomItem.TryGet(pickup, out var customItem))
                return false;
            if (!(customItem is IMistakenCustomItem mistakenItem))
                return false;
            if (mistakenItem.CustomItem == itemType)
                return true;

            return false;
        }

        /// <inheritdoc cref="CustomItem.Get(int)"/>
        public static CustomItem Get(this MistakenCustomItems customItem)
            => CustomItem.Get((int)customItem);

        /// <inheritdoc cref="CustomItem.TryGet(int, out CustomItem)"/>
        public static bool TryGet(this MistakenCustomItems id, out CustomItem customItem)
            => CustomItem.TryGet((int)id, out customItem);

        /// <inheritdoc cref="CustomItem.TrySpawn(int, Vector3, out Pickup)"/>
        public static bool TrySpawn(this MistakenCustomItems id, Vector3 position, out Pickup spawned)
            => CustomItem.TrySpawn((int)id, position, out spawned);

        /// <inheritdoc cref="CustomItem.TryGive(Player, int, bool)"/>
        public static bool TryGive(this MistakenCustomItems id, Player player, bool displayMessage = true)
            => CustomItem.TryGive(player, (int)id, displayMessage);

        /// <inheritdoc cref="CustomItem.RegisterItems(bool, object)"/>
        public static IEnumerable<CustomItem> RegisterItems()
        {
            List<CustomItem> registeredItems = new List<CustomItem>();
            foreach (Type type in Exiled.Loader.Loader.Plugins.Where(x => x.Config.IsEnabled).SelectMany(x => x.Assembly.GetTypes()).Where(x => !x.IsAbstract && x.IsClass).Where(x => x.GetInterface(nameof(IMistakenCustomItem)) != null))
            {
                if (!type.IsSubclassOf(typeof(CustomItem)) || type.GetCustomAttribute(typeof(CustomItemAttribute)) is null)
                    continue;

                foreach (Attribute attribute in type.GetCustomAttributes(typeof(CustomItemAttribute), true))
                {
                    try
                    {
                        CustomItem customItem = (CustomItem)Activator.CreateInstance(type);
                        customItem.Type = ((CustomItemAttribute)attribute).ItemType;
                        customItem.GetType().GetMethod("TryRegister", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(customItem, new object[0]);
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
