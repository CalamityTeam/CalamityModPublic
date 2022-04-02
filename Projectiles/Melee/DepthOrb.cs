using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class DepthOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 30;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0f, 0.5f);
            int num458 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 33, 0f, 0f, 100, default, 0.6f);
            Main.dust[num458].noGravity = true;
            Main.dust[num458].velocity *= 0.5f;
            Main.dust[num458].velocity += projectile.velocity * 0.1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 25)
                return false;

            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(50, 50, 255, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 64;
            projectile.Center = projectile.position;
            for (int dustIndex = 0; dustIndex < 30; dustIndex++)
            {
                float num463 = (float)Main.rand.Next(-10, 11);
                float num464 = (float)Main.rand.Next(-10, 11);
                float num465 = (float)Main.rand.Next(3, 9);
                float num466 = (float)Math.Sqrt((double)(num463 * num463 + num464 * num464));
                num466 = num465 / num466;
                num463 *= num466;
                num464 *= num466;
                int num467 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 33, 0f, 0f, 100, default, 1.2f);
                Dust dust = Main.dust[num467];
                dust.noGravity = true;
                dust.position.X = projectile.Center.X;
                dust.position.Y = projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
                dust.velocity.X = num463;
                dust.velocity.Y = num464;
            }
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.damage /= 2;
            projectile.Damage();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
        }
    }
}
