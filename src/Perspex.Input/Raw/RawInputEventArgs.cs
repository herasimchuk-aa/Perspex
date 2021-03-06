﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;

namespace Perspex.Input.Raw
{
    public class RawInputEventArgs : EventArgs
    {
        public RawInputEventArgs(IInputDevice device, uint timestamp)
        {
            Contract.Requires<ArgumentNullException>(device != null);

            Device = device;
            Timestamp = timestamp;
        }

        public IInputDevice Device { get; private set; }
        public bool Handled { get; set; }
        public uint Timestamp { get; private set; }
    }
}
