using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MechwormLaser : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 4;
            projectile.extraUpdates = 2;
            projectile.timeLeft = 600;
            projectile.minion = true;
            projectile.minionSlots = 0;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            // Very quickly fade in.
            projectile.alpha = Utils.Clamp(projectile.alpha - 25, 0, 255);

            Lighting.AddLight(projectile.Center, Color.Cyan.ToVector3());
            if (projectile.ai[1] == 0f)
            {
                projectile.localAI[0] += 6f;
                if (projectile.localAI[0] > 30)
                    projectile.localAI[0] = 30;
            }
            else
            {
                projectile.localAI[0] -= 6f;
                if (projectile.localAI[0] <= 0f)
                {
                    projectile.Kill();
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(Color.Fuchsia, Color.Cyan, (float)Math.Sin(Main.GlobalTime * 1.9f + projectile.identity * 2.4f) * 0.5f + 0.5f);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.DrawBeam(100f, 3f, lightColor);

        public override void Kill(int timeLeft)
        {
            int dustAmt = Main.rand.Next(3, 7);
            for (int d = 0; d < dustAmt; d++)
            {
                int rainbow = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 2.1f);
                Main.dust[rainbow].velocity *= 2f;
                Main.dust[rainbow].noGravity = true;
            }
        }
    }
}
