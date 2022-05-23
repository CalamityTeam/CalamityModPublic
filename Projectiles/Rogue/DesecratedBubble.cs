using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Rogue
{
    public class DesecratedBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 120;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.scale += 0.002f;
            if (Projectile.alpha <= 0)
            {
                Projectile.alpha = 0;
            }
            else if (Projectile.alpha > 50)
            {
                Projectile.alpha -= 20;
            }
            if (Projectile.timeLeft <= 100)
            {
                Projectile.ai[1] = 0f;
            }
            else
            {
                Projectile.velocity *= 0.995f;
            }
            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
            {
                if (Projectile.ai[1] == 0f)
                {
                    CalamityGlobalProjectile.HomeInOnNPC(Projectile, true, 200f, 8f, 20f);
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, (int)Projectile.position.X, (int)Projectile.position.Y);
            int num190 = Main.rand.Next(5, 9);
            for (int num191 = 0; num191 < num190; num191++)
            {
                int num192 = Dust.NewDust(Projectile.Center, 0, 0, 179, 0f, 0f, 100, default, 1.4f);
                Main.dust[num192].velocity *= 0.8f;
                Main.dust[num192].position = Vector2.Lerp(Main.dust[num192].position, Projectile.Center, 0.5f);
                Main.dust[num192].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[0] == 1f)
            {
                target.AddBuff(BuffID.Ichor, 180);
                target.AddBuff(BuffID.CursedInferno, 180);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Projectile.ai[0] == 1f)
            {
                target.AddBuff(BuffID.Ichor, 180);
                target.AddBuff(BuffID.CursedInferno, 180);
            }
        }

        // Cannot deal damage for the first several frames of existence.
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft >= 100)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player target) => Projectile.timeLeft < 100;
    }
}
