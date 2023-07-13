using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.SunkenSea;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class AstralSandBallFalling : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallAstral";
        public override void SetDefaults() => Projectile.FallingSandSetup(false);
        public override void AI() => Projectile.FallingSandAI(108, false);
        public override void Kill(int timeLeft) => Projectile.SpawnSand(ModContent.TileType<AstralSand>(), ModContent.ItemType<Items.Placeables.AstralSand>());
    }

    public class AstralSandBallGun : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallAstral";
        public override void SetDefaults() => Projectile.FallingSandSetup();
        public override void AI() => Projectile.FallingSandAI(108);
        public override void Kill(int timeLeft) => Projectile.SpawnSand(ModContent.TileType<AstralSand>(), ModContent.ItemType<Items.Placeables.AstralSand>());
    }

    public class EutrophicSandBallGun : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallEutrophic";
        public override void SetDefaults() => Projectile.FallingSandSetup();
        public override void AI() => Projectile.FallingSandAI(108); // Weirdly same dusts as Astral
        public override void Kill(int timeLeft) => Projectile.SpawnSand(ModContent.TileType<EutrophicSand>(), ModContent.ItemType<Items.Placeables.EutrophicSand>());
    }

    public class SulphurousSandBallGun : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallSulphurous";
        public override void SetDefaults() => Projectile.FallingSandSetup();
        public override void AI() => Projectile.FallingSandAI(32);
        public override void Kill(int timeLeft) => Projectile.SpawnSand(ModContent.TileType<SulphurousSand>(), ModContent.ItemType<Items.Placeables.SulphurousSand>());
    }

    // All the setups go here to prevent mass blocks of copypasting
    public static partial class FallingSandUtils
    {
        internal static void FallingSandSetup(this Projectile proj, bool fired = true)
        {
            proj.knockBack = 6f;
            proj.width = proj.height = 10;
            proj.friendly = true;
            proj.penetrate = -1;

            if (fired)
            {
                proj.MaxUpdates = 2;
                proj.DamageType = DamageClass.Ranged;
            }
            else
                proj.hostile = true;
        }

        public static void FallingSandAI(this Projectile proj, int DustID, bool fired = true)
        {
            if (Main.rand.NextBool(2))
            {
                int i = Dust.NewDust(proj.position, proj.width, proj.height, DustID);
                Main.dust[i].velocity.X *= 0.4f;
                Main.dust[i].velocity.Y += fired ? 0f : proj.velocity.Y * 0.5f;
            }

            proj.ai[1]++;
            proj.rotation += 0.1f;
            if (proj.ai[1] >= 60f || !fired)
            {
                proj.ai[1] = 60f;
                proj.velocity.Y += 0.2f;
            }
            if (proj.velocity.Y > 10f)
                proj.velocity.Y = 10f;

            Point p = proj.Center.ToTileCoordinates();
            // Don't check out of bounds
            if (p.X < 0 || p.X >= Main.maxTilesX || p.Y < 0 || p.Y >= Main.maxTilesY)
                return;
            Tile placer = Main.tile[p.X, p.Y + 1];
            if (placer.HasTile && TileID.Sets.Platforms[placer.TileType] && proj.ai[1] >= 60f)
                proj.Kill();
        }

        public static void SpawnSand(this Projectile proj, int SandBlockID, int SandItemID)
        {
            Point p = proj.Center.ToTileCoordinates();
            // If the sand is dying outside the world border, cancel placing sand.
            if (p.X < 0 || p.X >= Main.maxTilesX || p.Y < 0 || p.Y >= Main.maxTilesY)
                return;
            Tile placer = Main.tile[p.X, p.Y];

            // If the sand hit a half brick, but was mostly going downwards (at a lower than 45 degree angle), then stack atop the half brick.
            if (placer.IsHalfBlock && proj.velocity.Y > 0f && Math.Abs(proj.velocity.Y) > Math.Abs(proj.velocity.X))
                placer = Main.tile[p.X, --p.Y];
            
            bool ValidTileBelow = true;
            bool SlopeTileBelow = false;

            // Attempt to place sand and unslope tile below if available
            // Under no circumstances can falling sand destroy minecart tracks.
            if (!placer.HasTile && placer.TileType != TileID.MinecartTrack)
            {
                if (p.Y + 1 < Main.maxTilesY)
                {
                    Tile under = Main.tile[p.X, p.Y + 1];
                    if (under.HasTile)
                    {
                        if (under.TileType == TileID.MinecartTrack)
                            ValidTileBelow = false;
                        else if (under.IsHalfBlock || under.Slope != 0)
                            SlopeTileBelow = true;
                    }
                }

                if (ValidTileBelow)
                {
                    bool PlacedBlock = WorldGen.PlaceTile(p.X, p.Y, SandBlockID, false, true);
                    WorldGen.SquareTileFrame(p.X, p.Y);

                    if (PlacedBlock && SlopeTileBelow)
                    {
                        WorldGen.SlopeTile(p.X, p.Y + 1);
						if (Main.netMode != NetmodeID.SinglePlayer)
							NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 14, p.X, p.Y + 1);
                    }
                    if (PlacedBlock && Main.netMode != NetmodeID.SinglePlayer)
						NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 1, p.X, p.Y, SandBlockID);
                }
            }
            // Give the block back if you literally can't place it
            else
                Item.NewItem(proj.GetSource_DropAsItem(), proj.position, proj.width, proj.height, SandItemID);            
        }
    }
}
