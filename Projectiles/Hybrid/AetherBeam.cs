using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Hybrid
{
    public class AetherBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        private bool split = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 5;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 1f)
            {
                projectile.magic = false;
                projectile.ranged = true;
            }

            if (projectile.ai[1] == 1f)
            {
                split = false;
                projectile.tileCollide = false;
                projectile.ai[1] = 0f;
            }

            projectile.damage += projectile.Calamity().defDamage / 200;

            if (projectile.alpha > 0)
                projectile.alpha -= 25;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 1f, 0f, 0.7f);

            float num55 = 100f;
            float num56 = 2f;
            if (projectile.ai[1] == 0f)
            {
                projectile.localAI[0] += num56;
                if (projectile.localAI[0] > num55)
                    projectile.localAI[0] = num55;
            }
            else
            {
                projectile.localAI[0] -= num56;
                if (projectile.localAI[0] <= 0f)
                    projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 50, 200, 0);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.DrawBeam(100f, 2f, lightColor);

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (split)
            {
                float random = Main.rand.Next(30, 90);
                float spread = random * 0.0174f;
                double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 8f;
                double offsetAngle;
                int i;
                if (projectile.owner == Main.myPlayer)
                {
                    for (i = 0; i < 4; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        int proj1 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<AetherBeam>(), projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], 1f);
                        int proj2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<AetherBeam>(), projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], 1f);
                    }
                }
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 600);
        }
    }
}
