﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Perspex.Animation;
using Perspex.Controls.Primitives;
using Perspex.Controls.Templates;
using Perspex.Controls.Utils;
using Perspex.Input;

namespace Perspex.Controls
{
    /// <summary>
    /// An items control that displays its items as pages that fill the control.
    /// </summary>
    public class Carousel : SelectingItemsControl
    {
        /// <summary>
        /// Defines the <see cref="Transition"/> property.
        /// </summary>
        public static readonly StyledProperty<IPageTransition> TransitionProperty =
            PerspexProperty.Register<Carousel, IPageTransition>("Transition");

        /// <summary>
        /// The default value of <see cref="ItemsControl.ItemsPanelProperty"/> for 
        /// <see cref="Carousel"/>.
        /// </summary>
        private static readonly ITemplate<IPanel> PanelTemplate =
            new FuncTemplate<IPanel>(() => new Panel());

        /// <summary>
        /// Initializes static members of the <see cref="Carousel"/> class.
        /// </summary>
        static Carousel()
        {
            SelectionModeProperty.OverrideDefaultValue<Carousel>(SelectionMode.AlwaysSelected);
            ItemsPanelProperty.OverrideDefaultValue<Carousel>(PanelTemplate);
        }

        /// <summary>
        /// Gets or sets the transition to use when moving between pages.
        /// </summary>
        public IPageTransition Transition
        {
            get { return GetValue(TransitionProperty); }
            set { SetValue(TransitionProperty, value); }
        }

        /// <summary>
        /// Moves to the next item in the carousel.
        /// </summary>
        public void Next()
        {
            if (SelectedIndex < Items.Count() - 1)
            {
                ++SelectedIndex;
            }
        }

        /// <summary>
        /// Moves to the previous item in the carousel.
        /// </summary>
        public void Previous()
        {
            if (SelectedIndex > 0)
            {
                --SelectedIndex;
            }
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Ignore key presses.
        }

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            // Ignore pointer presses.
        }
    }
}
