using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class EXPLODINGFROG : ModProjectile
    {
        public const float MinExplodeDistance = 120f;
        public const float ExplodeWaitTime = 45f;
        public const float ExplosionAngleVariance = 0.8f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("A not frog of the explosive variety");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 44;
            projectile.height = 26;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.sentry = true;
            projectile.timeLeft = Projectile.SentryLifeTime;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (projectile.frameCounter++ > 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            Player player = Main.player[projectile.owner];
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] = 1;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            bool canExplode = projectile.Center.ClosestNPCAt(MinExplodeDistance) != null;
            if (player.HasMinionAttackTargetNPC)
            {
                canExplode = Main.npc[player.MinionAttackTargetNPC].Distance(projectile.Center) < MinExplodeDistance;
            }
            if (canExplode)
                projectile.ai[0]++;
            if (projectile.ai[0] >= ExplodeWaitTime)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    // Goop projectiles
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(projectile.Center,
                            new Vector2(0f, -Main.rand.NextFloat(6f, 10f)).RotatedByRandom(ExplosionAngleVariance),
                            ModContent.ProjectileType<FrogGore1>(), projectile.damage, projectile.knockBack, projectile.owner);
                        Projectile.NewProjectile(projectile.Center,
                            new Vector2(0f, -Main.rand.NextFloat(6f, 10f)).RotatedByRandom(ExplosionAngleVariance),
                            ModContent.ProjectileType<FrogGore2>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                    // Normally I would do something like mod.ProjectileType($"FrogGore1 + i") in a loop, but I suppose I'll let it go.
                    Projectile.NewProjectile(projectile.Center,
                        new Vector2(0f, -Main.rand.NextFloat(6f, 10f)).RotatedByRandom(ExplosionAngleVariance),
                        ModContent.ProjectileType<FrogGore3>(), projectile.damage, projectile.knockBack, projectile.owner);
                    Projectile.NewProjectile(projectile.Center,
                        new Vector2(0f, -Main.rand.NextFloat(6f, 10f)).RotatedByRandom(ExplosionAngleVariance),
                        ModContent.ProjectileType<FrogGore4>(), projectile.damage, projectile.knockBack, projectile.owner);
                    Projectile.NewProjectile(projectile.Center,
                        new Vector2(0f, -Main.rand.NextFloat(6f, 10f)).RotatedByRandom(ExplosionAngleVariance),
                        ModContent.ProjectileType<FrogGore5>(), projectile.damage, projectile.knockBack, projectile.owner);
                }
                // WoF vomit sound.
                Main.PlaySound(SoundID.NPCKilled, projectile.Center, 13);
                projectile.Kill();
            }
            projectile.velocity.Y += 0.5f;

            if (projectile.velocity.Y > 10f)
            {
                projectile.velocity.Y = 10f;
            }

            projectile.StickToTiles(false, false);
        }

        public override bool CanDamage() => false;
        // Don't die on tile collision
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
