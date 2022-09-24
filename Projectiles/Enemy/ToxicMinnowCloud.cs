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
            Projectile.alpha = 255;
            Projectile.penetrate = 7;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
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
                if (Projectile.alpha < 255)
                {
                    Projectile.alpha += 5;
                    if (Projectile.alpha > 255)
                        Projectile.alpha = 255;
                }
                else if (Projectile.owner == Main.myPlayer)
                    Projectile.Kill();
            }
            else if (Projectile.alpha > 80)
            {
                Projectile.alpha -= 30;
                if (Projectile.alpha < 80)
                    Projectile.alpha = 80;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 20f, targetHitbox);

        public override bool CanHitPlayer(Player target) => Projectile.alpha == 80;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (damage <= 0)
                return;

            if (Projectile.alpha == 80)
                target.AddBuff(BuffID.Poisoned, 240);
        }

        public override bool? CanHitNPC(NPC target) => Projectile.alpha == 80;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.alpha == 80)
                target.AddBuff(BuffID.Poisoned, 240);
        }
    }
}
