using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class WebBallBol : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/WebBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Web Ball");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
            projectile.aiStyle = 14;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 30, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.Calamity().stealthStrike)
            {
                target.AddBuff(BuffID.Webbed, 120);
            }
            else
            {
                target.AddBuff(BuffID.Webbed, 60);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (projectile.Calamity().stealthStrike)
            {
                target.AddBuff(BuffID.Webbed, 120);
            }
            else
            {
                target.AddBuff(BuffID.Webbed, 60);
            }
        }
    }
}

