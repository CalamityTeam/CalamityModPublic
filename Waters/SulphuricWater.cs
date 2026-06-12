using System;
using CalamityMod.Dusts.WaterSplash;
using CalamityMod.Gores.WaterDroplet;
using CalamityMod.Particles;
using CalamityMod.Systems.Graphic.LiquidSystem;
using CalamityMod.Tiles.Abyss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class SulphuricWaterflow : ModWaterfallStyle, IWaterfallStyleModifyColor
    {
        public void ModifyColor(in Tile tile, int x, int y, ref VertexColors liquidColor) => WaterStyleCommon.ModifySulphuricWaterColor(x, y, ref liquidColor, false);
    }

    public class SulphuricWater : ModWaterStyle, IWaterStyleModifyColor, IWaterStyleModifyLight, IWaterStylePostDrawEffect
    {
        public static ModWaterStyle Instance { get; private set; }
        public static ModWaterfallStyle WaterfallStyle { get; private set; }
        public static int SplashDust { get; private set; }
        public static int DropletGore { get; private set; }
        public static Asset<Texture2D> RainTexture { get; private set; }

        public override void SetStaticDefaults()
        {
            Instance = this;
            WaterfallStyle = ModContent.Find<ModWaterfallStyle>("CalamityMod/SulphuricWaterflow");
            SplashDust = ModContent.DustType<SulphuricSplash>();
            DropletGore = ModContent.GoreType<SulphuricWaterDroplet>();
        }

        public override void Unload()
        {
            Instance = null;
            WaterfallStyle = null;
            SplashDust = 0;
            DropletGore = 0;
            RainTexture = null;
        }

        public override int ChooseWaterfallStyle() => WaterfallStyle.Slot;
        public override int GetSplashDust() => SplashDust;
        public override int GetDropletGore() => DropletGore;
        public override Asset<Texture2D> GetRainTexture() => RainTexture ??= ModContent.Request<Texture2D>("CalamityMod/Waters/SulphuricRain");

        public override byte GetRainVariant() => (byte)Main.rand.Next(3);
        public override Color BiomeHairColor() => new Color(43, 168, 110);
        public void ModifyColor(in Tile tile, int x, int y, ref VertexColors liquidColor, bool isSlope) => WaterStyleCommon.ModifySulphuricWaterColor(x, y, ref liquidColor, isSlope);
        public void ModifyLight(in Tile tile, int i, int j, ref float r, ref float g, ref float b)
        {
            Vector3 outputColor = new Vector3(r, g, b);

            if (tile.TileType != RustyChestTile.TileType)
            {
                if (Main.dayTime && !Main.raining)
                {
                    float brightness = MathHelper.Clamp(0.2f - (j / ((int)Main.worldSurface + 120)), 0f, 0.2f);
                    if (j > (int)Main.worldSurface + 20)
                        brightness *= 1f - (j - ((int)Main.worldSurface + 20)) / 100f;

                    float time = Main.GameUpdateCount;
                    float waveScale1 = time * 0.014f;
                    float waveScale2 = time * 0.1f;
                    int scalar = i + (-j / 2);
                    float wave1 = waveScale1 * -50 + scalar * 15;
                    float wave2 = waveScale2 * -10 + scalar * 14;
                    float wave3 = waveScale1 * -100 + scalar * 13;
                    float wave4 = waveScale2 * 10 + scalar * 25;
                    float wave5 = waveScale1 * -70 + scalar * 5;
                    float wave1angle = 0.55f + 0.45f * MathF.Sin(MathHelper.ToRadians(wave1));
                    float wave2angle = 0.55f + 0.45f * MathF.Sin(MathHelper.ToRadians(wave2));
                    float wave3angle = 0.55f + 0.45f * MathF.Sin(MathHelper.ToRadians(wave3));
                    float wave4angle = 0.55f + 0.45f * MathF.Sin(MathHelper.ToRadians(wave4));
                    float wave5angle = 0.55f + 0.45f * MathF.Sin(MathHelper.ToRadians(wave5));
                    outputColor = Vector3.Lerp(outputColor, Color.LightSeaGreen.ToVector3(), 0.41f + wave1angle + wave2angle + wave3angle + wave4angle + wave5angle);
                    outputColor *= brightness;
                }

                if (!Main.dayTime && !Main.raining)
                {
                    float brightness = MathHelper.Clamp(0.17f - (j / ((int)Main.worldSurface + 120)), 0f, 0.17f);
                    if (j > ((int)Main.worldSurface + 20))
                        brightness *= 1f - (j - ((int)Main.worldSurface + 20)) / 100f;

                    float waveScale1 = Main.GameUpdateCount * 0.014f;
                    float waveScale2 = Main.GameUpdateCount * 0.1f;
                    int scalar = i + (-j / 2);
                    float wave1 = waveScale1 * -50 + scalar * 15;
                    float wave2 = waveScale2 * -10 + scalar * 14;
                    float wave3 = waveScale1 * -100 + scalar * 13;
                    float wave4 = waveScale2 * 10 + scalar * 25;
                    float wave5 = waveScale1 * -70 + scalar * 5;
                    float wave1angle = 0.55f + 0.45f * (float)Math.Sin(MathHelper.ToRadians(wave1));
                    float wave2angle = 0.55f + 0.45f * (float)Math.Sin(MathHelper.ToRadians(wave2));
                    float wave3angle = 0.55f + 0.45f * (float)Math.Sin(MathHelper.ToRadians(wave3));
                    float wave4angle = 0.55f + 0.45f * (float)Math.Sin(MathHelper.ToRadians(wave4));
                    float wave5angle = 0.55f + 0.45f * (float)Math.Sin(MathHelper.ToRadians(wave5));
                    outputColor = Vector3.Lerp(outputColor, Color.LightSeaGreen.ToVector3(), 0.41f + wave1angle + wave2angle + wave3angle + wave4angle + wave5angle);
                    outputColor *= brightness;
                }

                if (Main.raining)
                {
                    float brightness = MathHelper.Clamp(1f - (j / ((int)Main.worldSurface + 120)), 0f, 1f);
                    if (j > ((int)Main.worldSurface + 20))
                        brightness *= 1f - (j - ((int)Main.worldSurface + 20)) / 100f;

                    outputColor = Vector3.Lerp(outputColor, Color.LightSeaGreen.ToVector3(), 0.41f);
                    outputColor *= brightness;
                }
            }
            r = outputColor.X;
            g = outputColor.Y;
            b = outputColor.Z;
        }

        public void PostDrawEffect(in Tile tile, int x, int y)
        {
            Tile above = CalamityUtils.ParanoidTileRetrieval(x, y - 1);
            if (!Main.gamePaused && !above.HasTile && above.LiquidAmount <= 0 && Main.rand.NextBool(9))
            {
                MediumMistParticle acidFoam = new(new(x * 16f + Main.rand.NextFloat(16f), y * 16f + 8f), -Vector2.UnitY.RotatedByRandom(0.67f) * Main.rand.NextFloat(1f, 2.4f), Color.LightSeaGreen, Color.White, 0.16f, 128f, 0.02f);
                GeneralParticleHandler.SpawnParticle(acidFoam);
            }
        }
    }
}
