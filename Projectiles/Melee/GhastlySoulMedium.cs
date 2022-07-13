using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class GhastlySoulMedium : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghastly Soul");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.scale = 1.1f;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            int num822 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 173, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[num822];
            dust.velocity *= 0.1f;
            Main.dust[num822].scale = 1.3f;
            Main.dust[num822].noGravity = true;
            float num953 = 35f * Projectile.ai[1]; //100
            float scaleFactor12 = 7f * Projectile.ai[1]; //5
            float num954 = 900f;
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) - 1.57f;
            Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0.9f);
            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
            {
                if (Projectile.Distance(Main.player[Projectile.owner].Center) > num954)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * (num953 - 1f) + moveDirection * scaleFactor12) / num953;
                    return;
                }

                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 300f, 12f, 20f);
            }
            else
            {
                if (Projectile.timeLeft > 30)
                {
                    Projectile.timeLeft = 30;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
        }

        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 96;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoulEdge.ProjectileDeathSound, Projectile.Center);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 173, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 2f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
        }
    }
}
