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
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 30, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.Calamity().stealthStrike)
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
            if (Projectile.Calamity().stealthStrike)
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

