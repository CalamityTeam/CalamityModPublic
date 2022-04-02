using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class OldDukeHeadCorpse : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Absolutely Disgusting Shark Puker");
            Main.projFrames[projectile.type] = 2;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 60;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
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
                projectile.localAI[0] = 1;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            NPC target = projectile.Center.MinionHoming(845f, player, false);
            // No sense in targeting something below this sentry.
            if (target != null)
            {
                if (target.Bottom.Y > projectile.Top.Y)
                {
                    target = null;
                }
            }
            projectile.frame = (target != null).ToInt();
            if (target != null)
            {
                projectile.ai[0] += 1f;
                if (Main.myPlayer == projectile.owner &&
                    projectile.ai[0] % 8f == 0f)
                {
                    float angle = (float)Math.Atan(Math.Abs(target.Center.X - projectile.Center.X) / 450f);
                    angle *= Math.Sign(target.Center.X - projectile.Center.X);
                    Projectile.NewProjectile(projectile.Top + Vector2.UnitY * 7f,
                        new Vector2(0f, -Main.rand.NextFloat(21f, 30.5f)).RotatedBy(angle),
                        ModContent.ProjectileType<OldDukeSharkVomit>(), projectile.damage, 5f,
                        projectile.owner);
                }
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
