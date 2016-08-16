using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Windows.Forms;

namespace GBH
{
    public static class Renderer
    {
        public static GraphicsDevice Device { get; set; }
        public static ContentManager Content { get; set; }

        public static void Initialize()
        {
            var pp = new PresentationParameters();
            pp.DeviceWindowHandle = GameWindow.NativeHandle;

            pp.BackBufferFormat = SurfaceFormat.Color;
            pp.BackBufferWidth = GameWindow.ViewportSize.Width;
            pp.BackBufferHeight = GameWindow.ViewportSize.Height;
            pp.RenderTargetUsage = RenderTargetUsage.DiscardContents;
            pp.IsFullScreen = false;

            pp.DepthStencilFormat = DepthFormat.Depth16;

            pp.PresentationInterval = PresentInterval.Immediate;

            Device = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, pp);

            GameWindow.Resized += (size) =>
            {
                Device.PresentationParameters.BackBufferWidth = size.Width;
                Device.PresentationParameters.BackBufferHeight = size.Height;

                Device.Reset(Device.PresentationParameters);
            };

            //Device.Initialize();

            // create content manager
            var sp = new GBHServiceProvider();
            Content = sp.CreateContentManager("Content");
        }

        public static void Clear()
        {
            Device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Black, 1.0f, 0);
        }

        // handles device lost cases, returns false if the device can't be reset (yet)
        public static bool MakeDeviceAvailable()
        {
            if (Device.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal)
            {
                return true;
            }
            else if (Device.GraphicsDeviceStatus == GraphicsDeviceStatus.Lost)
            {
                return false;
            }
            else
            {
                //Device.Reset(Device.PresentationParameters);
                return true;
            }            
        }
    }

    // stuff for content manager, from http://stackoverflow.com/questions/6361691/custom-xna-game-loop-in-windows
    class GBHServiceProvider : IServiceProvider
    {
        private IGraphicsDeviceService gdService;

        public GBHServiceProvider()
        {
            gdService = new GBHGraphicsDeviceService(Renderer.Device);
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IGraphicsDeviceService))
            {
                return gdService;
            }

            return null;
        }

        public ContentManager CreateContentManager(string sPath)
        {
            ContentManager content = new ContentManager(this);
            content.RootDirectory = sPath;

            return content;
        }
    }

    class GBHGraphicsDeviceService : IGraphicsDeviceService
    {
        private GraphicsDevice graphicsDevice;
        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return graphicsDevice;
            }
        }

        public GBHGraphicsDeviceService(GraphicsDevice device)
        {
            graphicsDevice = device;
        }

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;
    }
}
