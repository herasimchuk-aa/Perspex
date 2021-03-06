﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive.Subjects;

namespace Perspex.Reactive
{
    public class AnonymousSubject<T> : AnonymousSubject<T, T>, ISubject<T>
    {
        public AnonymousSubject(IObserver<T> observer, IObservable<T> observable)
            : base(observer, observable)
        {
        }
    }
}
