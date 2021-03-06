﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

namespace Perspex.Input.Raw
{
    public class RawTextInputEventArgs : RawInputEventArgs
    {
        public string Text { get; set; }

        public RawTextInputEventArgs(IKeyboardDevice device, uint timestamp, string text) : base(device, timestamp)
        {
            Text = text;
        }
    }
}
