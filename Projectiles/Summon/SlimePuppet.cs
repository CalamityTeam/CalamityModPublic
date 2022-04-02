using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SlimePuppet : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Puppet");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 0;
            projectile.timeLeft = 300;
            projectile.penetrate = 1;
            projectile.minion = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            ResetDamage();

            // Prevent clumping of other slime puppets near this one.
            projectile.MinionAntiClump();

            NPC potentialTarget = projectile.Center.MinionHoming(800f, Owner);

            if (potentialTarget is null)
            {
                // Movement above the player, depending on the index of the projectile in the Main.projectile array.
                float destinationAngularOffset = (float)Math.Sin(projectile.identity / 6f % 6f * MathHelper.Pi) * MathHelper.Pi / 10f;
                Vector2 destination = Owner.Center - Vector2.UnitY.RotatedBy(destinationAngularOffset) * 160f;

                if (projectile.DistanceSQ(destination) > 18f * 18f)
                    projectile.velocity = (projectile.velocity * 20f + projectile.SafeDirectionTo(destination) * 9f) / 21f;
            }
            else
            {
                float nudgedVelocityDirection = projectile.velocity.ToRotation().AngleTowards(projectile.AngleTo(potentialTarget.Center), 0.078f);
                projectile.velocity = nudgedVelocityDirection.ToRotationVector2() * MathHelper.Lerp(projectile.velocity.Length(), 16f, 0.25f);
                if (projectile.WithinRange(potentialTarget.Center, 240f))
                {
                    projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(potentialTarget.Center) * 16f, 0.1f);
                    projectile.Center = projectile.Center.MoveTowards(potentialTarget.Center, 6f);
                }
            }
            if (projectile.ai[0] > 0f)
            {
                projectile.rotation += MathHelper.ToRadians(projectile.velocity.Length() / 4f) * Math.Sign(projectile.velocity.X);
                projectile.ai[0]--;
            }
            else if (potentialTarget != null)
                projectile.rotation = projectile.velocity.ToRotation();

            if (Vector2.Dot(projectile.oldVelocity.SafeNormalize(Vector2.Zero), projectile.velocity.SafeNormalize(Vector2.Zero)) < 0.87)
                projectile.ai[0] = 50f;
        }
        
        public void ResetDamage()
        {
            // Initialize minion damage values the moment the projectile is spawned.
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = Owner.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] = 1f;
            }
            if (Owner.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue / projectile.Calamity().spawnedPlayerMinionDamageValue * Owner.MinionDamage());
                projectile.damage = trueDamage;
            }
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 100);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.NPCDeath1, projectile.Center);

            if (!Main.dedServ)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 spawnOffset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(4f, 36f);
                    Dust slime = Dust.NewDustPerfect(projectile.Center + spawnOffset, 243);
                    slime.velocity = spawnOffset.RotatedBy(MathHelper.PiOver2 * Main.rand.NextBool(2).ToDirectionInt()) * 0.16f;
                    slime.scale = 1.2f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor);
            return false;
        }
    }
}
