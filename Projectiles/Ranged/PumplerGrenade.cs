using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Dusts;

namespace CalamityMod.Projectiles.Ranged
{
    public class PumplerGrenade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squash Shell");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.timeLeft = 180;
            projectile.penetrate = 1;
            projectile.ranged = true;
            projectile.ignoreWater = true;
			projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;

        }
        public override string Texture => "CalamityMod/Projectiles/Ranged/PumplerGrenade";

        private void Explode()
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            if (Main.myPlayer == projectile.owner)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(15f, 15f), ModContent.DustType<PumplerDust>());
                    dust.velocity = (dust.position - projectile.Center) * 0.2f + projectile.velocity;
                    dust.scale = Main.rand.NextFloat(0.8f, 1.6f);
                    dust.alpha = Main.rand.Next(30) + 100;
                    dust.rotation = Main.rand.NextFloat(6.28f);
                }
                //CalamityUtils.ExplosionGores(projectile.Center, 1);
            }
            projectile.Kill();
        }
        public override void AI()
        {
            if (projectile.timeLeft == 1)
                Explode();

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Point tileCoords = projectile.Bottom.ToTileCoordinates();
            if (Main.tile[tileCoords.X, tileCoords.Y + 1].nactive() &&
                WorldGen.SolidTile(Main.tile[tileCoords.X, tileCoords.Y + 1]) && projectile.timeLeft < 165)
            {
                Explode();
                projectile.Kill();
            }
            else
            {
                projectile.velocity.Y += 0.4f;
                if (projectile.velocity.Y > 16f)
                    projectile.velocity.Y = 16f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity *= -1f;
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            Explode();
        }
    }
}
