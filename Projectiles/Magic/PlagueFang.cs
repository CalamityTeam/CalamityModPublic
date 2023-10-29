using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class PlagueFang : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 50;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.alpha == 0)
            {
                int plagued = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 163, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f);
                Main.dust[plagued].noGravity = true;
                Main.dust[plagued].velocity *= 0.6f;
                Main.dust[plagued].velocity -= Projectile.velocity * 0.4f;

                int venomed = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 205, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f);
                Main.dust[venomed].noGravity = true;
                Main.dust[venomed].velocity *= 0.2f;
                Main.dust[venomed].velocity -= Projectile.velocity * 0.4f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.alpha > 0)
            {
                return Color.Transparent;
            }
            return null;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            for (int i = 0; i < 7; i++)
            {
                int killPlague = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 163, 0f, 0f, 100, default, 1f);
                Main.dust[killPlague].noGravity = true;
                Main.dust[killPlague].velocity *= 1.2f;
                Main.dust[killPlague].velocity -= Projectile.oldVelocity * 0.3f;

                int killVenom = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 205, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[killVenom];
                dust.noGravity = true;
                dust.velocity *= 1.2f;
                dust.velocity -= Projectile.oldVelocity * 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }
    }
}
