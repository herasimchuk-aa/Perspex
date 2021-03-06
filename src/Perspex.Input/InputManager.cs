﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Perspex.Input.Raw;

namespace Perspex.Input
{
    public class InputManager : IInputManager
    {
        private readonly Subject<RawInputEventArgs> _rawEventReceived = new Subject<RawInputEventArgs>();

        private readonly Subject<RawInputEventArgs> _postProcess = new Subject<RawInputEventArgs>();

        public static IInputManager Instance => PerspexLocator.Current.GetService<IInputManager>();

        public IObservable<RawInputEventArgs> RawEventReceived => _rawEventReceived;

        public IObservable<RawInputEventArgs> PostProcess => _postProcess;

        public void Process(RawInputEventArgs e)
        {
            _rawEventReceived.OnNext(e);
            _postProcess.OnNext(e);
        }
    }
}
