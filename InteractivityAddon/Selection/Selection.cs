﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Discord;
using Discord.WebSocket;
using InteractivityAddon.Actions;
using InteractivityAddon.Criterions;

namespace InteractivityAddon.Selection
{
    /// <summary>
    /// Represents a class which is used to send users selections into a <see cref="IMessageChannel"/> via a <see cref="InteractivityService"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects to select from.</typeparam>
    public sealed class Selection<T>
    {
        private readonly Embed selectionEmbed;

        /// <summary>
        /// Gets the values to select from in the <see cref="Selection{T}"/>.
        /// </summary>
        public ImmutableList<T> Values { get; }

        /// <summary>
        /// Gets the selection possibilities of the <see cref="Selection{T}"/>>.
        /// </summary>
        public ImmutableList<string> Possibilities { get; }

        /// <summary>
        /// Gets the <see cref="Embed"/> which is sent into the channel.
        /// </summary>
        public Embed SelectionEmbed => selectionEmbed.DeepClone();

        /// <summary>
        /// Gets the appearance of the <see cref="Selection{T}"/>.
        /// </summary>
        public SelectionAppearance Appearance { get; }

        /// <summary>
        /// Determites whether everyone can interact with the <see cref="Selection{T}"/>.
        /// </summary>
        public bool IsUserRestricted => Users.Count > 0;

        /// <summary>
        /// Gets which users can interact with the <see cref="Selection{T}"/>.
        /// </summary>
        public ImmutableList<SocketUser> Users { get; }

        /// <summary>
        /// Gets whether the <see cref="Selection{T}"/> allows cancellation.
        /// </summary>
        public bool AllowCancel { get; }

        /// <summary>
        /// Gets whether the <see cref="Selection{T}"/> is case sensitive.
        /// </summary>
        public bool IsCaseSensitive { get; }

        internal Selection(List<T> values, List<string> possibilities, Embed selection,
                              SelectionAppearance appearance, List<SocketUser> users,
                              bool allowCancel, bool isCaseSensitive)
        {
            selectionEmbed = selection;
            Values = values.ToImmutableList();
            Possibilities = possibilities.ToImmutableList();
            Appearance = appearance;
            Users = users.ToImmutableList();
            AllowCancel = allowCancel;
            IsCaseSensitive = isCaseSensitive;
        }

        internal Criteria<SocketMessage> GetCriteria()
        {
            var criteria = new Criteria<SocketMessage>();

            if (IsUserRestricted == true) {
                criteria.AddCriterion(new EnsureMessageUser(Users.ToArray()));
            }

            return criteria;
        }

        internal ActionCollection<SocketMessage> GetActions() => new ActionCollection<SocketMessage>(
            new DeleteMessages(Appearance.DeleteInvalidMessages, Appearance.DeleteValidMessage)
            );
    }
}
