using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class FlyingOrthocera : ModProjectile
    {
        public const float SearchDistance = 850f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flying Orthocera");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 46;
            projectile.height = 42;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.sentry = true;
            projectile.timeLeft = Projectile.SentryLifeTime;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                for (int i = 0; i < 56; i++)
                {
                    float angle = MathHelper.TwoPi / 56f * i;
                    Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.scale = 1.5f;
                    dust.velocity = angle.ToRotationVector2() * 7f;
                    dust.noGravity = true;
                }
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter % 5f == 4f)
            {
                projectile.frame++;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            NPC potentialTarget = projectile.Center.MinionHoming(SearchDistance, player);
            if (potentialTarget != null)
            {
                projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(potentialTarget.Center) - MathHelper.PiOver4, 0.085f);
                projectile.spriteDirection = (projectile.rotation < MathHelper.Pi).ToDirectionInt();
                if (projectile.ai[0]++ % 30f == 29f)
                {
                    if (projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(projectile.Center, projectile.SafeDirectionTo(potentialTarget.Center, Vector2.UnitY) * 11f, ModContent.ProjectileType<FlyingOrthoceraStream>(), projectile.damage, 4f, projectile.owner);
                }
            }
            else
            {
                projectile.rotation = projectile.rotation.AngleTowards(0f, 0.15f);
            }
        }
    }
}
