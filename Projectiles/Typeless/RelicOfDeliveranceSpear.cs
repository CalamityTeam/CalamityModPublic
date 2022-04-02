using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
	public class RelicOfDeliveranceSpear : ModProjectile
    {
        public Vector2 IdealVelocity = -Vector2.UnitY * MinChargeSpeed;
        public const int MaxCharges = 3;
        public const float DustSpawnInterval = 3f;
        public const float WaitTimeRequiredForCharge = 40f;
        public const float WaitTimeRequiredForMaximumCharge = 180f;
        public const float MinChargeSpeed = 16f;
        public const float MaxChargeSpeed = 30f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Relic of Deliverance");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 68;
            projectile.height = 32;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
            writer.WriteVector2(IdealVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
            IdealVelocity = reader.ReadVector2();
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.dead || 
                projectile.Center.Y < 200f ||
                projectile.Center.X < 200f ||
                projectile.Center.X > Main.maxTilesX * 16f - 200f)
            {
                projectile.Kill();
                return;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            projectile.ai[0]++;
            if (projectile.velocity == Vector2.Zero)
            {
                // Immediately die if the player is not holding the spear
                if (player.ActiveItem() == null)
                {
                    projectile.Kill();
                    return;
                }
                if (player.ActiveItem().type != ModContent.ItemType<RelicOfDeliverance>())
                {
                    projectile.Kill();
                    return;
                }
                projectile.rotation = projectile.AngleTo(Main.MouseWorld);
                projectile.Center = player.Center;
                if (projectile.ai[0] >= WaitTimeRequiredForCharge)
                {
                    // Begin the lunge
                    bool noObstaclesInWay = Collision.CanHitLine(projectile.Center, 1, 1, projectile.Center + projectile.SafeDirectionTo(Main.MouseWorld) * 25f, 1, 1);
                    if (Main.myPlayer == projectile.owner && Main.mouseLeft && !Main.blockMouse && !player.mouseInterface && noObstaclesInWay)
                    {
                        float chargeSpeed = MathHelper.Lerp(MinChargeSpeed, MaxChargeSpeed, (projectile.ai[0] - WaitTimeRequiredForCharge) / WaitTimeRequiredForMaximumCharge);
                        if (chargeSpeed > MaxChargeSpeed)
                            chargeSpeed = MaxChargeSpeed;
                        projectile.velocity = projectile.SafeDirectionTo(Main.MouseWorld) * chargeSpeed;
                        IdealVelocity = projectile.velocity;
                        projectile.ai[1] = chargeSpeed;
                        Main.PlaySound(SoundID.Item73, projectile.Center);
                        projectile.netUpdate = true;
                    }
                    // Charge-up dust
                    if (projectile.ai[0] <= WaitTimeRequiredForCharge + WaitTimeRequiredForMaximumCharge)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 dustPosition = Utils.NextVector2Circular(Main.rand, projectile.width * 1.6f, projectile.height * 2.3f) / 2f + projectile.Center;
                            Dust dust = Dust.NewDustPerfect(dustPosition, (int)CalamityDusts.ProfanedFire);
                            dust.velocity = projectile.SafeDirectionTo(dust.position) * projectile.Distance(dust.position) / -11f;
                            dust.velocity += player.velocity;
                            dust.noGravity = true;
                            dust.fadeIn = 1.6f;
                        }
                        projectile.Center += Utils.NextVector2Circular(Main.rand, 1.8f, 1.8f);
                    }
                    // Full charge dust indicator
                    if (projectile.ai[0] == WaitTimeRequiredForCharge + WaitTimeRequiredForMaximumCharge)
                    {
                        for (int i = 0; i < 60; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.ProfanedFire);
                            dust.velocity = Utils.NextVector2Circular(Main.rand, 9f, 9f) * Main.rand.NextFloat(0.8f, 1.2f);
                            dust.velocity += player.velocity;
                            dust.noGravity = true;
                            dust.fadeIn = 1.6f;
                        }
                        Main.PlaySound(SoundID.DD2_DarkMageHealImpact, projectile.Center);
                    }
                    player.direction = (Math.Cos(projectile.rotation) > 0).ToDirectionInt();
                }
            }
            else
            {
                // Adjust direction so that the spear is always launching in the direction of the mouse.
                // However, if the mouse position is really close to the projectile, don't do anything.
                // Being close to the target may cause erratic movements.
                // The projectile.localAI[1] part is to prevent the projectile from redirecting on a frame-by-frame basis (Main.mouseLeft seems to mean mouse hold, not release).
                if (projectile.Distance(Main.MouseWorld) > 60f &&
                    Main.mouseLeft && !Main.blockMouse && !player.mouseInterface &&
                    projectile.localAI[0] <= MaxCharges &&
                    projectile.localAI[1] <= 0f)
                {
                    IdealVelocity = projectile.SafeDirectionTo(Main.MouseWorld) * projectile.ai[1];
                    projectile.localAI[0]++;
                    projectile.localAI[1] = 40f;
                    Main.PlaySound(SoundID.Item73, projectile.Center);
                    projectile.netUpdate = true;
                }
                else if (projectile.localAI[0] > MaxCharges)
                {
                    if (projectile.timeLeft > 120)
                    {
                        projectile.timeLeft = 120;
                    }
                    projectile.alpha = (int)MathHelper.Lerp(0, 255, 1f - projectile.timeLeft / 120f);
                }
                if (projectile.velocity != IdealVelocity)
                {
                    projectile.velocity = Vector2.Lerp(projectile.velocity, IdealVelocity, 0.125f);
                }

                player.velocity = projectile.velocity;
                // Count down the charge cooldown
                if (projectile.localAI[1] > 0f)
                    projectile.localAI[1]--;
                projectile.rotation = projectile.velocity.ToRotation();
                // Player manipulations (setting the center point and disallowing mounts).
                player.Center = projectile.Center;
                // Adjust the player's held projectile type.
                if (player.heldProj == -1)
                    player.heldProj = projectile.whoAmI;
                if (player.mount != null)
                {
                    player.mount.Dismount(player);
                }
                player.direction = (projectile.velocity.X > 0).ToDirectionInt();
                // Generate dust
                if (projectile.ai[0] % DustSpawnInterval == DustSpawnInterval - 1f)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center + (projectile.Size / 2f).RotatedBy(projectile.rotation), (int)CalamityDusts.ProfanedFire);
                        dust.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(20f * (i % 2 == 0).ToDirectionInt()));
                        dust.noGravity = true;
                        dust.fadeIn = 1.8f;
                    }
                }
            }
            // Don't die when colliding with tiles unless the spear is lunging (and after a bit of time has passed).
            projectile.tileCollide = projectile.velocity != Vector2.Zero && projectile.ai[0] >= WaitTimeRequiredForCharge + 10f;

            // Die immediately if the owner of this projectile is clipping into tiles because of its movement.
            if (Collision.SolidCollision(player.position, player.width, player.height) && projectile.velocity != Vector2.Zero)
            {
                player.velocity.Y = 0f;
                DoDeathDust(projectile.oldVelocity);

                projectile.Kill();
            }

            Lighting.AddLight(projectile.Center, Color.LightGoldenrodYellow.ToVector3());
        }
        public void DoDeathDust(Vector2 oldVelocity)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 60; i++)
            {
                Vector2 spawnOffset = (projectile.Size / 2f).RotatedBy(projectile.rotation);
                spawnOffset += Utils.NextVector2Circular(Main.rand, 10f, 10f);
                Dust dust = Dust.NewDustPerfect(projectile.Center + spawnOffset, (int)CalamityDusts.ProfanedFire);
                dust.velocity = Utils.NextVector2Circular(Main.rand, 5f, 5f) + oldVelocity;
                dust.noGravity = true;
                dust.fadeIn = 1.9f;
            }
            Main.PlaySound(SoundID.Item14, projectile.Center);
        }


        public override bool CanDamage() => projectile.velocity != Vector2.Zero;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
        // Force the spear to have "priority" when drawing so that it draws over the player.
        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, 
                                                   List<int> drawCacheProjsBehindNPCs,
                                                   List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI) => drawCacheProjsOverWiresUI.Add(index);

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            DoDeathDust(oldVelocity);
            return base.OnTileCollide(oldVelocity);
        }
    }
}
