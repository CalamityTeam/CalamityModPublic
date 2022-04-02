using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class RadiantResolutionFire : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 10;
            projectile.friendly = true;
            projectile.light = 1f;
            projectile.timeLeft = 300;
            projectile.minion = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.ProfanedFire);
            dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi);
            dust.noGravity = true;
            NPC potentialTarget = projectile.Center.MinionHoming(1750f, player);
            if (potentialTarget != null)
                projectile.velocity = (projectile.velocity * 19f + projectile.SafeDirectionTo(potentialTarget.Center) * 20f) / 20f;
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 60);
            projectile.maxPenetrate = projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.damage /= 3;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.ProfanedFire);
                dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 6f);
                dust.noGravity = true;
            }
        }
    }
}
