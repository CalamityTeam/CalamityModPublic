using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class TextureLoadingSystem : ModSystem
    {
        // Vanilla Flying Carpet texture.
        public static Asset<Texture2D> CarpetOriginal;

        // Astral sky and background.
        public static Texture2D AstralSky;
        public static Texture2D AstralSurfaceFront;
        public static Texture2D AstralSurfaceFrontGlow;
        public static Texture2D AstralSurfaceClose;
        public static Texture2D AstralSurfaceCloseGlow;
        public static Texture2D AstralSurfaceMiddle;
        public static Texture2D AstralSurfaceMiddleGlow;

        // Astral Desert background.
        public static Texture2D AstralDesertSurfaceClose;
        public static Texture2D AstralDesertSurfaceMiddle;

        // Astral Snow background.
        public static Texture2D AstralSnowSurfaceMiddle;

        // Sulphurous Sea sky and background.
        public static Texture2D SulphurSeaSky;
        public static Texture2D SulphurSeaSkyFront;
        public static Texture2D SulphurSeaSurface;

        // Destroyer glowmasks.
        public static Asset<Texture2D>[] DestroyerGlowmasks = new Asset<Texture2D>[3];

        // Wall of Flesh glowmasks.
        public static Asset<Texture2D> WallOfFleshEyeGlowmask;
        public static Asset<Texture2D> WallOfFleshDemonSickleTexture;

        // Master Revengeance+ Skeletron Prime textures.
        public static Asset<Texture2D> ChadPrime;
        public static Asset<Texture2D> ChadPrimeEyeGlowmask;

        // Load all of the textures presented in this system.
        public override void OnModLoad()
        {
            CarpetOriginal = TextureAssets.FlyingCarpet;

            if (!Main.dedServ)
            {
                AstralSky = ModContent.Request<Texture2D>("CalamityMod/Skies/AstralSky", AssetRequestMode.ImmediateLoad).Value;
                AstralSurfaceFront = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSurfaceFront", AssetRequestMode.ImmediateLoad).Value;
                AstralSurfaceFrontGlow = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSurfaceFrontGlow", AssetRequestMode.ImmediateLoad).Value;
                AstralSurfaceClose = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSurfaceClose", AssetRequestMode.ImmediateLoad).Value;
                AstralSurfaceCloseGlow = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSurfaceCloseGlow", AssetRequestMode.ImmediateLoad).Value;
                AstralSurfaceMiddle = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSurfaceMiddle", AssetRequestMode.ImmediateLoad).Value;
                AstralSurfaceMiddleGlow = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSurfaceMiddleGlow", AssetRequestMode.ImmediateLoad).Value;

                AstralDesertSurfaceClose = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralDesertSurfaceClose", AssetRequestMode.ImmediateLoad).Value;
                AstralDesertSurfaceMiddle = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralDesertSurfaceMiddle", AssetRequestMode.ImmediateLoad).Value;

                AstralSnowSurfaceMiddle = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSnowSurfaceMiddle", AssetRequestMode.ImmediateLoad).Value;

                SulphurSeaSky = ModContent.Request<Texture2D>("CalamityMod/Skies/SulphurSeaSky", AssetRequestMode.ImmediateLoad).Value;
                SulphurSeaSkyFront = ModContent.Request<Texture2D>("CalamityMod/Skies/SulphurSeaSkyFront", AssetRequestMode.ImmediateLoad).Value;
                SulphurSeaSurface = ModContent.Request<Texture2D>("CalamityMod/Skies/SulphurSeaSurface", AssetRequestMode.ImmediateLoad).Value;

                DestroyerGlowmasks[0] = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBossGlowmasks/DestroyerHeadGlow", AssetRequestMode.AsyncLoad);
                DestroyerGlowmasks[1] = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBossGlowmasks/DestroyerBodyGlow", AssetRequestMode.AsyncLoad);
                DestroyerGlowmasks[2] = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBossGlowmasks/DestroyerTailGlow", AssetRequestMode.AsyncLoad);

                WallOfFleshEyeGlowmask = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBossGlowmasks/WallOfFleshEyeTelegraphGlow", AssetRequestMode.AsyncLoad);
                WallOfFleshDemonSickleTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/ForbiddenOathbladeProjectile", AssetRequestMode.AsyncLoad);

                ChadPrime = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ChadPrime", AssetRequestMode.AsyncLoad);
                ChadPrimeEyeGlowmask = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ChadPrimeHeadGlow", AssetRequestMode.AsyncLoad);
            }
        }

        // When unloading the mod, all of the texture values preferably should be set to null.
        public override void OnModUnload()
        {
            AstralSky = null;
            AstralSurfaceFront = null;
            AstralSurfaceFrontGlow = null;
            AstralSurfaceClose = null;
            AstralSurfaceCloseGlow = null;
            AstralSurfaceMiddle = null;
            AstralSurfaceMiddleGlow = null;

            AstralDesertSurfaceClose = null;
            AstralDesertSurfaceMiddle = null;

            AstralSnowSurfaceMiddle = null;

            SulphurSeaSky = null;
            SulphurSeaSkyFront = null;
            SulphurSeaSurface = null;

            for (int i = 0; i < 3; i++)
                DestroyerGlowmasks[i] = null;

            WallOfFleshEyeGlowmask = null;
            WallOfFleshDemonSickleTexture = null;

            ChadPrime = null;
            ChadPrimeEyeGlowmask = null;

            CarpetOriginal = null;
            if (!Main.dedServ)
                TextureAssets.FlyingCarpet = CarpetOriginal;
        }
    }
}
