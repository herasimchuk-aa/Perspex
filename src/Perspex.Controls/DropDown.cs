﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Perspex.Controls.Generators;
using Perspex.Controls.Primitives;
using Perspex.Controls.Shapes;
using Perspex.Input;
using Perspex.Layout;
using Perspex.Media;
using Perspex.VisualTree;

namespace Perspex.Controls
{

    /// <summary>
    /// A drop-down list control.
    /// </summary>
    public class DropDown : SelectingItemsControl
    {
        /// <summary>
        /// Defines the <see cref="IsDropDownOpen"/> property.
        /// </summary>
        public static readonly DirectProperty<DropDown, bool> IsDropDownOpenProperty =
            PerspexProperty.RegisterDirect<DropDown, bool>(
                nameof(IsDropDownOpen),
                o => o.IsDropDownOpen,
                (o, v) => o.IsDropDownOpen = v);

        /// <summary>
        /// Defines the <see cref="SelectionBoxItem"/> property.
        /// </summary>
        public static readonly DirectProperty<DropDown, object> SelectionBoxItemProperty =
            PerspexProperty.RegisterDirect<DropDown, object>("SelectionBoxItem", o => o.SelectionBoxItem);

        private bool _isDropDownOpen;
        private Popup _popup;
        private object _selectionBoxItem;

        /// <summary>
        /// Initializes static members of the <see cref="DropDown"/> class.
        /// </summary>
        static DropDown()
        {
            FocusableProperty.OverrideDefaultValue<DropDown>(true);
            SelectedItemProperty.Changed.AddClassHandler<DropDown>(x => x.SelectedItemChanged);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dropdown is currently open.
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return _isDropDownOpen; }
            set { SetAndRaise(IsDropDownOpenProperty, ref _isDropDownOpen, value); }
        }

        /// <summary>
        /// Gets or sets the item to display as the control's content.
        /// </summary>
        protected object SelectionBoxItem
        {
            get { return _selectionBoxItem; }
            set { SetAndRaise(SelectionBoxItemProperty, ref _selectionBoxItem, value); }
        }

        /// <inheritdoc/>
        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return new ItemContainerGenerator<DropDownItem>(this, DropDownItem.ContentProperty);
        }

        /// <inheritdoc/>
        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnAttachedToLogicalTree(e);
            this.UpdateSelectionBoxItem(this.SelectedItem);
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!e.Handled)
            {
                if (e.Key == Key.F4 ||
                    (e.Key == Key.Down && ((e.Modifiers & InputModifiers.Alt) != 0)))
                {
                    IsDropDownOpen = !IsDropDownOpen;
                    e.Handled = true;
                }
                else if (IsDropDownOpen && (e.Key == Key.Escape || e.Key == Key.Enter))
                {
                    IsDropDownOpen = false;
                    e.Handled = true;
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (!IsDropDownOpen && ((IVisual)e.Source).GetVisualRoot() != typeof(PopupRoot))
            {
                IsDropDownOpen = true;
                e.Handled = true;
            }

            if (!e.Handled)
            {
                if (UpdateSelectionFromEventSource(e.Source))
                {
                    _popup?.Close();
                    e.Handled = true;
                }
            }

            base.OnPointerPressed(e);
        }

        /// <inheritdoc/>
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            if (_popup != null)
            {
                _popup.Opened -= PopupOpened;
            }

            _popup = e.NameScope.Get<Popup>("PART_Popup");
            _popup.Opened += PopupOpened;
        }

        private void PopupOpened(object sender, EventArgs e)
        {
            var selectedIndex = SelectedIndex;

            if (selectedIndex != -1)
            {
                var container = ItemContainerGenerator.ContainerFromIndex(selectedIndex);
                container?.Focus();
            }
        }

        private void SelectedItemChanged(PerspexPropertyChangedEventArgs e)
        {
            UpdateSelectionBoxItem(e.NewValue);
        }

        private void UpdateSelectionBoxItem(object item)
        {
            var contentControl = item as IContentControl;

            if (contentControl != null)
            {
                item = contentControl.Content;
            }

            var control = item as IControl;

            if (control != null)
            {
                control.Measure(Size.Infinity);

                SelectionBoxItem = new Rectangle
                {
                    Width = control.DesiredSize.Width,
                    Height = control.DesiredSize.Height,
                    Fill = new VisualBrush
                    {
                        Visual = control,
                        Stretch = Stretch.None,
                        AlignmentX = AlignmentX.Left,
                    }
                };
            }
            else
            {
                SelectionBoxItem = item;
            }
        }
    }
}
