﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Perspex.Controls.Templates;

namespace Perspex.Controls.Generators
{
    /// <summary>
    /// Creates containers for tree items and maintains a list of created containers.
    /// </summary>
    /// <typeparam name="T">The type of the container.</typeparam>
    public class TreeItemContainerGenerator<T> : ItemContainerGenerator<T>, ITreeItemContainerGenerator
        where T : class, IControl, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeItemContainerGenerator{T}"/> class.
        /// </summary>
        /// <param name="owner">The owner control.</param>
        /// <param name="contentProperty">The container's Content property.</param>
        /// <param name="itemsProperty">The container's Items property.</param>
        /// <param name="isExpandedProperty">The container's IsExpanded property.</param>
        /// <param name="index">The container index for the tree</param>
        public TreeItemContainerGenerator(
            IControl owner,
            PerspexProperty contentProperty,
            PerspexProperty itemsProperty,
            PerspexProperty isExpandedProperty,
            TreeContainerIndex index)
            : base(owner, contentProperty)
        {
            Contract.Requires<ArgumentNullException>(owner != null);
            Contract.Requires<ArgumentNullException>(contentProperty != null);
            Contract.Requires<ArgumentNullException>(itemsProperty != null);
            Contract.Requires<ArgumentNullException>(isExpandedProperty != null);
            Contract.Requires<ArgumentNullException>(index != null);

            ItemsProperty = itemsProperty;
            IsExpandedProperty = isExpandedProperty;
            Index = index;
        }

        /// <summary>
        /// Gets the container index for the tree.
        /// </summary>
        public TreeContainerIndex Index { get; }

        /// <summary>
        /// Gets the item container's Items property.
        /// </summary>
        protected PerspexProperty ItemsProperty { get; }

        /// <summary>
        /// Gets the item container's IsExpanded property.
        /// </summary>
        protected PerspexProperty IsExpandedProperty { get; }

        /// <inheritdoc/>
        protected override IControl CreateContainer(object item)
        {
            var container = item as T;

            if (item == null)
            {
                return null;
            }
            else if (container != null)
            {
                Index.Add(item, container);
                return container;
            }
            else
            {
                var template = GetTreeDataTemplate(item);
                var result = new T();

                result.SetValue(ContentProperty, template.Build(item));
                result.SetValue(ItemsProperty, template.ItemsSelector(item));

                if (!(item is IControl))
                {
                    result.DataContext = item;
                }

                Index.Add(item, result);

                return result;
            }
        }

        public override IEnumerable<ItemContainer> Clear()
        {
            var items = base.Clear();
            Index.Remove(items);
            return items;
        }

        public override IEnumerable<ItemContainer> Dematerialize(int startingIndex, int count)
        {
            Index.Remove(GetContainerRange(startingIndex, count));
            return base.Dematerialize(startingIndex, count);
        }

        public override IEnumerable<ItemContainer> RemoveRange(int startingIndex, int count)
        {
            Index.Remove(GetContainerRange(startingIndex, count));
            return base.RemoveRange(startingIndex, count);
        }

        /// <summary>
        /// Gets the data template for the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The template.</returns>
        private ITreeDataTemplate GetTreeDataTemplate(object item)
        {
            var template = Owner.FindDataTemplate(item) ?? FuncDataTemplate.Default;
            var treeTemplate = template as ITreeDataTemplate ??
                new FuncTreeDataTemplate(typeof(object), template.Build, x => null);
            return treeTemplate;
        }
    }
}
