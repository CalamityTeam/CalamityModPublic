using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class DarkIceZero : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.coldDamage = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Ice");
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;

            return true;
        }

        public override void AI()
        {
            if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 16f)
            {
                Projectile.velocity *= 1.045f;
            }

            //make pretty dust
            int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 172, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1.25f);
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
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 192;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                Projectile.damage /= 2;
                Projectile.Damage();
                SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
                for (int i = 0; i < 30; i++)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 172, 0f, 0f, 0, default, Main.rand.NextFloat(1f, 2f));
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 4f;
                }
                for (int index1 = 0; index1 < 20; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 68, 0f, 0f, 0, new Color(), 1.3f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 1.5f;
                }
            }
        }
    }
}
