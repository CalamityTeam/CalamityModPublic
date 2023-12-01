using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Typeless;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class RelicOfDeliveranceSpear : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<RelicOfDeliverance>();
        public Vector2 IdealVelocity = -Vector2.UnitY * MinChargeSpeed;
        public const int MaxCharges = 3;
        public const float DustSpawnInterval = 3f;
        public const float WaitTimeRequiredForCharge = 40f;
        public const float WaitTimeRequiredForMaximumCharge = 180f;
        public const float MinChargeSpeed = 16f;
        public const float MaxChargeSpeed = 30f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.WriteVector2(IdealVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            IdealVelocity = reader.ReadVector2();
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead ||
                Projectile.Center.Y < 200f ||
                Projectile.Center.X < 200f ||
                Projectile.Center.X > Main.maxTilesX * 16f - 200f)
            {
                Projectile.Kill();
                return;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            Projectile.ai[0]++;
            if (Projectile.velocity == Vector2.Zero)
            {
                // Immediately die if the player is not holding the spear
                if (player.ActiveItem() == null)
                {
                    Projectile.Kill();
                    return;
                }
                if (player.ActiveItem().type != ModContent.ItemType<RelicOfDeliverance>())
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.rotation = Projectile.AngleTo(Main.MouseWorld);
                Projectile.Center = player.Center;
                if (Projectile.ai[0] >= WaitTimeRequiredForCharge)
                {
                    // Begin the lunge
                    bool noObstaclesInWay = Collision.CanHitLine(Projectile.Center, 1, 1, Projectile.Center + Projectile.SafeDirectionTo(Main.MouseWorld) * 25f, 1, 1);
                    if (Main.myPlayer == Projectile.owner && Main.mouseLeft && !Main.blockMouse && !player.mouseInterface && noObstaclesInWay)
                    {
                        float chargeSpeed = MathHelper.Lerp(MinChargeSpeed, MaxChargeSpeed, (Projectile.ai[0] - WaitTimeRequiredForCharge) / WaitTimeRequiredForMaximumCharge);
                        if (chargeSpeed > MaxChargeSpeed)
                            chargeSpeed = MaxChargeSpeed;
                        Projectile.velocity = Projectile.SafeDirectionTo(Main.MouseWorld) * chargeSpeed;
                        IdealVelocity = Projectile.velocity;
                        Projectile.ai[1] = chargeSpeed;
                        SoundEngine.PlaySound(SoundID.Item73, Projectile.Center);
                        Projectile.netUpdate = true;
                    }
                    // Charge-up dust
                    if (Projectile.ai[0] <= WaitTimeRequiredForCharge + WaitTimeRequiredForMaximumCharge)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 dustPosition = Utils.NextVector2Circular(Main.rand, Projectile.width * 1.6f, Projectile.height * 2.3f) / 2f + Projectile.Center;
                            Dust dust = Dust.NewDustPerfect(dustPosition, (int)CalamityDusts.ProfanedFire);
                            dust.velocity = Projectile.SafeDirectionTo(dust.position) * Projectile.Distance(dust.position) / -11f;
                            dust.velocity += player.velocity;
                            dust.noGravity = true;
                            dust.fadeIn = 1.6f;
                        }
                        Projectile.Center += Utils.NextVector2Circular(Main.rand, 1.8f, 1.8f);
                    }
                    // Full charge dust indicator
                    if (Projectile.ai[0] == WaitTimeRequiredForCharge + WaitTimeRequiredForMaximumCharge)
                    {
                        for (int i = 0; i < 60; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.ProfanedFire);
                            dust.velocity = Utils.NextVector2Circular(Main.rand, 9f, 9f) * Main.rand.NextFloat(0.8f, 1.2f);
                            dust.velocity += player.velocity;
                            dust.noGravity = true;
                            dust.fadeIn = 1.6f;
                        }
                        SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, Projectile.Center);
                    }
                    player.ChangeDir((Math.Cos(Projectile.rotation) > 0).ToDirectionInt());
                }
            }
            else
            {
                // Adjust direction so that the spear is always launching in the direction of the mouse.
                // However, if the mouse position is really close to the projectile, don't do anything.
                // Being close to the target may cause erratic movements.
                // The projectile.localAI[1] part is to prevent the projectile from redirecting on a frame-by-frame basis (Main.mouseLeft seems to mean mouse hold, not release).
                if (Projectile.Distance(Main.MouseWorld) > 60f &&
                    Main.mouseLeft && !Main.blockMouse && !player.mouseInterface &&
                    Projectile.localAI[0] <= MaxCharges &&
                    Projectile.localAI[1] <= 0f)
                {
                    IdealVelocity = Projectile.SafeDirectionTo(Main.MouseWorld) * Projectile.ai[1];
                    Projectile.localAI[0]++;
                    Projectile.localAI[1] = 40f;
                    SoundEngine.PlaySound(SoundID.Item73, Projectile.Center);
                    Projectile.netUpdate = true;
                }
                else if (Projectile.localAI[0] > MaxCharges)
                {
                    if (Projectile.timeLeft > 120)
                    {
                        Projectile.timeLeft = 120;
                    }
                    Projectile.alpha = (int)MathHelper.Lerp(0, 255, 1f - Projectile.timeLeft / 120f);
                }
                if (Projectile.velocity != IdealVelocity)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, IdealVelocity, 0.125f);
                }

                player.velocity = Projectile.velocity;
                // Count down the charge cooldown
                if (Projectile.localAI[1] > 0f)
                    Projectile.localAI[1]--;
                Projectile.rotation = Projectile.velocity.ToRotation();
                // Player manipulations (setting the center point and disallowing mounts).
                player.Center = Projectile.Center;
                // Adjust the player's held projectile type.
                if (player.heldProj == -1)
                    player.heldProj = Projectile.whoAmI;
                if (player.mount != null)
                {
                    player.mount.Dismount(player);
                }
                player.ChangeDir((int)Projectile.velocity.X);
                // Generate dust
                if (Projectile.ai[0] % DustSpawnInterval == DustSpawnInterval - 1f)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + (Projectile.Size / 2f).RotatedBy(Projectile.rotation), (int)CalamityDusts.ProfanedFire);
                        dust.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(20f * (i % 2 == 0).ToDirectionInt()));
                        dust.noGravity = true;
                        dust.fadeIn = 1.8f;
                    }
                }
            }
            // Don't die when colliding with tiles unless the spear is lunging (and after a bit of time has passed).
            Projectile.tileCollide = Projectile.velocity != Vector2.Zero && Projectile.ai[0] >= WaitTimeRequiredForCharge + 10f;

            // Die immediately if the owner of this projectile is clipping into tiles because of its movement.
            if (Collision.SolidCollision(player.position, player.width, player.height) && Projectile.velocity != Vector2.Zero)
            {
                player.velocity.Y = 0f;
                DoDeathDust(Projectile.oldVelocity);

                Projectile.Kill();
            }

            Lighting.AddLight(Projectile.Center, Color.LightGoldenrodYellow.ToVector3());
        }
        public void DoDeathDust(Vector2 oldVelocity)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 60; i++)
            {
                Vector2 spawnOffset = (Projectile.Size / 2f).RotatedBy(Projectile.rotation);
                spawnOffset += Utils.NextVector2Circular(Main.rand, 10f, 10f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + spawnOffset, (int)CalamityDusts.ProfanedFire);
                dust.velocity = Utils.NextVector2Circular(Main.rand, 5f, 5f) + oldVelocity;
                dust.noGravity = true;
                dust.fadeIn = 1.9f;
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }


        public override bool? CanDamage() => Projectile.velocity != Vector2.Zero ? null : false;

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        // Force the spear to have "priority" when drawing so that it draws over the player.
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            DoDeathDust(oldVelocity);
            return base.OnTileCollide(oldVelocity);
        }
    }
}
