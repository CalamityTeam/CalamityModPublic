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
    public class GhastlySoulSmall : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
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
            int ghostlyDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 173, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[ghostlyDust];
            dust.velocity *= 0.1f;
            Main.dust[ghostlyDust].scale = 1.3f;
            Main.dust[ghostlyDust].noGravity = true;
            float projVelocityFactor = 40f * Projectile.ai[1]; //100
            float scaleFactor = 8f * Projectile.ai[1]; //5
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) - 1.57f;
            Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0.9f);
            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
            {
                if (Projectile.Distance(Main.player[Projectile.owner].Center) > 900f)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * (projVelocityFactor - 1f) + moveDirection * scaleFactor) / projVelocityFactor;
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 60);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoulEdge.ProjectileDeathSound, Projectile.Center);
            int dustAmt = 36;
            for (int i = 0; i < dustAmt; i++)
            {
                Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                rotate = rotate.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 faceDirection = rotate - Projectile.Center;
                int killedDust = Dust.NewDust(rotate + faceDirection, 0, 0, 173, faceDirection.X * 1.5f, faceDirection.Y * 1.5f, 100, default, 2f);
                Main.dust[killedDust].noGravity = true;
                Main.dust[killedDust].noLight = true;
                Main.dust[killedDust].velocity = faceDirection;
            }
        }
    }
}
