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
            projectile.knockBack = 6f;
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.hostile = true;
            projectile.penetrate = -1;
            base.SetDefaults();
        }

        public override void Kill(int timeLeft)
        {
            Point p = projectile.Center.ToTileCoordinates();
            // If the sand is dying outside the world border, cancel placing sand.
            if (p.X < 0 || p.X >= Main.maxTilesX || p.Y < 0 || p.Y >= Main.maxTilesY)
                return;
            Tile t = Main.tile[p.X, p.Y];

            // If the sand hit a half brick, but was mostly going downwards (at a lower than 45 degree angle), then stack atop the half brick.
            if (t.halfBrick() && projectile.velocity.Y > 0f && Math.Abs(projectile.velocity.Y) > Math.Abs(projectile.velocity.X))
                t = Main.tile[p.X, --p.Y];

            // Under no circumstances can falling sand destroy minecart tracks.
            if (!t.active() && t.type != TileID.MinecartTrack)
            {
                WorldGen.PlaceTile(p.X, p.Y, ModContent.TileType<AstralSand>(), false, true);
                WorldGen.SquareTileFrame(p.X, p.Y);
            }
        }

        public override void AI()
        {
            if (Main.rand.NextBool(2))
            {
                int i = Dust.NewDust(projectile.position, projectile.width, projectile.height, 108, 0f, projectile.velocity.Y * 0.5f);
                Main.dust[i].velocity.X *= 0.2f;
            }
            projectile.velocity.Y += 0.2f;
            projectile.rotation += 0.1f;
            if (projectile.velocity.Y > 10f)
            {
                projectile.velocity.Y = 10f;
            }
            base.AI();
        }
    }
}
