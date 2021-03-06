﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;

namespace Perspex.Styling
{
    public class Styler : IStyler
    {
        public void ApplyStyles(IStyleable control)
        {
            var styleHost = control as IStyleHost;

            if (styleHost != null)
            {
                ApplyStyles(control, styleHost);
            }
        }

        private void ApplyStyles(IStyleable control, IStyleHost styleHost)
        {
            Contract.Requires<ArgumentNullException>(control != null);
            Contract.Requires<ArgumentNullException>(styleHost != null);

            var parentContainer = styleHost.StylingParent;

            if (parentContainer != null)
            {
                ApplyStyles(control, parentContainer);
            }

            styleHost.Styles.Attach(control, styleHost);
        }
    }
}
