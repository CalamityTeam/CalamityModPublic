using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Summon
{
    public class EXPLODINGFROG : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public const float MinExplodeDistance = 120f;
        public const float ExplodeWaitTime = 45f;
        public const float ExplosionAngleVariance = 0.8f;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 26;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (Projectile.frameCounter++ > 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            Player player = Main.player[Projectile.owner];
            bool canExplode = Projectile.Center.ClosestNPCAt(MinExplodeDistance) != null;
            if (player.HasMinionAttackTargetNPC)
            {
                canExplode = Main.npc[player.MinionAttackTargetNPC].Distance(Projectile.Center) < MinExplodeDistance;
            }
            if (canExplode)
                Projectile.ai[0]++;
            if (Projectile.ai[0] >= ExplodeWaitTime)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    // Goop projectiles
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                            new Vector2(0f, -Main.rand.NextFloat(6f, 10f)).RotatedByRandom(ExplosionAngleVariance),
                            ModContent.ProjectileType<FrogGore1>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                            new Vector2(0f, -Main.rand.NextFloat(6f, 10f)).RotatedByRandom(ExplosionAngleVariance),
                            ModContent.ProjectileType<FrogGore2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                        new Vector2(0f, -Main.rand.NextFloat(6f, 10f)).RotatedByRandom(ExplosionAngleVariance),
                        ModContent.ProjectileType<FrogGore3>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                        new Vector2(0f, -Main.rand.NextFloat(6f, 10f)).RotatedByRandom(ExplosionAngleVariance),
                        ModContent.ProjectileType<FrogGore4>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                        new Vector2(0f, -Main.rand.NextFloat(6f, 10f)).RotatedByRandom(ExplosionAngleVariance),
                        ModContent.ProjectileType<FrogGore5>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                // WoF vomit sound.
                SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
                Projectile.Kill();
            }
            Projectile.velocity.Y += 0.5f;

            if (Projectile.velocity.Y > 10f)
            {
                Projectile.velocity.Y = 10f;
            }

            Projectile.StickToTiles(false, false);
        }

        public override bool? CanDamage() => false;

        // Don't die on tile collision
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
