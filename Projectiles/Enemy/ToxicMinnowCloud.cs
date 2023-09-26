using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace CalamityMod.Projectiles.Enemy
{
    public class ToxicMinnowCloud : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = 45;
            Projectile.height = 45;
            Projectile.hostile = true;
            Projectile.friendly = true;
            Projectile.penetrate = 7;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.alpha = 120;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.1f * Projectile.Opacity, 1f * Projectile.Opacity, 0f);

            if (Main.rand.NextBool())
                Projectile.velocity *= 0.95f;
            else if (Main.rand.NextBool())
                Projectile.velocity *= 0.9f;
            else if (Main.rand.NextBool())
                Projectile.velocity *= 0.85f;
            else
                Projectile.velocity *= 0.8f;

            Projectile.ai[0] += 1f;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.ai[0] < 560f)
            {
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            if (Projectile.ai[0] > 560f)
            {
                Projectile.damage = 0;
            }
            else if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.Kill();
            }

            Projectile.velocity *= 0.98f;

            if (Math.Abs(Projectile.velocity.X) > 0f)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 20f, targetHitbox);

        public override bool CanHitPlayer(Player target) => Projectile.ai[0] < 560f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;
            
            target.AddBuff(BuffID.Poisoned, 240);
        }

        public override bool? CanHitNPC(NPC target) => Projectile.ai[0] < 560f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 240);
        }
    }
}
