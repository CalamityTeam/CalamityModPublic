using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DarkIceZero : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
            projectile.timeLeft = 600;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            projectile.coldDamage = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Ice");
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 595)
                return false;

            return true;
        }

        public override void AI()
        {
            if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 16f)
            {
                projectile.velocity *= 1.045f;
            }

            //make pretty dust
            int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 172, projectile.velocity.X, projectile.velocity.Y, 0, default, 1.25f);
            Main.dust[index2].noGravity = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(198, 197, 246);
        }

        public override void Kill(int timeLeft)
        {
            if (timeLeft > 0)
            {
                projectile.position = projectile.Center;
                projectile.width = projectile.height = 192;
                projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
                projectile.maxPenetrate = -1;
                projectile.penetrate = -1;
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 10;
                projectile.damage /= 2;
                projectile.Damage();
                Main.PlaySound(SoundID.Item27, projectile.Center);
                for (int i = 0; i < 30; i++)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 172, 0f, 0f, 0, default, Main.rand.NextFloat(1f, 2f));
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 4f;
                }
                for (int index1 = 0; index1 < 20; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 68, 0f, 0f, 0, new Color(), 1.3f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 1.5f;
                }
            }
        }
    }
}
