using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public abstract class CustomLavaStyle
    {
        internal Texture2D LavaTexture;
        internal Texture2D BlockTexture;
        internal Texture2D SlopeTexture;

        internal void Load()
        {
            // Don't load textures serverside.
            if (Main.netMode == NetmodeID.Server)
                return;

            LavaTexture = ModContent.Request<Texture2D>(LavaTexturePath, AssetRequestMode.ImmediateLoad).Value;
            BlockTexture = ModContent.Request<Texture2D>(BlockTexturePath, AssetRequestMode.ImmediateLoad).Value;
            SlopeTexture = ModContent.Request<Texture2D>(SlopeTexturePath, AssetRequestMode.ImmediateLoad).Value;
        }

        internal void Unload()
        {
            LavaTexture = null;
            BlockTexture = null;
        }

        public abstract string LavaTexturePath { get; }

        public abstract string BlockTexturePath { get; }

        public abstract string SlopeTexturePath { get; }

        public virtual bool ChooseLavaStyle() => false;

        public abstract int ChooseWaterfallStyle();

        public abstract int GetSplashDust();

        public abstract int GetDropletGore();

        public virtual void SelectLightColor(ref Color initialLightColor)
        {
        }
    }
}
