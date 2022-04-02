using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 120;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.scale += 0.002f;
            if (projectile.alpha <= 0)
            {
                projectile.alpha = 0;
            }
            else if (projectile.alpha > 50)
            {
                projectile.alpha -= 20;
            }
            if (projectile.timeLeft <= 100)
            {
                projectile.ai[1] = 0f;
            }
            else
            {
                projectile.velocity *= 0.995f;
            }
            if (Main.player[projectile.owner].active && !Main.player[projectile.owner].dead)
            {
                if (projectile.ai[1] == 0f)
                {
                    CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, 8f, 20f);
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item54, (int)projectile.position.X, (int)projectile.position.Y);
            int num190 = Main.rand.Next(5, 9);
            for (int num191 = 0; num191 < num190; num191++)
            {
                int num192 = Dust.NewDust(projectile.Center, 0, 0, 179, 0f, 0f, 100, default, 1.4f);
                Main.dust[num192].velocity *= 0.8f;
                Main.dust[num192].position = Vector2.Lerp(Main.dust[num192].position, projectile.Center, 0.5f);
                Main.dust[num192].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.ai[0] == 1f)
            {
                target.AddBuff(BuffID.Ichor, 180);
                target.AddBuff(BuffID.CursedInferno, 180);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (projectile.ai[0] == 1f)
            {
                target.AddBuff(BuffID.Ichor, 180);
                target.AddBuff(BuffID.CursedInferno, 180);
            }
        }

        // Cannot deal damage for the first several frames of existence.
        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.timeLeft >= 100)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player target) => projectile.timeLeft < 100;
    }
}
