using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class TrackingDiskProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/DraedonsArsenal/TrackingDisk";

        public bool ReturningToPlayer
        {
            get => projectile.ai[0] == 1f;
            set => projectile.ai[0] = value.ToInt();
        }

        public float Time
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }

        public const int LaserFireRate = 20;
        public const int MaxLaserCountPerShot = 4; // This only applies to stealth strikes.
        public const float MaxTargetSearchDistance = 480f;
        public const float ReturnAcceleration = 0.15f;
        public const float ReturnMaxSpeed = 12f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tracking Disk");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.Red.ToVector3());

            Player player = Main.player[projectile.owner];

            Time++;
            if (!ReturningToPlayer)
            {
                if (Time >= 45f)
                {
                    ReturningToPlayer = true;
                    projectile.tileCollide = false;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                float distanceFromPlayer = projectile.Distance(player.Center);
                if (distanceFromPlayer > 3000f)
                    projectile.Kill();

                // This is done instead of a Normalize or DirectionTo call because the variables needed are already present and calculating the square root again would be unnecessary.
                Vector2 idealVelocity = (player.Center - projectile.Center) / distanceFromPlayer * ReturnMaxSpeed;

                projectile.velocity.X += Math.Sign(idealVelocity.X - projectile.velocity.X) * ReturnAcceleration;
                projectile.velocity.Y += Math.Sign(idealVelocity.Y - projectile.velocity.Y) * ReturnAcceleration;

                if (Time % LaserFireRate == 0f)
                    AttemptToFireLasers((int)(projectile.damage * 0.25));

                if (Main.myPlayer == projectile.owner)
                {
                    if (projectile.Hitbox.Intersects(player.Hitbox))
                        projectile.Kill();
                }
            }

            projectile.rotation += 0.25f;
        }

        public void AttemptToFireLasers(int damage)
        {
            if (Main.myPlayer != projectile.owner)
                return;
            if (projectile.Calamity().stealthStrike)
            {
                int targetCount = 0;
                List<NPC> targets = Main.npc.Where(npc =>
                {
                    return npc.active && projectile.Distance(npc.Center) < MaxTargetSearchDistance && npc.CanBeChasedBy();
                }).ToList();
                foreach (var target in targets)
                {
                    if (targetCount >= MaxLaserCountPerShot)
                        break;
                    Projectile laser = Projectile.NewProjectileDirect(projectile.Center,
                                                                      projectile.SafeDirectionTo(target.Center) * 4f,
                                                                      ModContent.ProjectileType<TrackingDiskLaser>(),
                                                                      damage,
                                                                      projectile.knockBack,
                                                                      projectile.owner,
                                                                      1f);
                    laser.scale *= 1.6f;
                    laser.netUpdate = true;
                    targetCount++;
                }
            }
            else
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(MaxTargetSearchDistance);
                if (potentialTarget != null)
                {
                    Projectile.NewProjectile(projectile.Center, projectile.SafeDirectionTo(potentialTarget.Center) * 3f, ModContent.ProjectileType<TrackingDiskLaser>(), damage, projectile.knockBack, projectile.owner);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            ReturningToPlayer = true;
            projectile.tileCollide = false;
            projectile.netUpdate = true;
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;
            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, ProjectileID.Sets.TrailCacheLength[projectile.type]);
            return false;
        }
    }
}
