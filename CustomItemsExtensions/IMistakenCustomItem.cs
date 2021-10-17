﻿// -----------------------------------------------------------------------
// <copyright file="IMistakenCustomItem.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Mistaken.API.CustomItems
{
    /// <summary>
    /// Interace used to mark Mistaken's Custom Items.
    /// </summary>
    public interface IMistakenCustomItem
    {
        /// <summary>
        /// Gets custom item.
        /// </summary>
        MistakenCustomItems CustomItem { get; }
    }
}
