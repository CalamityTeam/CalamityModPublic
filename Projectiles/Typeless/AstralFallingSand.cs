using CalamityMod.Tiles.AstralDesert;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class AstralFallingSand : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.knockBack = 6f;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            base.SetDefaults();
        }

        public override void Kill(int timeLeft)
        {
            Point p = Projectile.Center.ToTileCoordinates();
            // If the sand is dying outside the world border, cancel placing sand.
            if (p.X < 0 || p.X >= Main.maxTilesX || p.Y < 0 || p.Y >= Main.maxTilesY)
                return;
            Tile t = Main.tile[p.X, p.Y];

            // If the sand hit a half brick, but was mostly going downwards (at a lower than 45 degree angle), then stack atop the half brick.
            if (t.IsHalfBlock && Projectile.velocity.Y > 0f && Math.Abs(Projectile.velocity.Y) > Math.Abs(Projectile.velocity.X))
                t = Main.tile[p.X, --p.Y];

            // Under no circumstances can falling sand destroy minecart tracks.
            if (!t.active() && t.TileType != TileID.MinecartTrack)
            {
                WorldGen.PlaceTile(p.X, p.Y, ModContent.TileType<AstralSand>(), false, true);
                WorldGen.SquareTileFrame(p.X, p.Y);
            }
        }

        public override void AI()
        {
            if (Main.rand.NextBool(2))
            {
                int i = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 108, 0f, Projectile.velocity.Y * 0.5f);
                Main.dust[i].velocity.X *= 0.2f;
            }
            Projectile.velocity.Y += 0.2f;
            Projectile.rotation += 0.1f;
            if (Projectile.velocity.Y > 10f)
            {
                Projectile.velocity.Y = 10f;
            }
            base.AI();
        }
    }
}
