using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
	public class BloodBreath : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.penetrate = 2;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 40;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 9;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, projectile.Opacity * 0.77f, projectile.Opacity * 0.15f, projectile.Opacity * 0.08f);
            Time++;
            if (Time > 7f)
            {
                float dustScale = 1f;
                switch ((int)Time)
                {
                    case 8:
                        dustScale = 0.25f;
                        break;
                    case 9:
                        dustScale = 0.5f;
                        break;
                    case 10:
                        dustScale = 0.75f;
                        break;
                }
                Time++;
                if (Main.rand.NextBool(2))
                {
                    Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 5, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100);
                    if (Main.rand.NextBool(3))
                    {
                        dust.noGravity = true;
                        dust.scale *= 3f;
                        dust.velocity *= 2f;
                    }
                    else
                    {
                        dust.scale *= 1.5f;
                    }
                    dust.velocity *= 1.2f;
                    dust.scale *= dustScale;
                }
            }
            projectile.rotation += 0.3f * projectile.direction;
        }
    }
}
