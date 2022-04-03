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
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value.ToInt();
        }

        public float Time
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public const int LaserFireRate = 20;
        public const int MaxLaserCountPerShot = 4; // This only applies to stealth strikes.
        public const float MaxTargetSearchDistance = 480f;
        public const float ReturnAcceleration = 0.15f;
        public const float ReturnMaxSpeed = 12f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tracking Disk");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3());

            Player player = Main.player[Projectile.owner];

            Time++;
            if (!ReturningToPlayer)
            {
                if (Time >= 45f)
                {
                    ReturningToPlayer = true;
                    Projectile.tileCollide = false;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                float distanceFromPlayer = Projectile.Distance(player.Center);
                if (distanceFromPlayer > 3000f)
                    Projectile.Kill();

                // This is done instead of a Normalize or DirectionTo call because the variables needed are already present and calculating the square root again would be unnecessary.
                Vector2 idealVelocity = (player.Center - Projectile.Center) / distanceFromPlayer * ReturnMaxSpeed;

                Projectile.velocity.X += Math.Sign(idealVelocity.X - Projectile.velocity.X) * ReturnAcceleration;
                Projectile.velocity.Y += Math.Sign(idealVelocity.Y - Projectile.velocity.Y) * ReturnAcceleration;

                if (Time % LaserFireRate == 0f)
                    AttemptToFireLasers((int)(Projectile.damage * 0.25));

                if (Main.myPlayer == Projectile.owner)
                {
                    if (Projectile.Hitbox.Intersects(player.Hitbox))
                        Projectile.Kill();
                }
            }

            Projectile.rotation += 0.25f;
        }

        public void AttemptToFireLasers(int damage)
        {
            if (Main.myPlayer != Projectile.owner)
                return;
            if (Projectile.Calamity().stealthStrike)
            {
                int targetCount = 0;
                List<NPC> targets = Main.npc.Where(npc =>
                {
                    return npc.active && Projectile.Distance(npc.Center) < MaxTargetSearchDistance && npc.CanBeChasedBy();
                }).ToList();
                foreach (var target in targets)
                {
                    if (targetCount >= MaxLaserCountPerShot)
                        break;
                    Projectile laser = Projectile.NewProjectileDirect(Projectile.Center,
                                                                      Projectile.SafeDirectionTo(target.Center) * 4f,
                                                                      ModContent.ProjectileType<TrackingDiskLaser>(),
                                                                      damage,
                                                                      Projectile.knockBack,
                                                                      Projectile.owner,
                                                                      1f);
                    laser.scale *= 1.6f;
                    laser.netUpdate = true;
                    targetCount++;
                }
            }
            else
            {
                NPC potentialTarget = Projectile.Center.ClosestNPCAt(MaxTargetSearchDistance);
                if (potentialTarget != null)
                {
                    Projectile.NewProjectile(Projectile.Center, Projectile.SafeDirectionTo(potentialTarget.Center) * 3f, ModContent.ProjectileType<TrackingDiskLaser>(), damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            ReturningToPlayer = true;
            Projectile.tileCollide = false;
            Projectile.netUpdate = true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, ProjectileID.Sets.TrailCacheLength[Projectile.type]);
            return false;
        }
    }
}
