﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reflection;
using Moq;
using Perspex.Input;
using Perspex.Layout;
using Perspex.Platform;
using Perspex.Shared.PlatformSupport;
using Perspex.Styling;
using Perspex.Themes.Default;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Perspex.UnitTests
{
    public class TestServices
    {
        private static IFixture s_fixture = new Fixture().Customize(new AutoMoqCustomization());

        public static readonly TestServices StyledWindow = new TestServices(
            assetLoader: new AssetLoader(),
            layoutManager: new LayoutManager(),
            platformWrapper: new PclPlatformWrapper(),
            renderInterface: s_fixture.Create<IPlatformRenderInterface>(),
            standardCursorFactory: Mock.Of<IStandardCursorFactory>(),
            styler: new Styler(),
            theme: () => new DefaultTheme(),
            threadingInterface: Mock.Of<IPlatformThreadingInterface>(x => x.CurrentThreadIsLoopThread == true),
            windowingPlatform: new MockWindowingPlatform());

        public static readonly TestServices MockPlatformWrapper = new TestServices(
            platformWrapper: Mock.Of<IPclPlatformWrapper>());

        public static readonly TestServices MockStyler = new TestServices(
            styler: Mock.Of<IStyler>());

        public static readonly TestServices MockThreadingInterface = new TestServices(
            threadingInterface: Mock.Of<IPlatformThreadingInterface>(x => x.CurrentThreadIsLoopThread == true));

        public static readonly TestServices RealStyler = new TestServices(
            styler: new Styler());

        public TestServices(
            IAssetLoader assetLoader = null,
            IInputManager inputManager = null,
            ILayoutManager layoutManager = null,
            IPclPlatformWrapper platformWrapper = null,
            IPlatformRenderInterface renderInterface = null,
            IStandardCursorFactory standardCursorFactory = null,
            IStyler styler = null,
            Func<Styles> theme = null,
            IPlatformThreadingInterface threadingInterface = null,
            IWindowImpl windowImpl = null,
            IWindowingPlatform windowingPlatform = null)
        {
            AssetLoader = assetLoader;
            InputManager = inputManager;
            LayoutManager = layoutManager;
            PlatformWrapper = platformWrapper;
            RenderInterface = renderInterface;
            StandardCursorFactory = standardCursorFactory;
            Styler = styler;
            Theme = theme;
            ThreadingInterface = threadingInterface;
            WindowImpl = windowImpl;
            WindowingPlatform = windowingPlatform;
        }

        public IAssetLoader AssetLoader { get; }
        public IInputManager InputManager { get; }
        public ILayoutManager LayoutManager { get; }
        public IPclPlatformWrapper PlatformWrapper { get; }
        public IPlatformRenderInterface RenderInterface { get; }
        public IStandardCursorFactory StandardCursorFactory { get; }
        public IStyler Styler { get; }
        public Func<Styles> Theme { get; }
        public IPlatformThreadingInterface ThreadingInterface { get; }
        public IWindowImpl WindowImpl { get; }
        public IWindowingPlatform WindowingPlatform { get; }

        public TestServices With(
            IAssetLoader assetLoader = null,
            IInputManager inputManager = null,
            ILayoutManager layoutManager = null,
            IPclPlatformWrapper platformWrapper = null,
            IPlatformRenderInterface renderInterface = null,
            IStandardCursorFactory standardCursorFactory = null,
            IStyler styler = null,
            Func<Styles> theme = null,
            IPlatformThreadingInterface threadingInterface = null,
            IWindowImpl windowImpl = null,
            IWindowingPlatform windowingPlatform = null)
        {
            return new TestServices(
                assetLoader: assetLoader ?? AssetLoader,
                inputManager: inputManager ?? InputManager,
                layoutManager: layoutManager ?? LayoutManager,
                platformWrapper: platformWrapper ?? PlatformWrapper,
                renderInterface: renderInterface ?? RenderInterface,
                standardCursorFactory: standardCursorFactory ?? StandardCursorFactory,
                styler: styler ?? Styler,
                theme: theme ?? Theme,
                threadingInterface: threadingInterface ?? ThreadingInterface,
                windowImpl: windowImpl ?? WindowImpl,
                windowingPlatform: windowingPlatform ?? WindowingPlatform);
        }
    }
}
