using CalamityMod.CalPlayer;
using CalamityMod.Buffs.Mounts;
using CalamityMod.Items;
using CalamityMod.Projectiles.Summon.AndromedaUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;

namespace CalamityMod.Projectiles.Summon
{
    public class AndromedaHead : EquipTexture
    {
        public override bool DrawHead() => false;
    }
    public class GiantIbanRobotOfDoom : ModProjectile
    {
        public int FrameX = 0;
        public int FrameY = 0;
        public int CurrentFrame
        {
            get => FrameY + FrameX * 7;
            set
            {
                FrameX = value / 7;
                FrameY = value % 7;
            }
        }
        public bool LeftBracketActive = false;
        public bool RightBracketActive = true; // This is supposed to be the default bracket, according to Iban. Ask him before changing this.
        public bool BottomBracketActive = false;

        public bool LeftIconActive = false;
        public bool TopIconActive = false;

        public int RightIconCooldown = 0;
        public const int RightIconAttackTime = 480; // 8 second wait
        public const int RightIconCooldownMax = RightIconAttackTime * 2; // 16 second wait.

        /// <summary>
        /// This cooldown is set in <see cref="CalamityGlobalItem.PerformAndromedaAttacks"/>, not <see cref="GiantIbanRobotOfDoom"/>
        /// </summary>
        public int LaserCooldown = 0;
        public static Vector2 LightningShootOffset;
        public const int SpecialLightningBaseDamage = 5600;
        public const int RegicideBaseDamageSmall = 12650;
        public const int RegicideBaseDamageLarge = 17800;
        public const int LaserBaseDamage = 21000;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Andromeda");
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 152;
            projectile.height = 212;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 18000;
            projectile.timeLeft *= 5;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.Calamity().rogue = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 11;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            InitializeValues(player);
            HandleCooldowns(player);
            ManipulatePlayerValues(player);
            RegisterRightClick(player);
            SetFrames(player);
            SetSpriteDirection(player);
            player.AddBuff(LeftIconActive ? ModContent.BuffType<AndromedaSmallBuff>() : ModContent.BuffType<AndromedaBuff>(), 2);
        }
        public void InitializeValues(Player player)
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] = 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
        }
        public void HandleCooldowns(Player player)
        {
            if (RightIconCooldown > 0)
            {
                RightIconCooldown--; // The shooting of the lightning is done in the SetFrames method.
            }
            if (LaserCooldown > 0)
            {
                LaserCooldown--;
                LaserBeam(player);
            }
        }
        public void ManipulatePlayerValues(Player player)
        {
            projectile.Center = player.Center + Vector2.UnitY * (6f + player.gfxOffY);
            player.Calamity().andromedaState = LeftIconActive ? AndromedaPlayerState.SmallRobot : AndromedaPlayerState.LargeRobot;
            player.channel = false;
            if (player.mount != null) // Kill any mounts
            {
                player.mount.Dismount(player);
            }
        }
        public void RegisterRightClick(Player player)
        {
            if (Main.mouseRight && projectile.ai[0] <= 0f)
            {
                projectile.ai[0] = 30f;
                // If the player has any existing UIs, kill them all.
                if (player.ownedProjectileCounts[ModContent.ProjectileType<AndromedaUI_Background>()] > 0)
                {
                    for (int i = 0; i < Main.projectile.Length; i++)
                    {
                        if (Main.projectile[i].active &&
                            Main.projectile[i].type == ModContent.ProjectileType<AndromedaUI_Background>() &&
                            Main.projectile[i].owner == player.whoAmI)
                        {
                            Main.projectile[i].Kill();
                        }
                    }
                }
                // Otherwise, create one.
                else
                {
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile ui = Projectile.NewProjectileDirect(projectile.Center + new Vector2(40f, -260f),
                                                 Vector2.Zero,
                                                 ModContent.ProjectileType<AndromedaUI_Background>(),
                                                 0,
                                                 0f,
                                                 player.whoAmI);
                        ui.localAI[0] = Projectile.GetByUUID(projectile.owner, projectile.whoAmI);
                        ui.netUpdate = true;
                    }
                }
            }
            else if (projectile.ai[0] > 0f)
            {
                projectile.ai[0]--;
            }
        }
        public void SetFrames(Player player)
        {
            projectile.frameCounter++;
            if (RightIconCooldown <= RightIconAttackTime)
            {
                if (Math.Abs(player.velocity.Y) != 0f)
                {
                    SetFlyingFrames(player);
                }
                else if (Math.Abs(player.velocity.X) > 3f)
                {
                    SetWalkingFrames();
                }
                if (player.velocity == Vector2.Zero)
                {
                    CurrentFrame = 0;
                }
            }
            else
            {
                SpecialLightningAttack(player);
            }
        }
        public void SetFlyingFrames(Player player)
        {
            // Falling
            if (player.velocity.Y > 0f)
            {
                CurrentFrame = 1;
                return;
            }
            // Quickly go to flying frames
            if (CurrentFrame == 0)
            {
                if (projectile.frameCounter >= 4)
                {
                    CurrentFrame++;
                    projectile.frameCounter = 0;
                }
            }
            // Flying frames
            else if (projectile.frameCounter % 4 == 3)
            {
                CurrentFrame++;
                if (CurrentFrame >= 6)
                    CurrentFrame = 2;
            }

            // Dust effect

            Vector2 dustOffset = new Vector2(94f, 58f);
            if (projectile.spriteDirection == -1)
            {
                dustOffset.X = 214 - dustOffset.X;
            }
            if (!Main.dedServ && player.Calamity().andromedaState == AndromedaPlayerState.LargeRobot)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.position + dustOffset, 263);
                    dust.velocity = Vector2.Normalize(dust.position - projectile.Top).RotatedByRandom(0.4f) * Main.rand.NextFloat(4f, 7f) + projectile.velocity;
                    dust.color = Color.SkyBlue;
                    dust.scale = Main.rand.NextFloat(0.9f, 1.35f);
                    dust.noGravity = true;
                }
            }

            // Clamp frames
            if (CurrentFrame >= 6)
                CurrentFrame = 0;
        }
        public void SetWalkingFrames()
        {
            Player player = Main.player[projectile.owner];
            int walkInterval = (int)MathHelper.Clamp(8 - Math.Abs(player.velocity.X) / 3.5f, 1, 8);
            if (player.velocity.X == 0f)
            {
                CurrentFrame = 0;
            }
            else if (projectile.frameCounter >= walkInterval)
            {
                projectile.frameCounter = 0;
                CurrentFrame++;
                if (CurrentFrame >= 13)
                {
                    CurrentFrame = 6;
                }
            }
            // Clamp frames
            if (CurrentFrame >= 14 || CurrentFrame <= 6)
                CurrentFrame = 6;
        }
        public void SetSpriteDirection(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<AndromedaDeathLightning>()] > 0)
                return;
            if (player.velocity.X != 0) // So that the original sprite direction is maintained when there is no X movement.
            {
                projectile.spriteDirection = (player.velocity.X > 0).ToDirectionInt();
            }
            int slashIndex = -1;

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active &&
                    Main.projectile[i].type == ModContent.ProjectileType<AndromedaRegislash>() &&
                    Main.projectile[i].owner == projectile.owner)
                {
                    slashIndex = i;
                    break;
                }
            }

            if (slashIndex != -1)
            {
                if (Main.projectile[slashIndex].frameCounter > 0) // To ensure that the starting variables of the blade have been initialized
                {
                    projectile.spriteDirection = (Math.Cos(Main.projectile[slashIndex].rotation) > 0).ToDirectionInt(); // ai[1] is the blade's starting rotation
                }
            }

            int laserBeamIndex = -1;

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active &&
                    Main.projectile[i].type == ModContent.ProjectileType<AndromedaDeathRay>() &&
                    Main.projectile[i].owner == projectile.owner)
                {
                    laserBeamIndex = i;
                    break;
                }
            }

            if (laserBeamIndex != -1)
            {
                projectile.spriteDirection = (Math.Cos(Main.projectile[laserBeamIndex].velocity.ToRotation()) > 0).ToDirectionInt(); // ai[1] is the blade's starting rotation
            }
        }
        public void LaserBeam(Player player)
        {
            if (LaserCooldown % (AndromedaDeathRay.TrueTimeLeft - 10) == (AndromedaDeathRay.TrueTimeLeft - 11))
            {
                if (projectile.owner == Main.myPlayer)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeMechGaussRifle"), projectile.Center);
                    int damage = LaserBaseDamage;
                    if (player.HeldItem != null)
                    {
                        if (player.HeldItem.magic)
                        {
                            damage = (int)(damage * player.MagicDamage());
                        }
                        else if (player.HeldItem.melee)
                        {
                            damage = (int)(damage * player.MeleeDamage());
                        }
                        else if (player.HeldItem.ranged)
                        {
                            damage = (int)(damage * player.RangedDamage());
                        }
                        else if (player.HeldItem.summon)
                        {
                            damage = (int)(damage * player.MinionDamage());
                        }
                        else if (player.HeldItem.Calamity().rogue)
                        {
                            damage = (int)(damage * player.RogueDamage());
                        }
                        else
                        {
                            damage = (int)(damage * player.AverageDamage());
                        }
                    }
                    Vector2 laserVelocity = (Main.MouseWorld - (Main.player[projectile.owner].Center + new Vector2(projectile.spriteDirection == 1 ? 48f : 22f, -28f))).SafeNormalize(Vector2.UnitX * projectile.spriteDirection);
                    Projectile deathLaser = Projectile.NewProjectileDirect(projectile.Center,
                                                                           laserVelocity,
                                                                           ModContent.ProjectileType<AndromedaDeathRay>(),
                                                                           damage,
                                                                           8f,
                                                                           projectile.owner,
                                                                           projectile.whoAmI);
                    if (player.HeldItem != null)
                    {
                        if (player.HeldItem.magic)
                        {
                            deathLaser.Calamity().forceMagic = true;
                        }
                        else if (player.HeldItem.melee)
                        {
                            deathLaser.Calamity().forceMelee = true;
                        }
                        else if (player.HeldItem.ranged)
                        {
                            deathLaser.Calamity().forceRanged = true;
                        }
                        else if (player.HeldItem.summon)
                        {
                            deathLaser.Calamity().forceMinion = true;
                        }
                        else if (player.HeldItem.Calamity().rogue)
                        {
                            deathLaser.Calamity().forceRogue = true;
                        }
                        else
                        {
                            deathLaser.Calamity().forceTypeless = true;
                        }
                    }
                }
            }
        }
        public void SpecialLightningAttack(Player player)
        {
            int adjustedCooldownTime = RightIconCooldown - RightIconAttackTime;
            // Ensure the projectile is in the correct frame range
            if (adjustedCooldownTime > RightIconAttackTime - 10)
            {
                CurrentFrame = 14;
            }
            // If it is, increment the frames
            else if (adjustedCooldownTime > RightIconAttackTime - 40 && adjustedCooldownTime % 10f == 9f)
            {
                CurrentFrame++;
            }

            LightningShootOffset = new Vector2(projectile.spriteDirection == 1 ? 94f : -28f, -24f);
            if (LeftIconActive)
            {
                LightningShootOffset = new Vector2(projectile.spriteDirection == 1 ? 14f : -8f, -16f);
            }

            // After a certain period of time, release a burst of 3 fast lightning bolts that arc with time.
            if (adjustedCooldownTime == RightIconAttackTime - 40 && Main.myPlayer == projectile.owner)
            {
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectileDirect(projectile.Center + LightningShootOffset, projectile.DirectionTo(Main.MouseWorld).RotatedBy(MathHelper.TwoPi / 2f * i) * 1.3f,
                        ModContent.ProjectileType<AndromedaDeathLightning>(),
                        (int)(SpecialLightningBaseDamage * player.AverageDamage()), 1.5f, projectile.owner, Main.rand.Next()).timeLeft = 20 * AndromedaDeathLightning.TrueTimeLeft / 2;

                }
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBolt"), projectile.Center);
                CurrentFrame = 18;
            }
            // After the initial burst, every 2.5 seconds, release slower bolts.
            if (adjustedCooldownTime <= RightIconAttackTime - 40 &&
                adjustedCooldownTime >= 80 &&
                Main.myPlayer == projectile.owner)
            {
                if (adjustedCooldownTime % 150 == 149 && Main.myPlayer == projectile.owner)
                {
                    float startingAngle = Main.rand.NextFloat(MathHelper.TwoPi);
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectileDirect(projectile.Center + LightningShootOffset, Vector2.UnitY.RotatedBy(MathHelper.TwoPi / 2f * i + startingAngle) * 0.5f,
                            ModContent.ProjectileType<AndromedaDeathLightning>(),
                            (int)(SpecialLightningBaseDamage * player.AverageDamage()), 1.5f, projectile.owner, Main.rand.Next());
                    }
                    if (!Main.dedServ)
                    {
                        for (int i = 0; i < 60; i++)
                        {
                            List<float> circularBurstSpeeds = new List<float>() { 8f, 13f, 19f };
                            for (int j = 0; j < circularBurstSpeeds.Count; j++)
                            {
                                float angle = MathHelper.TwoPi / 32f * i;
                                Dust dust = Dust.NewDustPerfect(projectile.Center + LightningShootOffset, 133);
                                dust.velocity = player.velocity + angle.ToRotationVector2() * circularBurstSpeeds[j];
                                dust.noGravity = true;
                            }
                        }
                    }
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBolt"), projectile.Center);
                }
                if (!Main.dedServ)
                {
                    float radius = LeftIconActive ? 10f : 24f;
                    for (int i = 0; i < 16; i++)
                    {
                        float angle = MathHelper.TwoPi / 16f * i;
                        Dust dust = Dust.NewDustPerfect(projectile.Center + LightningShootOffset + angle.ToRotationVector2() * radius, 133);
                        dust.velocity = Vector2.Zero;
                        if (Main.rand.NextBool(8))
                        {
                            dust.velocity.X = -projectile.spriteDirection * 2.4f;
                            if (LeftIconActive)
                            {
                                dust.velocity.X = -projectile.spriteDirection * 2.4f;
                            }
                        }
                        dust.fadeIn = -0.5f;
                        dust.noGravity = true;
                    }
                }
            }

            // Reduce the player's max Y speed
            player.velocity.Y = MathHelper.Clamp(player.velocity.Y, -7f, 7f);

            // Light burst effect
            if (adjustedCooldownTime <= 40)
            {
                // Begin frame incrementation again 
                if (adjustedCooldownTime == 40)
                {
                    CurrentFrame++;
                }
                // At the apex of the light burst, cause all lightning to disappear and play a sound
                if (adjustedCooldownTime == 20)
                {
                    for (int i = 0; i < Main.projectile.Length; i++)
                    {
                        if (Main.projectile[i].active &&
                            Main.projectile[i].type == ModContent.ProjectileType<AndromedaDeathLightning>() &&
                            Main.projectile[i].owner == projectile.owner)
                        {
                            Main.projectile[i].Kill();
                        }
                    }
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/TeslaCannonFire"), projectile.Center);
                    CurrentFrame++;
                }
                float lightRequested;
                if (adjustedCooldownTime >= 25f)
                {
                    lightRequested = MathHelper.Lerp(0f, 1f, 1f - (adjustedCooldownTime - 20f) / 20f); // Begin lighting up
                }
                else if (adjustedCooldownTime >= 15f)
                {
                    lightRequested = 1f; // Hold the light for 10 frames
                }
                else
                {
                    lightRequested = MathHelper.Lerp(1f, 0f, 1f - adjustedCooldownTime / 20f); // And fade back to normalcy
                }
                MoonlordDeathDrama.RequestLight(lightRequested, projectile.Center);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false; // Drawing is done completely by the player.
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override bool CanDamage() => false;
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            player.width = 20;
            player.height = 42;
        }
    }
}
