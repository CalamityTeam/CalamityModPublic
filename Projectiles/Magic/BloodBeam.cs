using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BloodBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.35f, 0f, 0f);
            if (projectile.ai[0] > 7f)
            {
                float scalar = 1f;
                if (projectile.ai[0] == 8f)
                {
                    scalar = 0.25f;
                }
                else if (projectile.ai[0] == 9f)
                {
                    scalar = 0.5f;
                }
                else if (projectile.ai[0] == 10f)
                {
                    scalar = 0.75f;
                }
                projectile.ai[0] += 1f;
                int dustType = DustID.Blood;
                int blood = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                Dust dust = Main.dust[blood];
                if (Main.rand.NextBool(3))
                {
                    dust.noGravity = true;
                    dust.scale *= 2f;
                    dust.velocity.X *= 2f;
                    dust.velocity.Y *= 2f;
                }
                dust.velocity.X *= 1.2f;
                dust.velocity.Y *= 1.2f;
                dust.scale *= scalar;
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) - MathHelper.PiOver2;
        }
    }
}
