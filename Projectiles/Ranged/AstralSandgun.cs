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
            Projectile.knockBack = 6f;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            base.SetDefaults();
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void Kill(int timeLeft)
        {
            int tileX = (int)(Projectile.Center.X / 16f);
            int tileY = (int)(Projectile.Center.Y / 16f);
            //Move the set tile upwards based on certain conditions
            if (Main.tile[tileX, tileY].IsHalfBlock && Projectile.velocity.Y > 0f && Math.Abs(Projectile.velocity.Y) > Math.Abs(Projectile.velocity.X))
            {
                tileY--;
            }
            if (!Main.tile[tileX, tileY].active())
            {
                if (Main.tile[tileX, tileY].TileType == TileID.MinecartTrack)
                    return;

                WorldGen.PlaceTile(tileX, tileY, ModContent.TileType<AstralSand>(), false, true);
                WorldGen.SquareTileFrame(tileX, tileY);
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
