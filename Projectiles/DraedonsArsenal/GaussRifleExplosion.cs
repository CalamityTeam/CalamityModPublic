using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class GaussRifleExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gauss Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 500;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 20;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            Time++;
            if (!Main.dedServ)
            {
                for (int i = 0; i < 50; i++)
                {
                    float angle = i / 30f * MathHelper.TwoPi;
                    Dust dust = Dust.NewDustPerfect(projectile.Center, 261);
                    dust.velocity = angle.ToRotationVector2();
                    dust.velocity = dust.velocity.RotatedByRandom(0.4f);
                    dust.velocity = dust.velocity.RotatedBy(Time / 60f * MathHelper.ToRadians(720f));
                    dust.velocity *= Main.rand.NextFloat(20f, 50f);
                    dust.scale = Main.rand.NextFloat(1.2f, 1.6f);
                    dust.noGravity = true;
                }
            }
        }
    }
}
