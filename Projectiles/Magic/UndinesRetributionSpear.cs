using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class UndinesRetributionSpear : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 420;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            float velocityMult = 25f * Projectile.ai[1]; //100
            float velocityScale = 5f * Projectile.ai[1]; //5
            float homingRange = 500f;
            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2((double)-(double)Projectile.velocity.Y, (double)-(double)Projectile.velocity.X);
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X);
            }
            Lighting.AddLight(Projectile.Center, 0f, 0.1f, 0.7f);
            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
            {
                if (Projectile.Distance(Main.player[Projectile.owner].Center) > homingRange)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * (velocityMult - 1f) + moveDirection * velocityScale) / velocityMult;
                    return;
                }

                CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 9f, 20f);
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
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
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
            Projectile.damage /= 2;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
            }
            for (int j = 0; j < 6; j++)
            {
                int undust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 186, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                Main.dust[undust].noGravity = true;
                Main.dust[undust].velocity *= 3f;
                undust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 186, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                Main.dust[undust].velocity *= 2f;
                Main.dust[undust].noGravity = true;
            }
        }
    }
}
