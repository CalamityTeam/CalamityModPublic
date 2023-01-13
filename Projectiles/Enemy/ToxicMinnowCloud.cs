using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class ToxicMinnowCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxic Cloud");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 48;
            Projectile.hostile = true;
            Projectile.friendly = true;
            Projectile.Opacity = 0f;
            Projectile.penetrate = 7;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.3f * Projectile.Opacity, 1.2f * Projectile.Opacity, 0f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 9)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            if (Main.rand.NextBool(2))
                Projectile.velocity *= 0.95f;
            else if (Main.rand.NextBool(2))
                Projectile.velocity *= 0.9f;
            else if (Main.rand.NextBool(2))
                Projectile.velocity *= 0.85f;
            else
                Projectile.velocity *= 0.8f;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 560f)
            {
                if (Projectile.Opacity > 0f)
                {
                    Projectile.Opacity -= 0.02f;
                    if (Projectile.Opacity < 0f)
                        Projectile.Opacity = 0f;
                }
                else if (Projectile.owner == Main.myPlayer)
                    Projectile.Kill();
            }
            else if (Projectile.Opacity < 0.9f)
            {
                Projectile.Opacity += 0.12f;
                if (Projectile.Opacity > 0.9f)
                    Projectile.Opacity = 0.9f;
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

        public override bool CanHitPlayer(Player target) => Projectile.Opacity >= 0.9f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (damage <= 0)
                return;

            if (Projectile.Opacity >= 0.9f)
                target.AddBuff(BuffID.Poisoned, 240);
        }

        public override bool? CanHitNPC(NPC target) => Projectile.Opacity >= 0.9f;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.Opacity >= 0.9f)
                target.AddBuff(BuffID.Poisoned, 240);
        }
    }
}
