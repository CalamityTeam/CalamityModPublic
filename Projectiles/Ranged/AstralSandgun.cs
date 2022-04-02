using CalamityMod.Tiles.AstralDesert;
using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class AstralSandgun : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.knockBack = 6f;
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            base.SetDefaults();
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void Kill(int timeLeft)
        {
            int tileX = (int)(projectile.Center.X / 16f);
            int tileY = (int)(projectile.Center.Y / 16f);
            //Move the set tile upwards based on certain conditions
            if (Main.tile[tileX, tileY].halfBrick() && projectile.velocity.Y > 0f && Math.Abs(projectile.velocity.Y) > Math.Abs(projectile.velocity.X))
            {
                tileY--;
            }
            if (!Main.tile[tileX, tileY].active())
            {
                if (Main.tile[tileX, tileY].type == TileID.MinecartTrack)
                    return;

                WorldGen.PlaceTile(tileX, tileY, ModContent.TileType<AstralSand>(), false, true);
                WorldGen.SquareTileFrame(tileX, tileY);
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
