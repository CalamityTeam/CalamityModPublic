using CalamityMod.Buffs;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.EntitySources;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Projectiles.VanillaProjectileOverrides;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;
using NanotechProjectile = CalamityMod.Projectiles.Typeless.Nanotech;

namespace CalamityMod.Projectiles
{
    public partial class CalamityGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        // Source variables.
        public bool CreatedByPlayerDash = false;

        // Damage Adjusters
        public const float PierceResistHarshness = 0.12f;
        public const float PierceResistCap = 0.8f;

        // defDamage was being used for frame1 hacks. this stands in as the replacement for that logic.
        private bool frameOneHacksExecuted = false;

        // Enables "supercrits". When crit is over 100%, projectiles with this bool enabled can "supercrit".
        // For every 100% critical strike chance over 100%, "supercrit" projectiles do a guaranteed +100% damage.
        // They then take the remainder (e.g. the remaining 16%) and roll against that for a final +100% (like normal crits).
        // For example if you have 716% critical strike chance, you are guaranteed +700% damage and then have a 16% chance for +800% damage instead.
        // These are currently only enabled for Soma Prime, but any bullet fired from that gun can supercrit.
        // Set this to -1 if you want the projectile to supercrit forever, and to any positive value to make it supercrit only x times
        public int supercritHits  = 0;

        // Without adjusting underlying crit calculations, set this to true to force a projectile as a crit.
        // TODO -- In the TML 1.4.4 port, there is a much better way to set NPC strike events to be forced crits.
        public bool forcedCrit = false;

        // The total bonus damage (as a ratio of the projectile's own damage) applied to this projectile as a result of a ricoshot combo.
        public float totalRicoshotDamageBonus = 0f;

        // If true, this projectile can apply the infinitely-stacking Shred debuff iconic to Soma Prime.
        public bool appliesSomaShred = false;

        // If true, this projectile creates impact sparks upon hitting enemies
        public bool deepcoreBullet = false;

        // If true, causes all projectiles fired by this weapon to have homing. Currently used for Arterial Assault.
        public bool allProjectilesHome = false;

        // Amount of extra updates that are set in SetDefaults.
        public int defExtraUpdates = -1;

        // How many times this projectile has pierced.
        public int timesPierced = 0;

        // Point-blank shot timer and distance check.
        public int pointBlankShotDuration = 0;
        public float pointBlankShotDistanceTravelled = 0f;
        public const int DefaultPointBlankDuration = 18; // 18 frames
        public const float PointBlankShotDistanceLimit = 240f; // 15 tiles

        // Temporary flat damage reduction effects. This is typically used for parry effects such as Ark of the Ancients
        public int flatDRTimer = 0;
        /// <summary>
        /// The amount of final damage substracted from the projectile's own damage count when hitting the player. Resets to 0 if the flatDRTimer variable drops to 0
        /// </summary>
        public int flatDR = 0;

        /// <summary>
        /// Allows hostile Projectiles to deal damage to the player's defense stat, used mostly for hard-hitting bosses.
        /// </summary>
        public bool DealsDefenseDamage = false;

        // Nihility Quiver
        public bool nihilicArrow = false;

        // Rogue Stuff
        public bool stealthStrike = false;
        public int stealthStrikeHitCount = 0;
        public bool extorterBoost = false;
        public bool LocketClone = false;
        public bool CannotProc = false;

        // Note: Although this was intended for fishing line colors, I use this as an AI variable a lot because vanilla only has 4 that sometimes are already in use.  ~Ben
        // TODO -- uses of this variable are undocumented and unstable. Remove it from the API surface.
        public int lineColor = 0;

        // This flag is set to true on summon-classed attacks that are NOT minions, and thus should ALWAYS be able to hit enemies ALL the time.
        // There are several enemies/NPCs in Calamity which do not take damage from minions in certain circumstances.
        public bool overridesMinionDamagePrevention = false;

        public static List<int> MechBossProjectileIDs = new()
        {
            ProjectileID.DeathLaser,
            ProjectileID.PinkLaser,
            ProjectileID.BombSkeletronPrime,
            ProjectileID.CursedFlameHostile,
            ProjectileID.EyeFire,
            ProjectileID.EyeLaser,
            ProjectileID.Skull,
            ProjectileID.SaucerMissile,
            ProjectileID.RocketSkeleton,
            ProjectileType<DestroyerCursedLaser>(),
            ProjectileType<DestroyerElectricLaser>(),
            ProjectileType<ShadowflameFireball>(),
            ProjectileType<Shadowflamethrower>(),
            ProjectileType<ScavengerLaser>()
        };

        // Enchantment variables.
        public int ExplosiveEnchantCountdown = 0;
        public const int ExplosiveEnchantTime = 2400;

        // Custom update priority.
        // Calamity sorts projectiles by their update priority to fix otherwise absurdly difficult to resolve visual bugs on certain weapons.
        // Examples include Mechworm segments detaching or Rancor's laser beam being offset from the magic circle.
        public float UpdatePriority = 0f;

        #region On Spawn

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            CreatedByPlayerDash = source is ProjectileSource_PlayerDashHit;

            IEntitySource sourceItem = source as EntitySource_ItemUse_WithAmmo;
            if (sourceItem != null)
                extorterBoost = true;

            // TODO -- it would be nice to move frame one hacks here, but this runs in the middle of NewProjectile
            // which is way too early, the projectile's own initialization isn't even done yet
        }
        #endregion On Spawn

        #region Set Defaults
        public override void SetDefaults(Projectile projectile)
        {
            // OLD 1.3 CODE: Disable Lunatic Cultist's homing resistance globally
            // ProjectileID.Sets.CultistIsResistantTo[projectile.type] = false;

            // Apply Calamity Global Projectile Tweaks.
            SetDefaults_ApplyTweaks(projectile);
        }
        #endregion

        #region Pre AI
        public override bool PreAI(Projectile projectile)
        {
            #region Vanilla Summons AI Changes

            //
            // MINION AI CHANGES:
            //

            // Hornet Staff's minion changes.
            if (projectile.type == ProjectileID.Hornet)
                return HornetMinionAI.DoHornetMinionAI(projectile);
            
            // Imp Staff's minion changes.
            if (projectile.type == ProjectileID.FlyingImp)
                return ImpMinionAI.DoImpMinionAI(projectile);

            // Raven Staff's minion changes.
            if (projectile.type == ProjectileID.Raven)
                return RavenMinionAI.DoRavenMinionAI(projectile);

            //
            // SENTRY AI CHANGES:
            //

            // Deerclop's sentry drop projectile changes.
            if (projectile.type == ProjectileID.HoundiusShootiusFireball)
                return HoundiusShootiusFireballAI.DoHoundiusShootiusFireballAI(projectile);

            #endregion
            
            if (!Main.player[projectile.owner].ActiveItem().IsAir && !Main.player[projectile.owner].ActiveItem().Calamity().canFirePointBlankShots)
                pointBlankShotDuration = 0;

            if (pointBlankShotDuration > 0)
                pointBlankShotDuration--;
            if (pointBlankShotDistanceTravelled < PointBlankShotDistanceLimit)
                pointBlankShotDistanceTravelled += projectile.velocity.Length() * projectile.MaxUpdates;

            // Reduce secondary yoyo damage if the player has Yoyo Glove
            // Brief behavior documentation of yoyo AI: ai[0, 1] are the x, y co-ords and localAI[0] is the airtime in frames
            // All secondary yoyos are spawned with ai[0] of 1 which tells then tell its AI to do secondary yoyo AI
            if (Main.player[projectile.owner].yoyoGlove && projectile.aiStyle == ProjAIStyleID.Yoyo)
            {
                // Store damage on first frame
                if (projectile.ai[2] == 0f)
                    projectile.ai[2] = projectile.damage;

                // Find the first yoyo projectile owned by the corresponding player
                // Limited lifespan yoyos are horrendous so it had to be this way
                // EDIT: ownedProjectileCounts does not work what the fuck
                int MainYoyo = -1;
                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile proj = Main.projectile[x];
                    if (proj.active && proj.type == projectile.type && proj.owner == projectile.owner)
                    {
                        MainYoyo = x;
                        break;
                    }
                }

                // Halve damage if not the main yoyo
                if (projectile.whoAmI != MainYoyo)
                    projectile.damage = (int)(projectile.ai[2] * 0.5f);
                else
                    projectile.damage = (int)projectile.ai[2];
            }

            // Chlorophyte Crystal AI rework.
            if (projectile.type == ProjectileID.CrystalLeaf)
                return ChlorophyteCrystalAI.DoChlorophyteCrystalAI(projectile);

            if (projectile.minion && ExplosiveEnchantCountdown > 0)
            {
                ExplosiveEnchantCountdown--;
                projectile.damage = (int)(projectile.originalDamage * MathHelper.SmoothStep(1f, 1.6f, 1f - ExplosiveEnchantCountdown / (float)ExplosiveEnchantTime));

                // Make fizzle sounds and fire dust to indicate the impending explosion.
                if (ExplosiveEnchantCountdown <= 300)
                {
                    if (Main.rand.NextBool(24))
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, projectile.Center);

                    Dust fire = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(projectile.width, projectile.height) * 0.42f, 267);
                    fire.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0.45f, 1f));
                    fire.scale = Main.rand.NextFloat(1.4f, 1.65f);
                    fire.fadeIn = 0.5f;
                    fire.noGravity = true;
                }

                if (ExplosiveEnchantCountdown % 40 == 39 && Main.rand.NextBool(12))
                {
                    int damage = (int)Main.player[projectile.owner].GetTotalDamage<SummonDamageClass>().ApplyTo(2000);
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<SummonBrimstoneExplosionSmall>(), damage, 0f, projectile.owner);
                }

                if (ExplosiveEnchantCountdown <= 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, projectile.Center);
                    if (Main.myPlayer == projectile.owner)
                    {
                        if (projectile.minionSlots > 0f)
                        {
                            int damage = (int)Main.player[projectile.owner].GetTotalDamage<SummonDamageClass>().ApplyTo(6000);
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<SummonBrimstoneExplosion>(), damage, 0f, projectile.owner);
                        }
                        projectile.Kill();
                    }
                }
            }

            if (projectile.type == ProjectileID.Skull && (projectile.ai[0] == 0f || projectile.ai[0] == -2f))
            {
                if (projectile.alpha > 0)
                    projectile.alpha -= 75;

                if (projectile.alpha < 0)
                    projectile.alpha = 0;

                projectile.frame++;
                if (projectile.frame > 2)
                    projectile.frame = 0;

                // Accelerate if fired in a spread from Skeletron
                if (projectile.ai[0] == -2f)
                {
                    if (projectile.velocity.Length() < 15f)
                    {
                        projectile.velocity *= 1.02f;
                        if (projectile.velocity.Length() > 15f)
                        {
                            projectile.velocity.Normalize();
                            projectile.velocity *= 12f;
                        }
                    }
                }

                for (int num172 = 0; num172 < 2; num172++)
                {
                    Dust flame = Dust.NewDustDirect(new Vector2(projectile.position.X + 4f, projectile.position.Y + 4f), projectile.width - 8, projectile.height - 8, 6, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                    flame.position -= projectile.velocity * 2f;
                    flame.noGravity = true;
                    flame.velocity *= 0.3f;
                }

                if (projectile.ai[0] == 0f)
                {
                    float num180 = (float)Math.Sqrt(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y);
                    float num181 = projectile.localAI[0];

                    if (num181 == 0f)
                    {
                        projectile.localAI[0] = num180;
                        num181 = num180;
                    }

                    float num182 = projectile.position.X;
                    float num183 = projectile.position.Y;
                    float num184 = 300f;
                    bool flag4 = false;
                    int num185 = 0;

                    if (projectile.ai[1] == 0f)
                    {
                        for (int num186 = 0; num186 < Main.maxNPCs; num186++)
                        {
                            if (Main.npc[num186].CanBeChasedBy(this) && (projectile.ai[1] == 0f || projectile.ai[1] == (float)(num186 + 1)))
                            {
                                float num187 = Main.npc[num186].position.X + (float)(Main.npc[num186].width / 2);
                                float num188 = Main.npc[num186].position.Y + (float)(Main.npc[num186].height / 2);
                                float num189 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num187) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num188);
                                if (num189 < num184 && Collision.CanHit(new Vector2(projectile.position.X + (float)(projectile.width / 2), projectile.position.Y + (float)(projectile.height / 2)), 1, 1, Main.npc[num186].position, Main.npc[num186].width, Main.npc[num186].height))
                                {
                                    num184 = num189;
                                    num182 = num187;
                                    num183 = num188;
                                    flag4 = true;
                                    num185 = num186;
                                }
                            }
                        }

                        if (flag4)
                            projectile.ai[1] = num185 + 1;

                        flag4 = false;
                    }

                    if (projectile.ai[1] > 0f)
                    {
                        int num190 = (int)(projectile.ai[1] - 1f);
                        if (Main.npc[num190].active && Main.npc[num190].CanBeChasedBy(this, ignoreDontTakeDamage: true) && !Main.npc[num190].dontTakeDamage)
                        {
                            float num191 = Main.npc[num190].position.X + (float)(Main.npc[num190].width / 2);
                            float num192 = Main.npc[num190].position.Y + (float)(Main.npc[num190].height / 2);
                            if (Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num191) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num192) < 1000f)
                            {
                                flag4 = true;
                                num182 = Main.npc[num190].position.X + (float)(Main.npc[num190].width / 2);
                                num183 = Main.npc[num190].position.Y + (float)(Main.npc[num190].height / 2);
                            }
                        }
                        else
                            projectile.ai[1] = 0f;
                    }

                    if (!projectile.friendly)
                        flag4 = false;

                    if (flag4)
                    {
                        float num193 = num181;
                        Vector2 vector9 = new(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                        float num194 = num182 - vector9.X;
                        float num195 = num183 - vector9.Y;
                        float num196 = (float)Math.Sqrt(num194 * num194 + num195 * num195);
                        num196 = num193 / num196;
                        num194 *= num196;
                        num195 *= num196;
                        int num197 = 32;
                        projectile.velocity.X = (projectile.velocity.X * (float)(num197 - 1) + num194) / (float)num197;
                        projectile.velocity.Y = (projectile.velocity.Y * (float)(num197 - 1) + num195) / (float)num197;
                    }
                }

                projectile.spriteDirection = projectile.direction;

                if (projectile.direction < 0)
                    projectile.rotation = (float)Math.Atan2(0f - projectile.velocity.Y, 0f - projectile.velocity.X);
                else
                    projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);

                return false;
            }

            else if (projectile.type == ProjectileID.BloodNautilusShot)
            {
                if (projectile.localAI[0] == 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item171, projectile.Center);
                    projectile.localAI[0] = 1f;
                    for (int num160 = 0; num160 < 8; num160++)
                    {
                        Dust blood1 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 5, projectile.velocity.X, projectile.velocity.Y, 100);
                        blood1.velocity = (Main.rand.NextFloatDirection() * MathHelper.Pi).ToRotationVector2() * 2f + projectile.velocity.SafeNormalize(Vector2.Zero) * 2f;
                        blood1.scale = 0.9f;
                        blood1.fadeIn = 1.1f;
                        blood1.position = projectile.Center;
                    }
                }

                projectile.alpha -= 20;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;

                for (int num161 = 0; num161 < 2; num161++)
                {
                    Dust blood2 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 5, projectile.velocity.X, projectile.velocity.Y, 100);
                    blood2.velocity = blood2.velocity / 4f + projectile.velocity / 2f;
                    blood2.scale = 1.2f;
                    blood2.position = projectile.Center + Main.rand.NextFloat() * projectile.velocity * 2f;
                }

                for (int num162 = 1; num162 < projectile.oldPos.Length && !(projectile.oldPos[num162] == Vector2.Zero); num162++)
                {
                    if (Main.rand.Next(3) == 0)
                    {
                        Dust blood3 = Dust.NewDustDirect(projectile.oldPos[num162], projectile.width, projectile.height, 5, projectile.velocity.X, projectile.velocity.Y, 100);
                        blood3.velocity = blood3.velocity / 4f + projectile.velocity / 2f;
                        blood3.scale = 1.2f;
                        blood3.position = projectile.oldPos[num162] + projectile.Size / 2f + Main.rand.NextFloat() * projectile.velocity * 2f;
                    }
                }

                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                return false;
            }

            else if (projectile.type == ProjectileID.FrostWave && projectile.ai[1] > 0f)
            {
                if (projectile.ai[0] < 0f)
                {
                    projectile.ai[0] += 1f;
                }
                else
                {
                    if (projectile.velocity.Length() < projectile.ai[1])
                    {
                        projectile.velocity *= 1.04f;
                        if (projectile.velocity.Length() > projectile.ai[1])
                        {
                            projectile.velocity.Normalize();
                            projectile.velocity *= projectile.ai[1];
                        }
                    }
                    else
                    {
                        if (projectile.ai[0] == 0f || projectile.ai[0] == 2f)
                        {
                            projectile.scale += 0.005f;
                            projectile.alpha -= 25;
                            if (projectile.alpha <= 0)
                            {
                                projectile.ai[0] = 1f;
                                projectile.alpha = 0;
                            }
                        }
                        else if (projectile.ai[0] == 1f)
                        {
                            projectile.scale -= 0.005f;
                            projectile.alpha += 25;
                            if (projectile.alpha >= 255)
                            {
                                projectile.ai[0] = 2f;
                                projectile.alpha = 255;
                            }
                        }
                    }
                }

                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                return false;
            }

            else if (projectile.type == ProjectileID.Starfury)
            {
                if (projectile.timeLeft > 75)
                    projectile.timeLeft = 75;

                if (projectile.ai[1] == 0f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[1] = 1f;
                    projectile.netUpdate = true;
                }

                if (projectile.soundDelay == 0)
                {
                    projectile.soundDelay = 20 + Main.rand.Next(40);
                    SoundEngine.PlaySound(SoundID.Item9, projectile.position);
                }

                if (projectile.localAI[0] == 0f)
                    projectile.localAI[0] = 1f;

                projectile.alpha += (int)(25f * projectile.localAI[0]);
                if (projectile.alpha > 200)
                {
                    projectile.alpha = 200;
                    projectile.localAI[0] = -1f;
                }
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                    projectile.localAI[0] = 1f;
                }

                projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * projectile.direction;

                if (projectile.ai[1] == 1f)
                {
                    projectile.light = 0.9f;

                    if (Main.rand.NextBool(10))
                        Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default, 1.2f);

                    if (Main.rand.NextBool(20) && Main.netMode != NetmodeID.Server)
                        Gore.NewGore(projectile.GetSource_FromAI(), projectile.position, projectile.velocity * 0.2f, Main.rand.Next(16, 18), 1f);
                }

                return false;
            }

            // Copy pasted vanilla AI with minor changes to the homing distance and velocity formula
            else if (projectile.type == ProjectileID.SpiritFlame)
            {
                float maxSpeed = 12f;
                int accelerationTime = 30;
                
                if (projectile.localAI[0] > 0f)
                    projectile.localAI[0]--;

                if (projectile.localAI[0] == 0f && projectile.ai[0] < 0f && projectile.owner == Main.myPlayer)
                {
                    projectile.localAI[0] = 5f;
                    for (int num1034 = 0; num1034 < Main.maxNPCs; num1034++)
                    {
                        NPC nPC13 = Main.npc[num1034];
                        if (nPC13.CanBeChasedBy(this))
                        {
                            bool flag63 = projectile.ai[0] < 0f || Main.npc[(int)projectile.ai[0]].Distance(projectile.Center) > nPC13.Distance(projectile.Center);
                            if ((flag63 & (nPC13.Distance(projectile.Center) < 500f)) && (Collision.CanHitLine(projectile.Center, 0, 0, nPC13.Center, 0, 0) || Collision.CanHitLine(projectile.Center, 0, 0, nPC13.Top, 0, 0)))
                                projectile.ai[0] = num1034;
                        }
                    }

                    if (projectile.ai[0] >= 0f)
                    {
                        projectile.timeLeft = 300;
                        projectile.netUpdate = true;
                    }
                }

                if (projectile.timeLeft > 30 && projectile.alpha > 0)
                    projectile.alpha -= 12;

                if (projectile.timeLeft > 30 && projectile.alpha < 128 && Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    projectile.alpha = 128;

                if (projectile.alpha < 0)
                    projectile.alpha = 0;

                if (++projectile.frameCounter > 4)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= 4)
                        projectile.frame = 0;
                }

                float num1035 = 0.5f;
                if (projectile.timeLeft < 120)
                    num1035 = 1.1f;

                if (projectile.timeLeft < 60)
                    num1035 = 1.6f;

                projectile.ai[1]++;
                float num1036 = projectile.ai[1] / 180f * MathHelper.TwoPi;
                for (float num1037 = 0f; num1037 < 3f; num1037++)
                {
                    if (Main.rand.Next(3) == 0)
                    {
                        Dust shflame = Dust.NewDustDirect(projectile.Center, 0, 0, 27, 0f, -2f, 200);
                        shflame.position = projectile.Center + Vector2.UnitY.RotatedBy(num1037 * MathHelper.TwoPi / 3f + projectile.ai[1]) * 10f;
                        shflame.noGravity = true;
                        shflame.velocity = projectile.DirectionFrom(shflame.position);
                        shflame.scale = num1035;
                        shflame.fadeIn = 0.5f;
                    }
                }

                if (projectile.timeLeft > 2 && Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    projectile.timeLeft = 2;

                int num1038 = (int)projectile.ai[0];
                if (num1038 >= 0 && Main.npc[num1038].active)
                {
                    if (projectile.Distance(Main.npc[num1038].Center) > 1f)
                    {
                        Vector2 vector106 = projectile.DirectionTo(Main.npc[num1038].Center).SafeNormalize(Vector2.UnitX);
                        float length = projectile.velocity.Length();
                        float step = maxSpeed / accelerationTime;
                        if (length >= maxSpeed)
                            step = 0f;

                        projectile.velocity = vector106 * (length + step);

                        if (length >= maxSpeed)
                        {
                            if ((projectile.Center + projectile.velocity).Distance(Main.npc[num1038].Center) > projectile.Center.Distance(Main.npc[num1038].Center))
                            {
                                projectile.velocity = Vector2.Zero;
                                projectile.Center = Main.npc[num1038].Center;
                            }
                        }
                    }

                    return false;
                }

                if (projectile.ai[0] == -1f && projectile.timeLeft > 5)
                    projectile.timeLeft = 5;

                if (projectile.ai[0] == -2f && projectile.timeLeft > 180)
                    projectile.timeLeft = 180;

                if (projectile.ai[0] >= 0f)
                {
                    projectile.ai[0] = -1f;
                    projectile.netUpdate = true;
                }

                return false;
            }

            if (projectile.type == ProjectileID.NurseSyringeHeal)
            {
                ref float initialSpeed = ref projectile.localAI[1];
                if (initialSpeed == 0f)
                    initialSpeed = projectile.velocity.Length();

                bool invalidHealTarget = !Main.npc.IndexInRange((int)projectile.ai[0]) || !Main.npc[(int)projectile.ai[0]].active || !Main.npc[(int)projectile.ai[0]].townNPC;
                if (invalidHealTarget)
                {
                    projectile.Kill();
                    return false;
                }

                NPC npcToHeal = Main.npc[(int)projectile.ai[0]];

                // If the needle is not colliding with the target, attempt to move towards it while falling.
                if (!projectile.WithinRange(npcToHeal.Center, initialSpeed) && !projectile.Hitbox.Intersects(npcToHeal.Hitbox))
                {
                    Vector2 flySpeed = projectile.SafeDirectionTo(npcToHeal.Center) * initialSpeed;

                    // Prevent the needle from ever violating its gravity.
                    if (flySpeed.Y < projectile.velocity.Y)
                        flySpeed.Y = projectile.velocity.Y;

                    flySpeed.Y++;

                    projectile.velocity = Vector2.Lerp(projectile.velocity, flySpeed, 0.04f);
                    projectile.rotation += projectile.velocity.X * 0.05f;
                    return false;
                }

                // Otherwise, die immediately and heal the target.
                projectile.Kill();

                int healAmount = npcToHeal.lifeMax - npcToHeal.life;
                int maxHealAmount = 20;

                // If the target has more than 250 max life, incorporate their total life into the max amount to heal.
                // This is done so that more powerful NPCs, such as Cirrus, do not take an eternity to receive meaningful healing benefits
                // from the Nurse.
                if (npcToHeal.lifeMax > 250)
                    maxHealAmount = (int)Math.Max(maxHealAmount, npcToHeal.lifeMax * 0.05f);

                if (healAmount > maxHealAmount)
                    healAmount = maxHealAmount;

                if (healAmount > 0)
                {
                    npcToHeal.life += healAmount;
                    npcToHeal.HealEffect(healAmount, true);
                    return false;
                }

                return false;
            }

            bool adultWyrmAlive = false;
            if (CalamityGlobalNPC.adultEidolonWyrmHead != -1)
            {
                if (Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active)
                    adultWyrmAlive = true;
            }

            if (adultWyrmAlive || (CalamityWorld.death && !CalamityPlayer.areThereAnyDamnBosses))
            {
                if (projectile.type == ProjectileID.CultistBossFireBallClone)
                {
                    if (projectile.ai[1] == 0f)
                    {
                        projectile.ai[1] = 1f;
                        SoundEngine.PlaySound(SoundID.Item34, projectile.position);
                    }
                    else if (projectile.ai[1] == 1f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num13 = -1;
                        float num14 = 2000f;
                        for (int num15 = 0; num15 < Main.maxPlayers; num15++)
                        {
                            if (Main.player[num15].active && !Main.player[num15].dead)
                            {
                                Vector2 center2 = Main.player[num15].Center;
                                float num16 = Vector2.Distance(center2, projectile.Center);
                                if ((num16 < num14 || num13 == -1) && Collision.CanHit(projectile.Center, 1, 1, center2, 1, 1))
                                {
                                    num14 = num16;
                                    num13 = num15;
                                }
                            }
                        }

                        if (num14 < 20f)
                        {
                            projectile.Kill();
                            return false;
                        }

                        if (num13 != -1)
                        {
                            projectile.ai[1] = 21f;
                            projectile.ai[0] = num13;
                            projectile.netUpdate = true;
                        }
                    }
                    else if (projectile.ai[1] > 20f && projectile.ai[1] < 200f)
                    {
                        projectile.ai[1] += 1f;
                        int num17 = (int)projectile.ai[0];
                        if (!Main.player[num17].active || Main.player[num17].dead)
                        {
                            projectile.ai[1] = 1f;
                            projectile.ai[0] = 0f;
                            projectile.netUpdate = true;
                        }
                        else
                        {
                            float num18 = projectile.velocity.ToRotation();
                            Vector2 vector2 = Main.player[num17].Center - projectile.Center;
                            if (vector2.Length() < 20f)
                            {
                                projectile.Kill();
                                return false;
                            }

                            float targetAngle2 = vector2.ToRotation();
                            if (vector2 == Vector2.Zero)
                                targetAngle2 = num18;

                            float num19 = num18.AngleLerp(targetAngle2, 0.01f);
                            projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(num19);
                        }
                    }

                    if (projectile.ai[1] >= 1f && projectile.ai[1] < 20f)
                    {
                        projectile.ai[1] += 1f;
                        if (projectile.ai[1] == 20f)
                            projectile.ai[1] = 1f;
                    }

                    projectile.alpha -= 40;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    projectile.spriteDirection = projectile.direction;

                    projectile.frameCounter++;
                    if (projectile.frameCounter >= 3)
                    {
                        projectile.frame++;
                        projectile.frameCounter = 0;
                        if (projectile.frame >= 4)
                            projectile.frame = 0;
                    }

                    if (Main.rand.Next(4) == 0)
                    {
                        Vector2 value4 = -Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(11.25f)).RotatedBy(projectile.velocity.ToRotation());
                        Dust smoke = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100);
                        smoke.velocity *= 0.1f;
                        smoke.position = projectile.Center + value4 * projectile.width / 2f;
                        smoke.fadeIn = 0.9f;
                    }

                    if (Main.rand.Next(32) == 0)
                    {
                        Vector2 value5 = -Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(22.5f)).RotatedBy(projectile.velocity.ToRotation());
                        Dust smoke = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 155, default, 0.8f);
                        smoke.velocity *= 0.3f;
                        smoke.position = projectile.Center + value5 * projectile.width / 2f;
                        if (Main.rand.Next(2) == 0)
                            smoke.fadeIn = 1.4f;
                    }

                    if (Main.rand.Next(2) == 0)
                    {
                        Vector2 value6 = -Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(45f)).RotatedBy(projectile.velocity.ToRotation());
                        Dust shflame = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 0, default, 1.2f);
                        shflame.velocity *= 0.3f;
                        shflame.noGravity = true;
                        shflame.position = projectile.Center + value6 * projectile.width / 2f;
                        if (Main.rand.Next(2) == 0)
                            shflame.fadeIn = 1.4f;
                    }

                    return false;
                }
                else if (projectile.type == ProjectileID.CultistBossIceMist)
                {
                    if (projectile.localAI[1] == 0f)
                    {
                        projectile.localAI[1] = 1f;
                        SoundEngine.PlaySound(SoundID.Item120, projectile.position);
                    }

                    projectile.ai[0] += 1f;

                    // Main projectile
                    float duration = 300f;
                    if (projectile.ai[1] == 1f)
                    {
                        if (projectile.ai[0] >= duration - 20f)
                            projectile.alpha += 10;
                        else
                            projectile.alpha -= 10;

                        if (projectile.alpha < 0)
                            projectile.alpha = 0;
                        if (projectile.alpha > 255)
                            projectile.alpha = 255;

                        if (projectile.ai[0] >= duration)
                        {
                            projectile.Kill();
                            return false;
                        }

                        int num103 = Player.FindClosest(projectile.Center, 1, 1);
                        Vector2 vector11 = Main.player[num103].Center - projectile.Center;
                        float scaleFactor2 = projectile.velocity.Length();
                        vector11.Normalize();
                        vector11 *= scaleFactor2;
                        projectile.velocity = (projectile.velocity * 15f + vector11) / 16f;
                        projectile.velocity.Normalize();
                        projectile.velocity *= scaleFactor2;

                        if (projectile.ai[0] % 60f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vector50 = projectile.rotation.ToRotationVector2();
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, vector50, projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
                        }

                        projectile.rotation += MathHelper.Pi / 30f;

                        return false;
                    }

                    // Split projectiles
                    projectile.position -= projectile.velocity;

                    if (projectile.ai[0] >= duration - 260f)
                        projectile.alpha += 3;
                    else
                        projectile.alpha -= 40;

                    if (projectile.alpha < 0)
                        projectile.alpha = 0;
                    if (projectile.alpha > 255)
                        projectile.alpha = 255;

                    if (projectile.ai[0] >= duration - 255f)
                    {
                        projectile.Kill();
                        return false;
                    }

                    Vector2 value39 = new Vector2(0f, -720f).RotatedBy(projectile.velocity.ToRotation());
                    float scaleFactor3 = projectile.ai[0] % (duration - 255f) / (duration - 255f);
                    Vector2 spinningpoint13 = value39 * scaleFactor3;

                    for (int num724 = 0; num724 < 6; num724++)
                    {
                        Vector2 vector51 = projectile.Center + spinningpoint13.RotatedBy(num724 * MathHelper.TwoPi / 6f);
                        Dust ice = Dust.NewDustDirect(vector51 + Utils.RandomVector2(Main.rand, -8f, 8f) / 2f, 8, 8, 197, 0f, 0f, 100, Color.Transparent);
                        ice.noGravity = true;
                    }

                    return false;
                }
                else if (projectile.type == ProjectileID.CultistBossLightningOrbArc && !projectile.friendly)
                {
                    projectile.frameCounter++;
                    if (projectile.velocity == Vector2.Zero)
                    {
                        if (projectile.frameCounter >= projectile.extraUpdates * 2)
                        {
                            projectile.frameCounter = 0;
                            bool flag30 = true;
                            for (int num742 = 1; num742 < projectile.oldPos.Length; num742++)
                            {
                                if (projectile.oldPos[num742] != projectile.oldPos[0])
                                    flag30 = false;
                            }

                            if (flag30)
                            {
                                projectile.Kill();
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (projectile.frameCounter < projectile.extraUpdates * 2)
                            return false;

                        projectile.frameCounter = 0;
                        float num748 = projectile.velocity.Length();
                        UnifiedRandom unifiedRandom = new((int)projectile.ai[1]);
                        int num749 = 0;
                        Vector2 spinningpoint14 = -Vector2.UnitY;
                        while (true)
                        {
                            int num750 = unifiedRandom.Next();
                            projectile.ai[1] = num750;
                            num750 %= 100;
                            float f = (float)num750 / 100f * MathHelper.TwoPi;
                            Vector2 vector55 = f.ToRotationVector2();
                            if (vector55.Y > 0f)
                            {
                                vector55.Y *= -1f;
                            }

                            bool flag31 = false;
                            if (vector55.Y > -0.02f)
                            {
                                flag31 = true;
                            }
                            if (vector55.X * (float)(projectile.extraUpdates + 1) * 2f * num748 + projectile.localAI[0] > 40f)
                            {
                                flag31 = true;
                            }
                            if (vector55.X * (float)(projectile.extraUpdates + 1) * 2f * num748 + projectile.localAI[0] < -40f)
                            {
                                flag31 = true;
                            }

                            if (flag31)
                            {
                                if (num749++ >= 100)
                                {
                                    projectile.velocity = Vector2.Zero;
                                    projectile.localAI[1] = 1f;
                                    break;
                                }
                                continue;
                            }

                            spinningpoint14 = vector55;

                            break;
                        }

                        if (projectile.velocity != Vector2.Zero)
                        {
                            projectile.localAI[0] += spinningpoint14.X * (float)(projectile.extraUpdates + 1) * 2f * num748;
                            projectile.velocity = spinningpoint14.RotatedBy(projectile.ai[0] + MathHelper.Pi / 2f) * num748;
                            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi / 2f;
                        }
                    }

                    return false;
                }
            }

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
            {
                if (projectile.type == ProjectileID.DeerclopsIceSpike)
                {
                    int dustType = 16;
                    float dustVelocityMultiplier = 0.75f;
                    int numDust = 5;
                    int numDust2 = 5;
                    int fadeInTime = 10;
                    int fadeOutGateValue = 10;
                    float killGateValue = 20f;
                    int maxFrames = 5;

                    bool fadeIn = projectile.ai[0] < (float)fadeInTime;
                    bool fadeOut = projectile.ai[0] >= (float)fadeOutGateValue;
                    bool killProjectile = projectile.ai[0] >= killGateValue;
                    projectile.ai[0] += 1f;
                    if (projectile.localAI[0] == 0f)
                    {
                        projectile.localAI[0] = 1f;
                        projectile.rotation = projectile.velocity.ToRotation();
                        projectile.frame = Main.rand.Next(maxFrames);

                        for (int i = 0; i < numDust; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(24f, 24f), dustType, projectile.velocity * dustVelocityMultiplier * MathHelper.Lerp(0.2f, 0.7f, Main.rand.NextFloat()));
                            dust.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                            dust.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                        }

                        for (int j = 0; j < numDust2; j++)
                        {
                            Dust dust2 = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(24f, 24f), dustType, Main.rand.NextVector2Circular(2f, 2f) + projectile.velocity * dustVelocityMultiplier * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                            dust2.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                            dust2.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                            dust2.fadeIn = 1f;
                        }

                        SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, projectile.Center);
                    }

                    if (fadeIn)
                    {
                        projectile.Opacity += 0.1f;
                        projectile.scale = projectile.Opacity * projectile.ai[1];
                    }

                    if (fadeOut)
                        projectile.Opacity -= 0.2f;

                    if (killProjectile)
                        projectile.Kill();

                    return false;
                }

                // Override Deerclops rubble behavior to create a wave of rubble instead of it all flying up at the same time
                // Rubble doesn't deal damage if it's not moving
                else if (projectile.type == ProjectileID.DeerclopsRangedProjectile)
                {
                    projectile.ai[0] += 1f;

                    projectile.frame = (int)projectile.ai[1];

                    if (projectile.localAI[0] == 0f)
                    {
                        projectile.localAI[0] = 1f;
                        projectile.rotation = projectile.velocity.ToRotation();
                        for (int dustIndex = 0; dustIndex < 5; dustIndex++)
                        {
                            Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(24f, 24f), 16, projectile.velocity * MathHelper.Lerp(0.2f, 0.7f, Main.rand.NextFloat()));
                            dust.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                            dust.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                        }

                        for (int dustIndex = 0; dustIndex < 5; dustIndex++)
                        {
                            Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(24f, 24f), 16, Main.rand.NextVector2Circular(2f, 2f) + projectile.velocity * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                            dust.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                            dust.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                            dust.fadeIn = 1f;
                        }
                    }

                    if (projectile.ai[0] >= 5f + projectile.ai[2])
                        projectile.velocity.Y += 0.15f;

                    // Create a wave of rubble
                    // Make sure the projectile doesn't despawn before it starts going up
                    if (projectile.ai[0] <= projectile.ai[2])
                    {
                        projectile.timeLeft += 1;

                        // Use the expected velocity when the time is right
                        if (projectile.ai[0] == projectile.ai[2])
                        {
                            projectile.velocity *= 100f;
                            projectile.velocity *= 12f + Main.rand.NextFloat() * 2f;
                        }
                    }

                    return false;
                }

                else if (projectile.type == ProjectileID.DemonSickle)
                {
                    if (Main.wofNPCIndex < 0 || !Main.npc[Main.wofNPCIndex].active || Main.npc[Main.wofNPCIndex].life <= 0)
                        return true;

                    if (projectile.ai[0] == 0f)
                        SoundEngine.PlaySound(SoundID.Item8, projectile.position);

                    projectile.rotation += projectile.direction * 0.8f;

                    projectile.ai[0] += 1f;
                    if (projectile.velocity.Length() < projectile.ai[1])
                    {
                        if (projectile.ai[0] >= 30f)
                            projectile.velocity *= 1.06f;
                    }

                    Vector2 vector11 = Main.player[Main.npc[Main.wofNPCIndex].target].Center - projectile.Center;
                    if (vector11.Length() < 10f)
                    {
                        projectile.Kill();
                        return false;
                    }

                    if (projectile.ai[0] < 210f)
                    {
                        float scaleFactor2 = projectile.velocity.Length();
                        vector11.Normalize();
                        vector11 *= scaleFactor2;
                        projectile.velocity = (projectile.velocity * 30f + vector11) / 31f;
                        projectile.velocity.Normalize();
                        projectile.velocity *= scaleFactor2;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Dust shflame = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100);
                        shflame.noGravity = true;
                    }

                    return false;
                }

                else if (projectile.type == ProjectileID.EyeLaser && projectile.ai[0] == 1f)
                {
                    projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                    Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.3f / 255f, 0f, (255 - projectile.alpha) * 0.3f / 255f);

                    if (projectile.alpha > 0)
                        projectile.alpha -= 125;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    if (projectile.localAI[1] == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item33, projectile.position);
                        projectile.localAI[1] = 1f;
                    }

                    if (projectile.velocity.Length() < 12f)
                        projectile.velocity *= 1.0025f;

                    return false;
                }

                else if (projectile.type == ProjectileID.QueenSlimeGelAttack)
                {
                    // Phase 1 and 2 projectiles do not bounce and do not have gravity.
                    if (projectile.ai[1] == -2f)
                    {
                        if (projectile.alpha == 0 && Main.rand.Next(3) == 0)
                        {
                            Color newColor = NPC.AI_121_QueenSlime_GetDustColor();
                            newColor.A = 150;
                            int num72 = 8;
                            bool noGravity = Main.rand.NextBool();
                            Dust slime = Dust.NewDustDirect(projectile.position - new Vector2(num72, num72) + projectile.velocity, projectile.width + num72 * 2, projectile.height + num72 * 2, 4, 0f, 0f, 50, newColor, 1.2f);
                            slime.velocity *= 0.3f;
                            slime.velocity += projectile.velocity * 0.3f;
                            slime.noGravity = noGravity;
                        }

                        projectile.alpha -= 50;
                        if (projectile.alpha < 0)
                            projectile.alpha = 0;

                        projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.05f;

                        if (CalamityWorld.LegendaryMode && projectile.velocity.Length() > 2f)
                            projectile.velocity *= 0.985f;

                        return false;
                    }
                }

                else if (projectile.type == ProjectileID.QueenSlimeMinionBlueSpike)
                {
                    // When Queen Slime fires these they aren't as affected by gravity.
                    if (projectile.ai[1] < 0f)
                    {
                        if (projectile.frameCounter == 0)
                        {
                            projectile.frameCounter = 1;
                            projectile.frame = Main.rand.Next(3);
                        }

                        if (projectile.alpha == 0 && Main.rand.Next(3) == 0)
                        {
                            Color newColor = new Color(78, 136, 255, 150);
                            Dust slime = Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, 4, 0f, 0f, 50, newColor, 1.2f);
                            slime.velocity *= 0.3f;
                            slime.velocity += projectile.velocity * 0.3f;
                            slime.noGravity = true;
                        }

                        projectile.alpha -= 50;
                        if (projectile.alpha < 0)
                            projectile.alpha = 0;

                        if (projectile.ai[1] == -1f)
                        {
                            if (projectile.ai[0] >= 5f)
                                projectile.velocity.Y += 0.05f;
                            else
                                projectile.ai[0] += 1f;

                            if (projectile.velocity.Y > 16f)
                                projectile.velocity.Y = 16f;
                        }

                        projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                        return false;
                    }
                }

                else if (projectile.type == ProjectileID.DeathLaser && projectile.ai[0] == 1f)
                {
                    // Unlikely that vanilla sets originalDamage for hostile projectiles.
                    // TODO -- this might not work and mech boss projectiles might deal too much damage again.
                    if (projectile.originalDamage == 0)
                    {
                        // Reduce mech boss projectile damage depending on the new ore progression changes
                        if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                        {
                            if (!NPC.downedMechBossAny)
                                projectile.damage = (int)(projectile.damage * 0.8);
                            else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                                projectile.damage = (int)(projectile.damage * 0.9);
                        }

                        projectile.originalDamage = projectile.damage;
                    }

                    projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                    Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.75f / 255f, 0f, 0f);

                    if (projectile.alpha > 0)
                        projectile.alpha -= 125;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    if (projectile.localAI[1] == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item33, projectile.position);
                        projectile.localAI[1] = 1f;
                    }

                    if (projectile.velocity.Length() < 12f)
                        projectile.velocity *= 1.0025f;

                    return false;
                }

                else if (projectile.type == ProjectileID.RocketSkeleton && projectile.ai[1] == 1f)
                {
                    bool homeIn = false;
                    float spreadOutCutoffTime = 510f;
                    float homeInCutoffTime = 420f;
                    float minAcceleration = 0.05f;
                    float maxAcceleration = 0.1f;
                    float homingVelocity = 25f;

                    if (projectile.timeLeft > homeInCutoffTime && projectile.timeLeft <= spreadOutCutoffTime)
                        homeIn = true;
                    else if (projectile.velocity.Length() < 15f)
                        projectile.velocity *= 1.1f;

                    if (homeIn)
                    {
                        int playerIndex = (int)projectile.ai[0];
                        Vector2 velocity = projectile.velocity;
                        if (Main.player.IndexInRange(playerIndex))
                        {
                            Player player = Main.player[playerIndex];
                            velocity = projectile.DirectionTo(player.Center) * homingVelocity;
                        }

                        float amount = MathHelper.Lerp(minAcceleration, maxAcceleration, Utils.GetLerpValue(spreadOutCutoffTime, 30f, projectile.timeLeft, clamped: true));
                        projectile.velocity = Vector2.SmoothStep(projectile.velocity, velocity, amount);
                    }

                    if (projectile.timeLeft <= 3)
                    {
                        projectile.position.X += projectile.width / 2;
                        projectile.position.Y += projectile.height / 2;
                        projectile.width = 128;
                        projectile.height = 128;
                        projectile.position.X -= projectile.width / 2;
                        projectile.position.Y -= projectile.height / 2;
                    }

                    if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
                    {
                        projectile.tileCollide = false;
                        projectile.alpha = 255;
                    }
                    else
                    {
                        for (int n = 0; n < 2; n++)
                        {
                            float num22 = 0f;
                            float num23 = 0f;
                            if (n == 1)
                            {
                                num22 = projectile.velocity.X * 0.5f;
                                num23 = projectile.velocity.Y * 0.5f;
                            }

                            Dust fire = Dust.NewDustDirect(new Vector2(projectile.position.X + 3f + num22, projectile.position.Y + 3f + num23) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 6, 0f, 0f, 100);
                            fire.scale *= 2f + Main.rand.Next(10) * 0.1f;
                            fire.velocity *= 0.2f;
                            fire.noGravity = true;

                            Dust smoke = Dust.NewDustDirect(new Vector2(projectile.position.X + 3f + num22, projectile.position.Y + 3f + num23) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 31, 0f, 0f, 100, default(Color), 0.5f);
                            smoke.fadeIn = 1f + Main.rand.Next(5) * 0.1f;
                            smoke.velocity *= 0.05f;
                        }
                    }

                    if (projectile.velocity != Vector2.Zero)
                        projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                    return false;
                }

                else if (projectile.type == ProjectileID.SeedPlantera || projectile.type == ProjectileID.PoisonSeedPlantera)
                {
                    projectile.frameCounter++;
                    if (projectile.frameCounter > 1)
                    {
                        projectile.frameCounter = 0;
                        projectile.frame++;

                        if (projectile.frame > 1)
                            projectile.frame = 0;
                    }

                    if (projectile.ai[1] == 0f)
                    {
                        projectile.ai[1] = 1f;
                        SoundEngine.PlaySound(SoundID.Item17, projectile.position);
                    }

                    if (projectile.alpha > 0)
                        projectile.alpha -= 30;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    projectile.ai[0] += 1f;
                    if (projectile.ai[0] >= 120f)
                    {
                        if (projectile.velocity.Length() < 18f)
                            projectile.velocity *= 1.01f;
                    }

                    projectile.tileCollide = projectile.ai[0] >= 300f;
                    
                    if (projectile.timeLeft > 600)
                        projectile.timeLeft = 600;

                    projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                    return false;
                }

                else if (projectile.type == ProjectileID.ThornBall && !projectile.tileCollide)
                {
                    if (projectile.alpha > 0)
                    {
                        projectile.alpha -= 30;
                        if (projectile.alpha < 0)
                            projectile.alpha = 0;
                    }

                    Point point = projectile.Center.ToTileCoordinates();
                    Tile tileSafely = Framing.GetTileSafely(point);
                    bool stickOnCollision = tileSafely.HasUnactuatedTile && Main.tileSolid[tileSafely.TileType];
                    if (stickOnCollision)
                    {
                        projectile.velocity = Vector2.Zero;
                        projectile.ai[1] += 1f;
                        float explodeGateValue = 600f;
                        if (projectile.ai[1] >= explodeGateValue)
                        {
                            if (projectile.owner == Main.myPlayer)
                            {
                                int totalProjectiles = 8;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                int type = ModContent.ProjectileType<ThornBallSpike>();
                                float velocity = 1f;
                                Vector2 spinningPoint = new Vector2(0f, -velocity);
                                for (int k = 0; k < totalProjectiles; k++)
                                {
                                    Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center + Vector2.Normalize(velocity2) * 16f, velocity2, type, (int)Math.Round(projectile.damage * 0.8), 0f, Main.myPlayer);
                                }
                            }

                            SoundEngine.PlaySound(SoundID.Item17, projectile.position);

                            for (int i = 0; i < 8; i++)
                            {
                                int randomDustType = Main.rand.NextBool() ? 125 : 148;
                                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, randomDustType, 0f, 0f, 0, default, 2f);
                                dust.velocity *= 3f;
                                if (Main.rand.NextBool())
                                {
                                    dust.scale = 0.5f;
                                    dust.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                                }
                            }
                            for (int i = 0; i < 10; i++)
                            {
                                int randomDustType = Main.rand.NextBool() ? 125 : 148;
                                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, randomDustType, 0f, 0f, 0, default, 3f);
                                dust.noGravity = true;
                                dust.velocity *= 5f;
                                dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, randomDustType, 0f, 0f, 0, default, 2f);
                                dust.velocity *= 2f;
                            }

                            projectile.Kill();
                        }
                    }
                    else
                    {
                        int closestPlayer = Player.FindClosest(projectile.Center, 1, 1);
                        float homingSpeed = 7.5f + Vector2.Distance(Main.player[closestPlayer].Center, projectile.Center) * 0.01f;
                        Vector2 homingVelocity = Vector2.Normalize(Main.player[closestPlayer].Center - projectile.Center) * homingSpeed;
                        int inertia = 200;
                        projectile.velocity.X = (projectile.velocity.X * (inertia - 1) + homingVelocity.X) / inertia;

                        if (projectile.velocity.Length() > 16f)
                        {
                            projectile.velocity.Normalize();
                            projectile.velocity *= 16f;
                        }

                        projectile.ai[0] += 1f;
                        if (projectile.ai[0] > 15f)
                            projectile.velocity.Y += 0.1f;

                        projectile.rotation += projectile.velocity.X * 0.05f;

                        if (projectile.velocity.Y > 16f)
                            projectile.velocity.Y = 16f;
                    }

                    return false;
                }

                else if (projectile.type == ProjectileID.HallowBossRainbowStreak && projectile.hostile)
                {
                    bool spreadOut = false;
                    bool homeIn = false;
                    float spreadOutCutoffTime = 140f;
                    float homeInCutoffTime = Main.dayTime ? 55f : 80f;
                    float spreadDeceleration = 0.98f;
                    float minAcceleration = 0.05f;
                    float maxAcceleration = 0.1f;
                    float homingVelocity = 30f;

                    if (projectile.timeLeft > spreadOutCutoffTime)
                        spreadOut = true;
                    else if (projectile.timeLeft > homeInCutoffTime)
                        homeIn = true;

                    if (spreadOut)
                    {
                        float spreadVelocity = (float)Math.Cos(projectile.whoAmI % 6f / 6f + projectile.position.X / 320f + projectile.position.Y / 160f);
                        projectile.velocity *= spreadDeceleration;
                        projectile.velocity = projectile.velocity.RotatedBy(spreadVelocity * MathHelper.TwoPi * 0.125f * 1f / 30f);
                    }

                    if (homeIn)
                    {
                        int playerIndex = (int)projectile.ai[0];
                        Vector2 velocity = projectile.velocity;
                        if (Main.player.IndexInRange(playerIndex))
                        {
                            Player player = Main.player[playerIndex];
                            velocity = projectile.DirectionTo(player.Center) * homingVelocity;
                        }

                        float amount = MathHelper.Lerp(minAcceleration, maxAcceleration, Utils.GetLerpValue(spreadOutCutoffTime, 30f, projectile.timeLeft, clamped: true));
                        projectile.velocity = Vector2.SmoothStep(projectile.velocity, velocity, amount);
                    }

                    projectile.Opacity = Utils.GetLerpValue(240f, 220f, projectile.timeLeft, clamped: true);
                    projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi / 2f;

                    return false;
                }

                // Phase 1 sharknado
                else if (projectile.type == ProjectileID.SharknadoBolt)
                {
                    if (projectile.ai[1] < 0f)
                    {
                        float num623 = 0.209439516f;
                        float num624 = -2f;
                        float num625 = (float)(Math.Cos(num623 * projectile.ai[0]) - 0.5) * num624;

                        projectile.velocity.Y -= num625;

                        projectile.ai[0] += 1f;

                        num625 = (float)(Math.Cos(num623 * projectile.ai[0]) - 0.5) * num624;

                        projectile.velocity.Y += num625;

                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] > 10f)
                        {
                            projectile.alpha -= 5;
                            if (projectile.alpha < 100)
                                projectile.alpha = 100;

                            projectile.rotation += projectile.velocity.X * 0.1f;
                            projectile.frame = (int)(projectile.localAI[0] / 3f) % 3;
                        }

                        return false;
                    }
                }

                else if (projectile.type == ProjectileID.Sharknado)
                {
                    projectile.damage = projectile.GetProjectileDamage(NPCID.DukeFishron);
                }

                // Larger cthulhunadoes
                else if (projectile.type == ProjectileID.Cthulunado)
                {
                    projectile.damage = projectile.GetProjectileDamage(NPCID.DukeFishron);

                    int num606 = 16;
                    int num607 = 16;
                    float num608 = 2f;
                    int num609 = 150;
                    int num610 = 42;

                    if (projectile.velocity.X != 0f)
                        projectile.direction = projectile.spriteDirection = -Math.Sign(projectile.velocity.X);

                    int num3 = projectile.frameCounter;
                    projectile.frameCounter = num3 + 1;
                    if (projectile.frameCounter > 2)
                    {
                        num3 = projectile.frame;
                        projectile.frame = num3 + 1;
                        projectile.frameCounter = 0;
                    }
                    if (projectile.frame >= 6)
                        projectile.frame = 0;

                    if (projectile.localAI[0] == 0f && Main.myPlayer == projectile.owner)
                    {
                        projectile.localAI[0] = 1f;
                        projectile.position.X += projectile.width / 2;
                        projectile.position.Y += projectile.height / 2;
                        projectile.scale = (num606 + num607 - projectile.ai[1]) * num608 / (num607 + num606);
                        projectile.width = (int)(num609 * projectile.scale);
                        projectile.height = (int)(num610 * projectile.scale);
                        projectile.position.X -= projectile.width / 2;
                        projectile.position.Y -= projectile.height / 2;
                        projectile.netUpdate = true;
                    }

                    if (projectile.ai[1] != -1f)
                    {
                        projectile.scale = (num606 + num607 - projectile.ai[1]) * num608 / (num607 + num606);
                        projectile.width = (int)(num609 * projectile.scale);
                        projectile.height = (int)(num610 * projectile.scale);
                    }

                    if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    {
                        projectile.alpha -= 30;
                        if (projectile.alpha < 60)
                            projectile.alpha = 60;
                        if (projectile.alpha < 100)
                            projectile.alpha = 100;
                    }
                    else
                    {
                        projectile.alpha += 30;
                        if (projectile.alpha > 150)
                            projectile.alpha = 150;
                    }

                    if (projectile.ai[0] > 0f)
                        projectile.ai[0] -= 1f;

                    if (projectile.ai[0] == 1f && projectile.ai[1] > 0f && projectile.owner == Main.myPlayer)
                    {
                        projectile.netUpdate = true;

                        Vector2 center = projectile.Center;
                        center.Y -= num610 * projectile.scale / 2f;

                        float num611 = (num606 + num607 - projectile.ai[1] + 1f) * num608 / (num607 + num606);
                        center.Y -= num610 * num611 / 2f;
                        center.Y += 2f;

                        Projectile.NewProjectile(projectile.GetSource_FromThis(), center.X, center.Y, projectile.velocity.X, projectile.velocity.Y, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 10f, projectile.ai[1] - 1f);

                        if ((int)projectile.ai[1] % 3 == 0 && projectile.ai[1] != 0f)
                        {
                            int num614 = NPC.NewNPC(projectile.GetSource_FromAI(), (int)center.X, (int)center.Y, NPCID.Sharkron2);
                            Main.npc[num614].velocity = projectile.velocity;
                            Main.npc[num614].scale = 1.5f;
                            Main.npc[num614].netUpdate = true;
                            Main.npc[num614].ai[2] = projectile.width;
                            Main.npc[num614].ai[3] = -1.5f;
                        }
                    }

                    if (projectile.ai[0] <= 0f)
                    {
                        float num615 = 0.104719758f;
                        float num616 = projectile.width / 5f * 2.5f;
                        float num617 = (float)(Math.Cos(num615 * -(double)projectile.ai[0]) - 0.5) * num616;

                        projectile.position.X -= num617 * -projectile.direction;

                        projectile.ai[0] -= 1f;

                        num617 = (float)(Math.Cos(num615 * -(double)projectile.ai[0]) - 0.5) * num616;
                        projectile.position.X += num617 * -projectile.direction;
                    }

                    return false;
                }

                else if (projectile.type == ProjectileID.AncientDoomProjectile)
                {
                    if (projectile.velocity.Length() < 8f)
                        projectile.velocity *= 1.01f;
                }

                else if (projectile.type == ProjectileID.CultistBossLightningOrb && BossRushEvent.BossRushActive)
                {
                    if (NPC.AnyNPCs(NPCID.CultistBoss))
                    {
                        if (projectile.localAI[1] == 0f)
                        {
                            SoundEngine.PlaySound(SoundID.Item121, projectile.position);
                            projectile.localAI[1] = 1f;
                        }

                        if (projectile.ai[0] < 180f)
                        {
                            projectile.alpha -= 5;
                            if (projectile.alpha < 0)
                                projectile.alpha = 0;
                        }
                        else
                        {
                            projectile.alpha += 5;
                            if (projectile.alpha > 255)
                            {
                                projectile.alpha = 255;
                                projectile.Kill();
                                return false;
                            }
                        }

                        ref float reference = ref projectile.ai[0];
                        ref float reference46 = ref reference;
                        float num15 = reference;
                        reference46 = num15 + 1f;

                        if (projectile.ai[0] % 30f == 0f && projectile.ai[0] < 180f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int[] array6 = new int[5];
                            Vector2[] array7 = new Vector2[5];
                            int num731 = 0;
                            float num732 = 2000f;

                            for (int num733 = 0; num733 < 255; num733++)
                            {
                                if (!Main.player[num733].active || Main.player[num733].dead)
                                    continue;

                                Vector2 center9 = Main.player[num733].Center;
                                float num734 = Vector2.Distance(center9, projectile.Center);
                                if (num734 < num732 && Collision.CanHit(projectile.Center, 1, 1, center9, 1, 1))
                                {
                                    array6[num731] = num733;
                                    array7[num731] = center9;
                                    int num34 = num731 + 1;
                                    num731 = num34;
                                    if (num34 >= array7.Length)
                                        break;
                                }
                            }

                            for (int num735 = 0; num735 < num731; num735++)
                            {
                                Vector2 vector52 = array7[num735] + Main.player[array6[num735]].velocity * 40f - projectile.Center;
                                float ai = Main.rand.Next(100);
                                Vector2 vector53 = Vector2.Normalize(vector52.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, vector53, 466, projectile.damage, 0f, Main.myPlayer, vector52.ToRotation(), ai);
                            }
                        }

                        Lighting.AddLight(projectile.Center, 0.4f, 0.85f, 0.9f);

                        if (++projectile.frameCounter >= 4)
                        {
                            projectile.frameCounter = 0;
                            if (++projectile.frame >= Main.projFrames[projectile.type])
                                projectile.frame = 0;
                        }

                        if (projectile.alpha >= 150 || !(projectile.ai[0] < 180f))
                            return false;

                        for (int num736 = 0; num736 < 1; num736++)
                        {
                            float num737 = (float)Main.rand.NextDouble() * 1f - 0.5f;
                            if (num737 < -0.5f)
                                num737 = -0.5f;
                            if (num737 > 0.5f)
                                num737 = 0.5f;

                            Vector2 value40 = new Vector2(-projectile.width * 0.2f * projectile.scale, 0f).RotatedBy(num737 * MathHelper.TwoPi).RotatedBy(projectile.velocity.ToRotation());
                            Dust zap = Dust.NewDustDirect(projectile.Center - Vector2.One * 5f, 10, 10, 226, (0f - projectile.velocity.X) / 3f, (0f - projectile.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
                            zap.position = projectile.Center + value40;
                            zap.velocity = Vector2.Normalize(zap.position - projectile.Center) * 2f;
                            zap.noGravity = true;
                        }

                        for (int num739 = 0; num739 < 1; num739++)
                        {
                            float num740 = (float)Main.rand.NextDouble() * 1f - 0.5f;
                            if (num740 < -0.5f)
                                num740 = -0.5f;
                            if (num740 > 0.5f)
                                num740 = 0.5f;

                            Vector2 value41 = new Vector2(-projectile.width * 0.6f * projectile.scale, 0f).RotatedBy(num740 * MathHelper.TwoPi).RotatedBy(projectile.velocity.ToRotation());
                            Dust zap = Dust.NewDustDirect(projectile.Center - Vector2.One * 5f, 10, 10, 226, (0f - projectile.velocity.X) / 3f, (0f - projectile.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
                            zap.velocity = Vector2.Zero;
                            zap.position = projectile.Center + value41;
                            zap.noGravity = true;
                        }

                        return false;
                    }
                }

                else if (projectile.type == ProjectileID.CultistBossIceMist)
                {
                    if (projectile.localAI[1] == 0f)
                    {
                        projectile.localAI[1] = 1f;
                        SoundEngine.PlaySound(SoundID.Item120, projectile.position);
                    }

                    projectile.ai[0] += 1f;

                    // Main projectile
                    float duration = 300f;
                    if (projectile.ai[1] == 1f)
                    {
                        if (projectile.ai[0] >= duration - 20f)
                            projectile.alpha += 10;
                        else
                            projectile.alpha -= 10;

                        if (projectile.alpha < 0)
                            projectile.alpha = 0;
                        if (projectile.alpha > 255)
                            projectile.alpha = 255;

                        if (projectile.ai[0] >= duration)
                        {
                            projectile.Kill();
                            return false;
                        }

                        int num103 = Player.FindClosest(projectile.Center, 1, 1);
                        Vector2 vector11 = Main.player[num103].Center - projectile.Center;
                        float scaleFactor2 = projectile.velocity.Length();
                        vector11.Normalize();
                        vector11 *= scaleFactor2;
                        projectile.velocity = (projectile.velocity * 20f + vector11) / 21f;
                        projectile.velocity.Normalize();
                        projectile.velocity *= scaleFactor2;

                        if (projectile.ai[0] % 60f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vector50 = projectile.rotation.ToRotationVector2();
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, vector50, projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
                        }

                        projectile.rotation += MathHelper.Pi / 30f;

                        Lighting.AddLight(projectile.Center, 0.3f, 0.75f, 0.9f);

                        return false;
                    }

                    // Split projectiles
                    projectile.position -= projectile.velocity;

                    if (projectile.ai[0] >= duration - 260f)
                        projectile.alpha += 3;
                    else
                        projectile.alpha -= 40;

                    if (projectile.alpha < 0)
                        projectile.alpha = 0;
                    if (projectile.alpha > 255)
                        projectile.alpha = 255;

                    if (projectile.ai[0] >= duration - 255f)
                    {
                        projectile.Kill();
                        return false;
                    }

                    Vector2 value39 = new Vector2(0f, -720f).RotatedBy(projectile.velocity.ToRotation());
                    float scaleFactor3 = projectile.ai[0] % (duration - 255f) / (duration - 255f);
                    Vector2 spinningpoint13 = value39 * scaleFactor3;

                    for (int num724 = 0; num724 < 6; num724++)
                    {
                        Vector2 vector51 = projectile.Center + spinningpoint13.RotatedBy(num724 * MathHelper.TwoPi / 6f);

                        Lighting.AddLight(vector51, 0.3f, 0.75f, 0.9f);

                        for (int num725 = 0; num725 < 2; num725++)
                        {
                            Dust ice = Dust.NewDustDirect(vector51 + Utils.RandomVector2(Main.rand, -8f, 8f) / 2f, 8, 8, 197, 0f, 0f, 100, Color.Transparent);
                            ice.noGravity = true;
                        }
                    }

                    return false;
                }

                // Change the stupid homing eyes
                else if (projectile.type == ProjectileID.PhantasmalEye)
                {
                    projectile.alpha -= 40;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    if (projectile.ai[0] == 0f)
                    {
                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] >= 45f)
                        {
                            projectile.localAI[0] = 0f;
                            projectile.ai[0] = 1f;
                            projectile.ai[1] = 0f - projectile.ai[1];
                            projectile.netUpdate = true;
                        }

                        projectile.velocity.X = projectile.velocity.RotatedBy(projectile.ai[1]).X;
                        projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -6f, 6f);
                        projectile.velocity.Y -= 0.08f;

                        if (projectile.velocity.Y > 0f)
                            projectile.velocity.Y -= 0.2f;
                        if (projectile.velocity.Y < -7f)
                            projectile.velocity.Y = -7f;
                    }
                    else if (projectile.ai[0] == 1f)
                    {
                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] >= 90f)
                        {
                            projectile.localAI[0] = 0f;
                            projectile.ai[0] = 2f;
                            projectile.ai[1] = Player.FindClosest(projectile.position, projectile.width, projectile.height);
                            projectile.netUpdate = true;
                        }

                        projectile.velocity.X = projectile.velocity.RotatedBy(projectile.ai[1]).X;
                        projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -6f, 6f);
                        projectile.velocity.Y -= 0.08f;

                        if (projectile.velocity.Y > 0f)
                            projectile.velocity.Y -= 0.2f;
                        if (projectile.velocity.Y < -7f)
                            projectile.velocity.Y = -7f;
                    }
                    else if (projectile.ai[0] == 2f)
                    {
                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] >= 45f)
                        {
                            projectile.localAI[0] = 0f;
                            projectile.ai[0] = 3f;
                            projectile.netUpdate = true;
                        }

                        Vector2 value23 = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
                        value23.Normalize();
                        value23 *= 12f;
                        value23 = Vector2.Lerp(projectile.velocity, value23, 0.6f);

                        float num675 = 0.4f;
                        if (projectile.velocity.X < value23.X)
                        {
                            projectile.velocity.X += num675;
                            if (projectile.velocity.X < 0f && value23.X > 0f)
                                projectile.velocity.X += num675;
                        }
                        else if (projectile.velocity.X > value23.X)
                        {
                            projectile.velocity.X -= num675;
                            if (projectile.velocity.X > 0f && value23.X < 0f)
                                projectile.velocity.X -= num675;
                        }
                        if (projectile.velocity.Y < value23.Y)
                        {
                            projectile.velocity.Y += num675;
                            if (projectile.velocity.Y < 0f && value23.Y > 0f)
                                projectile.velocity.Y += num675;
                        }
                        else if (projectile.velocity.Y > value23.Y)
                        {
                            projectile.velocity.Y -= num675;
                            if (projectile.velocity.Y > 0f && value23.Y < 0f)
                                projectile.velocity.Y -= num675;
                        }
                    }
                    else if (projectile.ai[0] == 3f)
                    {
                        Vector2 value23 = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
                        if (value23.Length() < 30f)
                        {
                            projectile.Kill();
                            return false;
                        }

                        float velocityLimit = ((CalamityWorld.death || BossRushEvent.BossRushActive) ? 28f : 24f) / MathHelper.Clamp(lineColor * 0.75f, 1f, 3f);
                        if (projectile.velocity.Length() < velocityLimit)
                            projectile.velocity *= 1.01f;
                    }

                    if (projectile.alpha < 40)
                    {
                        Dust dust = Dust.NewDustDirect(projectile.Center - Vector2.One * 5f, 10, 10, 229, (0f - projectile.velocity.X) / 3f, (0f - projectile.velocity.Y) / 3f, 150, Color.Transparent, 1.2f);
                        dust.noGravity = true;
                    }

                    projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

                    return false;
                }

                // Moon Lord big eye spheres
                else if (projectile.type == ProjectileID.PhantasmalSphere && Main.npc[(int)projectile.ai[1]].type == NPCID.MoonLordHand)
                {
                    float velocityLimit = (CalamityWorld.death || BossRushEvent.BossRushActive) ? 14f : 12f;
                    if (projectile.velocity.Length() < velocityLimit)
                        projectile.velocity *= 1.0075f;

                    return true;
                }

                // Moon Lord leech tongue
                else if (projectile.type == ProjectileID.MoonLeech)
                {
                    Vector2 value35 = new Vector2(0f, 216f);
                    projectile.alpha -= 15;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    int num738 = (int)Math.Abs(projectile.ai[0]) - 1;
                    int num739 = (int)projectile.ai[1];
                    if (!Main.npc[num738].active || Main.npc[num738].type != NPCID.MoonLordHead)
                    {
                        projectile.Kill();
                        return false;
                    }

                    projectile.localAI[0]++;
                    if (projectile.localAI[0] >= 330f && projectile.ai[0] > 0f && Main.netMode != 1)
                    {
                        projectile.ai[0] *= -1f;
                        projectile.netUpdate = true;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && projectile.ai[0] > 0f && (!Main.player[(int)projectile.ai[1]].active || Main.player[(int)projectile.ai[1]].dead))
                    {
                        projectile.ai[0] *= -1f;
                        projectile.netUpdate = true;
                    }

                    projectile.rotation = (Main.npc[(int)Math.Abs(projectile.ai[0]) - 1].Center - Main.player[(int)projectile.ai[1]].Center + value35).ToRotation() + MathHelper.Pi / 2f;
                    if (projectile.ai[0] > 0f)
                    {
                        Vector2 value36 = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
                        if (value36.X != 0f || value36.Y != 0f)
                            projectile.velocity = Vector2.Normalize(value36) * Math.Min(32f, value36.Length());
                        else
                            projectile.velocity = Vector2.Zero;

                        if (value36.Length() < 40f && projectile.localAI[1] == 0f)
                        {
                            projectile.localAI[1] = 1f;
                            int timeToAdd = 840;
                            if (Main.expertMode)
                                timeToAdd = 960;

                            if (!Main.player[num739].creativeGodMode)
                                Main.player[num739].AddBuff(BuffID.MoonLeech, timeToAdd);
                        }
                    }
                    else
                    {
                        Vector2 value37 = Main.npc[(int)Math.Abs(projectile.ai[0]) - 1].Center - projectile.Center + value35;
                        if (value37.X != 0f || value37.Y != 0f)
                            projectile.velocity = Vector2.Normalize(value37) * Math.Min(32f, value37.Length());
                        else
                            projectile.velocity = Vector2.Zero;

                        if (value37.Length() < 40f)
                            projectile.Kill();
                    }

                    return false;
                }

                // Moon Lord Deathray
                else if (projectile.type == ProjectileID.PhantasmalDeathray)
                {
                    if (Main.npc[(int)projectile.ai[1]].type == NPCID.MoonLordHead)
                    {
                        Vector2? vector78 = null;

                        if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                            projectile.velocity = -Vector2.UnitY;

                        if (Main.npc[(int)projectile.ai[1]].active)
                        {
                            Vector2 value21 = new(27f, 59f);
                            Vector2 value22 = Utils.Vector2FromElipse(Main.npc[(int)projectile.ai[1]].localAI[0].ToRotationVector2(), value21 * Main.npc[(int)projectile.ai[1]].localAI[1]);
                            projectile.position = Main.npc[(int)projectile.ai[1]].Center + value22 - projectile.Size / 2f;
                        }

                        if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                            projectile.velocity = -Vector2.UnitY;

                        if (projectile.localAI[0] == 0f)
                            SoundEngine.PlaySound(SoundID.Zombie104, projectile.position);

                        float num801 = 1f;
                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] >= 180f)
                        {
                            projectile.Kill();
                            return false;
                        }

                        projectile.scale = (float)Math.Sin(projectile.localAI[0] * MathHelper.Pi / 180f) * 10f * num801;
                        if (projectile.scale > num801)
                            projectile.scale = num801;

                        float num804 = projectile.velocity.ToRotation();
                        num804 += projectile.ai[0];
                        projectile.rotation = num804 - MathHelper.PiOver2;
                        projectile.velocity = num804.ToRotationVector2();

                        float num805 = 3f;
                        float num806 = projectile.width;

                        Vector2 samplingPoint = projectile.Center;
                        if (vector78.HasValue)
                        {
                            samplingPoint = vector78.Value;
                        }

                        float[] array3 = new float[(int)num805];
                        Collision.LaserScan(samplingPoint, projectile.velocity, num806 * projectile.scale, 2400f, array3);
                        float num807 = 0f;
                        int num3;
                        for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
                        {
                            num807 += array3[num808];
                            num3 = num808;
                        }
                        num807 /= num805;

                        // Fire laser through walls at max length if target cannot be seen
                        if (!Collision.CanHitLine(Main.npc[(int)projectile.ai[1]].Center, 1, 1, Main.player[Main.npc[(int)projectile.ai[1]].target].Center, 1, 1) &&
                            Main.npc[(int)projectile.ai[1]].Calamity().newAI[0] == 1f)
                        {
                            num807 = 2400f;
                        }

                        float amount = 0.5f;
                        projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], num807, amount);

                        Vector2 vector79 = projectile.Center + projectile.velocity * (projectile.localAI[1] - 14f);
                        for (int num809 = 0; num809 < 2; num809 = num3 + 1)
                        {
                            float num810 = projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                            float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                            Vector2 vector80 = new((float)Math.Cos(num810) * num811, (float)Math.Sin(num810) * num811);
                            Dust dust = Dust.NewDustDirect(vector79, 0, 0, 229, vector80.X, vector80.Y);
                            dust.noGravity = true;
                            dust.scale = 1.7f;
                            num3 = num809;
                        }

                        if (Main.rand.Next(5) == 0)
                        {
                            Vector2 value29 = projectile.velocity.RotatedBy(MathHelper.PiOver2) * ((float)Main.rand.NextDouble() - 0.5f) * projectile.width;
                            Dust smoke = Dust.NewDustDirect(vector79 + value29 - Vector2.One * 4f, 8, 8, 31, 0f, 0f, 100, default, 1.5f);
                            smoke.velocity *= 0.5f;
                            smoke.velocity.Y = -Math.Abs(smoke.velocity.Y);
                        }

                        DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
                        Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, DelegateMethods.CastLight);

                        return false;
                    }
                }
            }

            return true;
        }
        #endregion

        #region AI
        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (!frameOneHacksExecuted)
            {
                if (projectile.hostile)
                {
                    // These projectiles are way too fucking fast so they need to be slower
                    if ((projectile.type == ProjectileID.QueenSlimeMinionBlueSpike && projectile.ai[1] >= 0f) || projectile.type == ProjectileID.QueenSlimeMinionPinkBall)
                        projectile.velocity *= 0.5f;

                    // Reduce Nail damage from Nailheads because they're stupid
                    if (projectile.type == ProjectileID.Nail && Main.expertMode)
                        projectile.damage /= 2;

                    if ((CalamityLists.hardModeNerfList.Contains(projectile.type) && Main.hardMode && !CalamityPlayer.areThereAnyDamnBosses && !Main.snowMoon) || projectile.type == ProjectileID.JavelinHostile)
                        projectile.damage = (int)(projectile.damage * 0.65);

                    // Reduce mech boss projectile damage depending on the new ore progression changes
                    if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                    {
                        if (!NPC.downedMechBossAny)
                        {
                            if (MechBossProjectileIDs.Contains(projectile.type))
                            {
                                if (CalamityUtils.AnyBossNPCS(true))
                                    projectile.damage = (int)(projectile.damage * 0.8);
                            }
                        }
                        else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                        {
                            if (MechBossProjectileIDs.Contains(projectile.type))
                            {
                                if (CalamityUtils.AnyBossNPCS(true))
                                    projectile.damage = (int)(projectile.damage * 0.9);
                            }
                        }
                    }
                }
                else
                {
                    if (modPlayer.deadshotBrooch && projectile.CountsAsClass<RangedDamageClass>() && player.heldProj != projectile.whoAmI)
                    {
                        if (projectile.type != ProjectileType<RicoshotCoin>())
                            projectile.extraUpdates += 1;
                        if (projectile.type == ProjectileID.MechanicalPiranha)
                        {
                            projectile.localNPCHitCooldown *= 2;
                            projectile.timeLeft *= 2;
                        }
                    }

                    if (modPlayer.camper && !player.StandingStill())
                        projectile.damage = (int)(projectile.damage * 0.1);

                    if (projectile.CountsAsClass<RogueDamageClass>() && stealthStrike)
                    {
                        int gloveArmorPenAmt = modPlayer.nanotech ? 15 : 8;
                        if (modPlayer.filthyGlove || modPlayer.bloodyGlove)
                            projectile.ArmorPenetration += gloveArmorPenAmt;
                    }
                }

                if (NPC.downedMoonlord)
                {
                    if (CalamityLists.dungeonProjectileBuffList.Contains(projectile.type))
                    {
                        // ai[1] being set to 1 is done only by the Calamity usages of these projectiles in Skeletron and Skeletron Prime boss fights
                        bool isSkeletronBossProjectile = (projectile.type == ProjectileID.RocketSkeleton || projectile.type == ProjectileID.Shadowflames) && projectile.ai[1] == 1f;

                        // These projectiles will not be buffed if Golem is alive
                        bool isGolemBossProjectile = NPC.golemBoss > 0 && (projectile.type == ProjectileID.InfernoHostileBolt || projectile.type == ProjectileID.InfernoHostileBlast);

                        if (!isSkeletronBossProjectile && !isGolemBossProjectile)
                            projectile.damage += 30;
                    }
                }

                if (DownedBossSystem.downedDoG && (Main.pumpkinMoon || Main.snowMoon || Main.eclipse))
                {
                    if (CalamityLists.eventProjectileBuffList.Contains(projectile.type))
                        projectile.damage += 15;
                }

                if (projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.Bee)
                {
                    if (projectile.timeLeft > 570) //all of these have a time left of 600 or 660
                    {
                        if (player.ActiveItem().type == ItemID.BeesKnees)
                            projectile.DamageType = DamageClass.Ranged;
                    }
                }
                else if (projectile.type == ProjectileID.SoulDrain)
                    projectile.DamageType = DamageClass.Magic;

                frameOneHacksExecuted = true;
            }

            // Setting this in SetDefaults didn't work
            switch (projectile.type)
            {
                // GFB traps, boulders and explosives go hard
                case ProjectileID.SpikyBallTrap:
                case ProjectileID.FlamethrowerTrap:
                case ProjectileID.SpearTrap:
                case ProjectileID.PoisonDartTrap:
                case ProjectileID.VenomDartTrap:
                case ProjectileID.Dynamite:
                case ProjectileID.BouncyDynamite:
                case ProjectileID.StickyDynamite:
                case ProjectileID.Bomb:
                case ProjectileID.BombFish:
                case ProjectileID.BouncyBomb:
                case ProjectileID.DirtBomb:
                case ProjectileID.DirtStickyBomb:
                case ProjectileID.DryBomb:
                case ProjectileID.HoneyBomb:
                case ProjectileID.LavaBomb:
                case ProjectileID.ScarabBomb:
                case ProjectileID.StickyBomb:
                case ProjectileID.WetBomb:
                case ProjectileID.Boulder:
                case ProjectileID.MiniBoulder:
                case ProjectileID.BouncyBoulder:
                case ProjectileID.LifeCrystalBoulder:
                case ProjectileID.MoonBoulder:
                    projectile.extraUpdates = Main.zenithWorld ? 1 : 0;
                    break;

                case ProjectileID.RockGolemRock:
                case ProjectileID.BloodShot:
                case ProjectileID.DandelionSeed:
                    projectile.extraUpdates = CalamityWorld.revenge ? 1 : 0;
                    break;

                case ProjectileID.Bee:
                case ProjectileID.Wasp:
                case ProjectileID.TinyEater:
                case ProjectileID.GiantBee:
                case ProjectileID.Bat:
                    projectile.extraUpdates = 1;
                    break;
            }

            // Jack O Lantern Launcher projectile tweak
            if (projectile.type == ProjectileID.JackOLantern)
            {
                if (projectile.ai[0] >= 20f)
                {
                    // Offset the gravity until 30 frames later
                    projectile.ai[2]++;
                    if (projectile.ai[2] < 30f)
                        projectile.velocity.Y -= 0.5f;
                }
            }

            // Random velocities for Bouncy Boulders in GFB
            if (projectile.type == ProjectileID.BouncyBoulder && Main.zenithWorld)
            {
                // 5% chance every frame to get a random velocity multiplier (this is actually rolled twice per frame, due to the extra update in GFB)
                if (Main.rand.Next(100) >= 95)
                    projectile.velocity *= Main.rand.NextFloat(0.9f, 1.25f);
            }

            // Accelerate for 1.5 seconds to full velocity
            if (projectile.type == ProjectileID.HallowBossLastingRainbow && (CalamityWorld.revenge || BossRushEvent.BossRushActive))
            {
                if (projectile.timeLeft > 570)
                    projectile.velocity *= 1.015525f;
            }

            if (projectile.type == ProjectileID.OrnamentFriendly && lineColor == 1) //spawned by Festive Wings
            {
                Vector2 center = projectile.Center;
                float maxDistance = 460f;
                bool homeIn = false;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(projectile, false))
                    {
                        float extraDistance = (float)(Main.npc[i].width / 2) + (Main.npc[i].height / 2);

                        bool canHit = Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1);

                        if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
                        {
                            center = Main.npc[i].Center;
                            homeIn = true;
                            break;
                        }
                    }
                }

                if (homeIn)
                {
                    Vector2 moveDirection = projectile.SafeDirectionTo(center, Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * 20f + moveDirection * 15f) / 21f;
                }
            }

            if (!projectile.npcProj && !projectile.trap && projectile.friendly && projectile.damage > 0)
            {
                if (modPlayer.fungalSymbiote && CalamityLists.MushroomProjectileIDs.Contains(projectile.type))
                {
                    if (Main.player[projectile.owner].miscCounter % 6 == 0 && projectile.FinalExtraUpdate())
                    {
                        if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileID.Mushroom] < 15)
                        {
                            //Note: these don't count as true melee anymore but its useful code to keep around
                            if (projectile.type == ProjectileType<NebulashFlail>() || projectile.type == ProjectileType<CosmicDischargeFlail>() ||
                                projectile.type == ProjectileType<MourningstarFlail>() || projectile.type == ProjectileID.SolarWhipSword)
                            {
                                Vector2 vector24 = Main.OffsetsPlayerOnhand[Main.player[projectile.owner].bodyFrame.Y / 56] * 2f;
                                if (player.direction != 1)
                                {
                                    vector24.X = player.bodyFrame.Width - vector24.X;
                                }
                                if (player.gravDir != 1f)
                                {
                                    vector24.Y = player.bodyFrame.Height - vector24.Y;
                                }
                                vector24 -= new Vector2(player.bodyFrame.Width - player.width, player.bodyFrame.Height - 42) / 2f;
                                Vector2 newCenter = player.RotatedRelativePoint(player.position + vector24, true) + projectile.velocity;
                                Projectile.NewProjectile(projectile.GetSource_FromThis(), newCenter, Vector2.Zero, ProjectileID.Mushroom, 0, 0f, projectile.owner);
                            }
                            else
                            {
                                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileID.Mushroom, 0, 0f, projectile.owner);
                            }
                        }
                    }
                }

                if (projectile.CountsAsClass<RogueDamageClass>())
                {
                    if (!LocketClone && !CannotProc)
                    {
                        if (modPlayer.nanotech)
                        {
                            if (Main.player[projectile.owner].miscCounter % 30 == 0 && projectile.FinalExtraUpdate())
                            {
                                if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileType<NanotechProjectile>()] < 5)
                                {
                                    int damage = (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(60);
                                    damage = player.ApplyArmorAccDamageBonusesTo(damage);
                                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<NanotechProjectile>(), damage, 0f, projectile.owner);
                                }
                            }
                        }
                        else if (modPlayer.moonCrown)
                        {
                            if (Main.player[projectile.owner].miscCounter % 120 == 0 && projectile.FinalExtraUpdate())
                            {
                                if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileType<MoonSigil>()] < 5)
                                {
                                    int damage = (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(42);
                                    damage = player.ApplyArmorAccDamageBonusesTo(damage);

                                    int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<MoonSigil>(), damage, 0f, projectile.owner);
                                    if (proj.WithinBounds(Main.maxProjectiles))
                                        Main.projectile[proj].DamageType = DamageClass.Generic;
                                }
                            }
                        }

                        if (modPlayer.dragonScales)
                        {
                            if (Main.player[projectile.owner].miscCounter % 50 == 0 && projectile.FinalExtraUpdate())
                            {
                                if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileType<DragonShit>()] < 5)
                                {
                                    int damage = (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(DragonScales.ShitBaseDamage);
                                    damage = player.ApplyArmorAccDamageBonusesTo(damage);

                                    int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 1.2f, ProjectileType<DragonShit>(), damage, 0f, projectile.owner);
                                    if (proj.WithinBounds(Main.maxProjectiles))
                                    {
                                        Main.projectile[proj].DamageType = DamageClass.Generic;
                                        Main.projectile[proj].ArmorPenetration = 10;
                                    }
                                }
                            }
                        }

                        if (modPlayer.daedalusSplit)
                        {
                            if (Main.player[projectile.owner].miscCounter % 30 == 0 && projectile.FinalExtraUpdate())
                            {
                                if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileID.CrystalShard] < 15)
                                {
                                    // Daedalus Rogue Crystals: 2 x 25%, soft cap starts at 120 base damage
                                    int crystalDamage = CalamityUtils.DamageSoftCap(projectile.damage * 0.25, 30);
                                    crystalDamage = player.ApplyArmorAccDamageBonusesTo(crystalDamage);

                                    for (int i = 0; i < 2; i++)
                                    {
                                        Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                                        int shard = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, velocity, ProjectileID.CrystalShard, crystalDamage, 0f, projectile.owner);
                                        if (shard.WithinBounds(Main.maxProjectiles))
                                            Main.projectile[shard].DamageType = DamageClass.Generic;
                                    }
                                }
                            }
                        }
                    }

                    if (player.meleeEnchant > 0 && !projectile.noEnchantments && !projectile.noEnchantmentVisuals)
                    {
                        switch (player.meleeEnchant)
                        {
                            case 1:
                                if (!Main.rand.NextBool(3))
                                {
                                    Dust venom = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 171, 0f, 0f, 100);
                                    venom.noGravity = true;
                                    venom.fadeIn = 1.5f;
                                    venom.velocity *= 0.25f;
                                }
                                break;
                            case 2:
                                if (Main.rand.NextBool())
                                {
                                    Dust cflame = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 75, projectile.velocity.X * 0.2f + (projectile.direction * 3), projectile.velocity.Y * 0.2f, 100, new Color(), 2.5f);
                                    cflame.noGravity = true;
                                    cflame.velocity *= 0.7f;
                                    cflame.velocity.Y -= 0.5f;
                                }
                                break;
                            case 3:
                                if (Main.rand.NextBool())
                                {
                                    Dust fire = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 6, projectile.velocity.X * 0.2f + (projectile.direction * 3), projectile.velocity.Y * 0.2f, 100, new Color(), 2.5f);
                                    fire.noGravity = true;
                                    fire.velocity *= 0.7f;
                                    fire.velocity.Y -= 0.5f;
                                }
                                break;
                            case 4:
                                if (Main.rand.NextBool())
                                {
                                    Dust gold = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 57, projectile.velocity.X * 0.2f + (projectile.direction * 3), projectile.velocity.Y * 0.2f, 100, new Color(), 1.1f);
                                    gold.noGravity = true;
                                    gold.velocity *= 0.5f;
                                }
                                break;
                            case 5:
                                if (Main.rand.NextBool())
                                {
                                    Dust ichor = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 169, 0f, 0f, 100);
                                    ichor.velocity.X += projectile.direction;
                                    ichor.velocity.Y += 0.2f;
                                    ichor.noGravity = true;
                                }
                                break;
                            case 6:
                                if (Main.rand.NextBool())
                                {
                                    Dust nanite = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 135, 0f, 0f, 100);
                                    nanite.velocity.X += projectile.direction;
                                    nanite.velocity.Y += 0.2f;
                                    nanite.noGravity = true;
                                }
                                break;
                            case 8:
                                if (Main.rand.NextBool(4))
                                {
                                    Dust poison = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 46, 0f, 0f, 100);
                                    poison.noGravity = true;
                                    poison.fadeIn = 1.5f;
                                    poison.velocity *= 0.25f;
                                }
                                break;
                            case CalamityGlobalBuff.ModdedFlaskEnchant:
                                int dustType = player.Calamity().flaskHoly ? (Main.rand.NextBool() ? 87 : (int)CalamityDusts.ProfanedFire) : player.Calamity().flaskBrimstone ? (Main.rand.NextBool() ? 114 : ModContent.DustType<BrimstoneFlame>()) : (Main.rand.NextBool() ? 121 : DustID.Stone);
                                if (Main.rand.NextBool(4))
                                {
                                    Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, Main.rand.NextFloat(0.6f, 0.9f));
                                    dust.noGravity = dust.type == 121 ? false : true;
                                    if (!player.Calamity().flaskHoly)
                                        dust.fadeIn = 1f;
                                    dust.velocity = player.Calamity().flaskHoly && Main.rand.NextBool(3) ? new Vector2(Main.rand.NextFloat(-0.9f, 0.9f), Main.rand.NextFloat(-6.6f, -9.8f)) : dust.type == 121 ? new Vector2(Main.rand.NextFloat(-0.7f, 0.7f), Main.rand.NextFloat(0.6f, 1.8f)) : -projectile.velocity * 0.2f;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (projectile.CountsAsClass<MeleeDamageClass>() || projectile.CountsAsClass<SummonMeleeSpeedDamageClass>())
                {
                    if ((player.Calamity().flaskBrimstone || player.Calamity().flaskCrumbling || player.Calamity().flaskHoly) && !projectile.noEnchantments && !projectile.noEnchantmentVisuals)
                    {
                        int dustType = player.Calamity().flaskHoly ? (Main.rand.NextBool() ? 87 : (int)CalamityDusts.ProfanedFire) : player.Calamity().flaskBrimstone ? (Main.rand.NextBool() ? 114 : ModContent.DustType<BrimstoneFlame>()) : (Main.rand.NextBool() ? 121 : DustID.Stone);
                        if (Main.rand.NextBool(4))
                        {
                            Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, Main.rand.NextFloat(0.6f, 0.9f));
                            dust.noGravity = dust.type == 121 ? false : true;
                            if (!player.Calamity().flaskHoly)
                                dust.fadeIn = 1f;
                            dust.velocity = player.Calamity().flaskHoly && Main.rand.NextBool(3) ? new Vector2(Main.rand.NextFloat(-0.9f, 0.9f), Main.rand.NextFloat(-6.6f, -9.8f)) : dust.type == 121 ? new Vector2(Main.rand.NextFloat(-0.7f, 0.7f), Main.rand.NextFloat(0.6f, 1.8f)) : -projectile.velocity * 0.2f;
                        }
                    }
                }

                if (modPlayer.theBee && projectile.owner == Main.myPlayer && projectile.damage > 0)
                {
                    bool lifeAndShieldCondition = player.statLife >= player.statLifeMax2 && (!modPlayer.HasAnyEnergyShield || modPlayer.TotalEnergyShielding >= modPlayer.TotalMaxShieldDurability);
                    if (lifeAndShieldCondition && Main.rand.NextBool(5))
                    {
                        Dust dust = Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, 91, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 0.5f);
                        dust.noGravity = true;
                    }
                }

                if (!projectile.CountsAsClass<MeleeDamageClass>() && player.meleeEnchant == 7 && !projectile.noEnchantmentVisuals) //flask of party affects all types of weapons
                {
                    Vector2 velocity = projectile.velocity;
                    if (velocity.Length() > 4.0)
                        velocity *= 4f / velocity.Length();
                    if (Main.rand.NextBool(20))
                    {
                        Dust confetti = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, Main.rand.Next(139, 143), velocity.X, velocity.Y, 0, new Color(), 1.2f);
                        confetti.scale *= 1f + Main.rand.Next(-20, 21) * 0.01f;
                        confetti.velocity.X *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        confetti.velocity.Y *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        confetti.velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                        confetti.velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                    }
                    if (Main.rand.NextBool(40) && Main.netMode != NetmodeID.Server)
                    {
                        int Type = Main.rand.Next(276, 283);
                        Gore confetti = Gore.NewGoreDirect(projectile.GetSource_FromAI(), projectile.position, velocity, Type, 1f);
                        confetti.scale *= 1f + Main.rand.Next(-20, 21) * 0.01f;
                        confetti.velocity.X *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        confetti.velocity.Y *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        confetti.velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                        confetti.velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                    }
                }

                if (allProjectilesHome)
                {
                    CalamityUtils.HomeInOnNPC(projectile, !projectile.tileCollide, 300f, 12f, 20f);
                }
            }
        }
        #endregion

        #region Post AI
        public override void PostAI(Projectile projectile)
        {
            if (projectile.FinalExtraUpdate() && flatDRTimer > 0)
            {
                flatDRTimer--;
                if (flatDRTimer <= 0)
                    flatDR = 0;
            }

            // optimization to remove conversion X/Y loop for irrelevant projectiles
            bool isConversionProjectile = projectile.type == ProjectileID.PurificationPowder
                || projectile.type == ProjectileID.VilePowder
                || projectile.type == ProjectileID.ViciousPowder
                || projectile.type == ProjectileID.PureSpray
                || projectile.type == ProjectileID.CorruptSpray
                || projectile.type == ProjectileID.CrimsonSpray
                || projectile.type == ProjectileID.HallowSpray;
            if (!isConversionProjectile)
                return;

            if (projectile.owner == Main.myPlayer/* && Main.netMode != NetmodeID.MultiplayerClient*/)
            {
                int x = (int)(projectile.Center.X / 16f);
                int y = (int)(projectile.Center.Y / 16f);

                bool isPowder = projectile.type == ProjectileID.PurificationPowder || projectile.type == ProjectileID.VilePowder || projectile.type == ProjectileID.ViciousPowder;

                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (projectile.type == ProjectileID.PureSpray || projectile.type == ProjectileID.PurificationPowder)
                        {
                            AstralBiome.ConvertFromAstral(i, j, ConvertType.Pure, !isPowder);
                        }
                        if (projectile.type == ProjectileID.CorruptSpray || projectile.type == ProjectileID.VilePowder)
                        {
                            AstralBiome.ConvertFromAstral(i, j, ConvertType.Corrupt, !isPowder);
                        }
                        if (projectile.type == ProjectileID.CrimsonSpray || projectile.type == ProjectileID.ViciousPowder)
                        {
                            AstralBiome.ConvertFromAstral(i, j, ConvertType.Crimson, !isPowder);
                        }
                        if (projectile.type == ProjectileID.HallowSpray)
                        {
                            AstralBiome.ConvertFromAstral(i, j, ConvertType.Hallow);
                        }
                        NetMessage.SendTileSquare(-1, i, j, 1, 1);
                    }
                }
            }
        }
        #endregion

        #region Grappling Hooks
        public override void GrapplePullSpeed(Projectile projectile, Player player, ref float speed)
        {
            if (player.Calamity().reaverSpeed)
                speed *= 1.1f;
        }

        public override void GrappleRetreatSpeed(Projectile projectile, Player player, ref float speed)
        {
            if (player.Calamity().reaverSpeed)
                speed *= 1.1f;
        }
        #endregion

        #region Modify Hit NPC
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            // Old Fashioned damage boost
            if (modPlayer.oldFashioned)
            {
                // Yoyo bullshit
                if (player.counterWeight > 0)
                {
                    if (projectile.type >= ProjectileID.BlackCounterweight && projectile.type <= ProjectileID.YellowCounterweight)
                        modifiers.SourceDamage *= OldFashioned.AccessoryAndSetBonusDamageMultiplier;
                }

                // Honey Balloon, Bee Cloak, Honey Comb, Stinger Necklace, Sweetheart Necklace
                if (player.honeyCombItem != null && !player.honeyCombItem.IsAir)
                {
                    if (projectile.type == ProjectileID.Bee)
                        modifiers.SourceDamage *= OldFashioned.AccessoryAndSetBonusDamageMultiplier;
                }

                // Star Cloak, Mana Cloak, Star Veil, Bee Cloak
                if (player.starCloakItem != null && !player.starCloakItem.IsAir)
                {
                    if (projectile.type == ProjectileID.BeeCloakStar || projectile.type == ProjectileID.ManaCloakStar || projectile.type == ProjectileID.StarCloakStar)
                        modifiers.SourceDamage *= OldFashioned.AccessoryAndSetBonusDamageMultiplier;
                }

                // Hive Pack
                if (player.strongBees)
                {
                    if (projectile.type == ProjectileID.GiantBee)
                        modifiers.SourceDamage *= OldFashioned.AccessoryAndSetBonusDamageMultiplier;
                }

                // Bone Glove
                if (player.boneGloveItem != null && !player.boneGloveItem.IsAir)
                {
                    if (projectile.type == ProjectileID.BoneGloveProj)
                        modifiers.SourceDamage *= OldFashioned.AccessoryAndSetBonusDamageMultiplier;
                }

                // Bone Helm
                if (player.HasItem(ItemID.BoneHelm))
                {
                    if (projectile.type == ProjectileID.InsanityShadowFriendly)
                        modifiers.SourceDamage *= OldFashioned.AccessoryAndSetBonusDamageMultiplier;
                }

                // Volatile Gelatin
                if (player.volatileGelatin)
                {
                    if (projectile.type == ProjectileID.VolatileGelatinBall)
                        modifiers.SourceDamage *= OldFashioned.AccessoryAndSetBonusDamageMultiplier;
                }

                // Spore Sac
                if (player.sporeSac)
                {
                    if (projectile.type == ProjectileID.SporeTrap || projectile.type == ProjectileID.SporeTrap2 ||
                        projectile.type == ProjectileID.SporeGas || projectile.type == ProjectileID.SporeGas2 ||
                        projectile.type == ProjectileID.SporeGas3)
                        modifiers.SourceDamage *= OldFashioned.AccessoryAndSetBonusDamageMultiplier;
                }

                // Spectre Mask bonus
                if (player.ghostHurt)
                {
                    if (projectile.type == ProjectileID.SpectreWrath)
                        modifiers.SourceDamage *= OldFashioned.AccessoryAndSetBonusDamageMultiplier;
                }

                // Orichalcum Armor bonus
                if (player.onHitPetal)
                {
                    if (projectile.type == ProjectileID.FlowerPetal)
                        modifiers.SourceDamage *= OldFashioned.AccessoryAndSetBonusDamageMultiplier;
                }

                // Titanium Armor bonus
                if (player.onHitTitaniumStorm)
                {
                    if (projectile.type == ProjectileID.TitaniumStormShard)
                        modifiers.SourceDamage *= OldFashioned.AccessoryAndSetBonusDamageMultiplier;
                }

                // Forbidden Armor bonus
                if (player.setForbidden)
                {
                    if (projectile.type == ProjectileID.SandnadoFriendly)
                        modifiers.SourceDamage *= OldFashioned.AccessoryAndSetBonusDamageMultiplier;
                }

                // Stardust Armor bonus
                if (player.setStardust)
                {
                    if (projectile.type == ProjectileID.StardustGuardianExplosion || projectile.type == ProjectileID.StardustPunch)
                        modifiers.SourceDamage *= OldFashioned.AccessoryAndSetBonusDamageMultiplier;
                }
            }

            // The vanilla damage Jousting Lance multiplier is as follows. Calamity overrides this with a new formula.
            // damageScale = 0.1f + player.velocity.Length() / 7f * 0.9f
            if (projectile.type == ProjectileID.JoustingLance || projectile.type == ProjectileID.HallowJoustingLance || projectile.type == ProjectileID.ShadowJoustingLance)
            {
                float baseVelocityDamageMultiplier = 0.01f + player.velocity.Length() * 0.002f;
                float calamityVelocityDamageMultiplier = 100f * (1f - (1f / (1f + baseVelocityDamageMultiplier)));
                modifiers.SourceDamage *= calamityVelocityDamageMultiplier;
            }

            // If applicable, use ricoshot bonus damage.
            if (totalRicoshotDamageBonus > 0f)
                modifiers.ScalingBonusDamage += totalRicoshotDamageBonus;

            // If this projectile is forced to crit, simply set the crit bool.
            if (forcedCrit)
                modifiers.SetCrit();

            if (modPlayer.rottenDogTooth && projectile.Calamity().stealthStrike)
                target.AddBuff(BuffType<Crumbling>(), RottenDogtooth.ArmorCrunchDebuffTime);

            if (modPlayer.flamingItemEnchant && !projectile.minion && !projectile.npcProj && !projectile.Calamity().CreatedByPlayerDash)
                target.AddBuff(BuffType<VulnerabilityHex>(), VulnerabilityHex.AflameDuration);

            if (modPlayer.farProximityRewardEnchant)
            {
                float proximityDamageInterpolant = Utils.GetLerpValue(250f, 2400f, target.Distance(player.Center), true);
                float proximityDamageFactor = MathHelper.SmoothStep(0.7f, 1.45f, proximityDamageInterpolant);
                modifiers.SourceDamage *= proximityDamageFactor;
            }

            if (modPlayer.closeProximityRewardEnchant)
            {
                float proximityDamageInterpolant = Utils.GetLerpValue(400f, 175f, target.Distance(player.Center), true);
                float proximityDamageFactor = MathHelper.SmoothStep(0.75f, 1.75f, proximityDamageInterpolant);
                modifiers.SourceDamage *= proximityDamageFactor;
            }

            // Aerial Bane does 50% damage to "airborne" enemies. This is just simple math to revert that as it is a very unbalanced mechanic.
            if (projectile.type == ProjectileID.DD2BetsyArrow)
            {
                if (!WorldUtils.Find(projectile.Center.ToTileCoordinates(), Searches.Chain(new Searches.Down(12), new Conditions.IsSolid()), out _))
                    modifiers.SourceDamage /= 1.5f;
            }

            // create sparks on hit to hammer in the defense shredding 
            if (deepcoreBullet)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Vector2 cen = Vector2.Lerp(projectile.Center, target.Center, 0.65f);
                    int numSparks = Main.rand.Next(2, 5);
                    for (int i = 0; i < numSparks; i++)
                    {
                        Vector2 sparkVelocity = projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.6f) * Main.rand.NextFloat(2f, 8f);
                        Color sparkColor = Color.Lerp(Color.AliceBlue, Color.DarkSlateBlue, Main.rand.NextFloat(0.7f));
                        sparkColor = Color.Lerp(sparkColor, Color.CornflowerBlue, Main.rand.NextFloat());
                        SparkParticle sparke = new(cen, -sparkVelocity, false, 10, 0.5f, sparkColor);
                        GeneralParticleHandler.SpawnParticle(sparke);
                    }
                }
            }
        }
        #endregion

        #region Modify Hit Player
        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage.Flat -= flatDR;
        }
        #endregion

        #region Can Damage + Can Hit
        public override bool? CanDamage(Projectile projectile)
        {
            if (projectile.hostile && (projectile.damage - flatDR <= 0))
                return false;

            switch (projectile.type)
            {
                // Rev+ Deerclops ice spikes can only deal damage while they're not fading out
                case ProjectileID.DeerclopsIceSpike:
                    if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
                        return projectile.ai[0] < 10f;
                    break;

                // Rev+ Deerclops rubble doesn't deal damage while it's not flying upwards
                case ProjectileID.DeerclopsRangedProjectile:
                    if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
                        return projectile.ai[0] > projectile.ai[2];
                    break;

                // Storm Weaver frost waves don't deal damage unless they're at their max velocity
                case ProjectileID.FrostWave:
                    if (projectile.ai[1] > 0f)
                        return projectile.velocity.Length() >= projectile.ai[1];
                    break;

                // Duke Fishron tornadoes don't deal damage for a bit after they spawn
                case ProjectileID.Sharknado:
                    if (projectile.timeLeft > 420)
                        return false;
                    break;

                case ProjectileID.Cthulunado:
                    if (projectile.timeLeft > 720)
                        return false;
                    break;

                default:
                    break;
            }
            return null;
        }

        // Cultist lightning orbs cannot hit players specifically. This could probably be switched to CanDamage?
        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            if (projectile.type == ProjectileID.CultistBossLightningOrb)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Drawing
        public override Color? GetAlpha(Projectile projectile, Color lightColor)
        {
            if (Main.player[Main.myPlayer].Calamity().trippy)
                return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, projectile.alpha);

            if (Main.LocalPlayer.Calamity().omniscience && projectile.hostile && projectile.damage > 0 && projectile.alpha < 255)
            {
                if (projectile.ModProjectile is null || (projectile.ModProjectile != null && projectile.ModProjectile.CanHitPlayer(Main.LocalPlayer) && (projectile.ModProjectile.CanDamage() ?? true)))
                    return Color.Coral;
            }

            if (projectile.type == ProjectileID.Stinger)
                return new Color(200, 200, 0, projectile.alpha);

            if (projectile.type == ProjectileID.QueenBeeStinger)
                return new Color(250, 250, 0, projectile.alpha);

            if (projectile.type == ProjectileID.QueenSlimeGelAttack || projectile.type == ProjectileID.QueenSlimeMinionBlueSpike || projectile.type == ProjectileID.QueenSlimeMinionPinkBall)
                return new Color(255, 255, 255, projectile.alpha);

            if (projectile.type == ProjectileID.PinkLaser)
            {
                if (projectile.alpha < 200)
                    return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);

                return Color.Transparent;
            }

            if (projectile.ai[1] > 0f && projectile.type == ProjectileID.FrostWave)
            {
                if (projectile.velocity.Length() < projectile.ai[1])
                {
                    float minVelocity = projectile.ai[1] * 0.5f;
                    float velocityRatio = (projectile.velocity.Length() - minVelocity) / minVelocity;
                    byte b2 = (byte)(velocityRatio * 200);
                    byte a2 = (byte)(b2 / 200f * 255f);
                    return new Color(b2, b2, b2, a2);
                }
                return new Color(200, 200, 200, projectile.alpha);
            }

            if (projectile.type == ProjectileID.ThornBall)
            {
                float startTurningBrownGateValue = 420f;
                float timeToReachFullBrown = 180f;
                float timeToReachThornExplosion = startTurningBrownGateValue + timeToReachFullBrown;
                Color initialColor = Color.White;
                initialColor.A = (byte)projectile.alpha;
                Color finalColor = Color.RosyBrown;
                finalColor.A = (byte)projectile.alpha;
                if (projectile.ai[1] > startTurningBrownGateValue)
                {
                    float colorTransitionRatio = (projectile.ai[1] - startTurningBrownGateValue) / timeToReachFullBrown;
                    Color dehydratedColor = Color.Lerp(initialColor, finalColor, colorTransitionRatio);
                    return dehydratedColor;
                }
                else
                    return initialColor;
            }

            if (projectile.type == ProjectileID.SeedPlantera || projectile.type == ProjectileID.PoisonSeedPlantera ||
                projectile.type == ProjectileID.CultistBossFireBallClone || projectile.type == ProjectileID.AncientDoomProjectile)
            {
                if (projectile.timeLeft < 85)
                {
                    byte b2 = (byte)(projectile.timeLeft * 3);
                    byte a2 = (byte)(projectile.alpha * (b2 / 255f));
                    return new Color(b2, b2, b2, a2);
                }
                return new Color(255, 255, 255, projectile.alpha);
            }

            return null;
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            #region Vanilla Summons Drawing Changes

            //
            // MINION AI CHANGES:
            //

            if (projectile.type == ProjectileID.Raven)
                return RavenMinionAI.DoRavenMinionDrawing(projectile, ref lightColor);

            #endregion

            // Chlorophyte Crystal AI rework.
            if (projectile.type == ProjectileID.CrystalLeaf)
                return ChlorophyteCrystalAI.DoChlorophyteCrystalDrawing(projectile);

            if (Main.player[Main.myPlayer].Calamity().trippy)
            {
                Texture2D texture = TextureAssets.Projectile[projectile.type].Value;

                SpriteEffects spriteEffects = SpriteEffects.None;
                if (projectile.spriteDirection == -1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                Vector2 vector11 = new(texture.Width / 2, texture.Height / Main.projFrames[projectile.type] / 2);
                Color color9 = new(Main.DiscoR, Main.DiscoG, Main.DiscoB, projectile.alpha);
                Color alpha15 = projectile.GetAlpha(color9);

                float offsetX = Main.screenPosition.X + (projectile.width / 2) - texture.Width * projectile.scale / 2f + vector11.X * projectile.scale;
                float offsetY = Main.screenPosition.Y + projectile.height - texture.Height * projectile.scale / Main.projFrames[projectile.type] + 4f + vector11.Y * projectile.scale + projectile.gfxOffY;
                for (int num213 = 0; num213 < 4; num213++)
                {
                    Vector2 position9 = projectile.position;
                    float num214 = Math.Abs(projectile.Center.X - Main.player[Main.myPlayer].Center.X);
                    float num215 = Math.Abs(projectile.Center.Y - Main.player[Main.myPlayer].Center.Y);

                    if (num213 == 0 || num213 == 2)
                        position9.X = Main.player[Main.myPlayer].Center.X + num214;
                    else
                        position9.X = Main.player[Main.myPlayer].Center.X - num214;

                    position9.X -= projectile.width / 2;

                    if (num213 == 0 || num213 == 1)
                        position9.Y = Main.player[Main.myPlayer].Center.Y + num215;
                    else
                        position9.Y = Main.player[Main.myPlayer].Center.Y - num215;

                    int frames = texture.Height / Main.projFrames[projectile.type];
                    int y = frames * projectile.frame;
                    position9.Y -= projectile.height / 2;

                    Main.spriteBatch.Draw(texture,
                        new Vector2(position9.X - offsetX, position9.Y - offsetY),
                        new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y, texture.Width, frames)), alpha15, projectile.rotation, vector11, projectile.scale, spriteEffects, 0);
                }
            }

            if (Main.zenithWorld)
            {
                if (NPC.AnyNPCs(NPCType<NPCs.CeaselessVoid.CeaselessVoid>()))
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                    var midnightShader = Terraria.Graphics.Shaders.GameShaders.Armor.GetShaderFromItemId(ItemID.MidnightRainbowDye);
                    midnightShader.Apply();
                }
            }

            return true;
        }
        #endregion

        #region Kill
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.owner == Main.myPlayer && !projectile.npcProj && !projectile.trap)
            {
                if (projectile.CountsAsClass<RogueDamageClass>())
                {
                    if (modPlayer.etherealExtorter && extorterBoost && Main.player[projectile.owner].ownedProjectileCounts[ProjectileType<LostSoulFriendly>()] < 5)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);

                            int damage = (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(20);
                            damage = player.ApplyArmorAccDamageBonusesTo(damage);

                            int soul = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, velocity, ProjectileType<LostSoulFriendly>(), damage, 0f, projectile.owner);
                            Main.projectile[soul].tileCollide = false;
                            if (soul.WithinBounds(Main.maxProjectiles))
                                Main.projectile[soul].DamageType = DamageClass.Generic;
                        }
                    }

                    if (modPlayer.scuttlersJewel && stealthStrike && modPlayer.scuttlerCooldown <= 0)
                    {
                        int damage = (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(18);
                        damage = player.ApplyArmorAccDamageBonusesTo(damage);

                        int spike = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<JewelSpike>(), damage, projectile.knockBack, projectile.owner);
                        Main.projectile[spike].frame = 4;
                        if (spike.WithinBounds(Main.maxProjectiles))
                            Main.projectile[spike].DamageType = DamageClass.Generic;
                        modPlayer.scuttlerCooldown = 30;
                    }
                }

                if (projectile.type == ProjectileID.UnholyWater)
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<WaterConvertor>(), 0, 0f, projectile.owner, 1f);

                if (projectile.type == ProjectileID.BloodWater)
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<WaterConvertor>(), 0, 0f, projectile.owner, 2f);

                if (projectile.type == ProjectileID.HolyWater)
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<WaterConvertor>(), 0, 0f, projectile.owner, 3f);
            }
        }
        #endregion

        #region Life Steal
        public static bool CanSpawnLifeStealProjectile(float healMultiplier, float healAmount)
        {
            if (healMultiplier <= 0f || (int)healAmount <= 0)
                return false;

            return true;
        }

        public static void SpawnLifeStealProjectile(Projectile projectile, Player player, float healAmount, int healProjectileType, float distanceRequired, float cooldownMultiplier)
        {
            if (Main.player[Main.myPlayer].moonLeech)
                return;

            Main.player[Main.myPlayer].lifeSteal -= healAmount * cooldownMultiplier;

            float lowestHealthCheck = 0f;
            int healTarget = projectile.owner;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player otherPlayer = Main.player[i];
                if (otherPlayer.active && !otherPlayer.dead && ((!player.hostile && !otherPlayer.hostile) || player.team == otherPlayer.team))
                {
                    float playerDist = Vector2.Distance(projectile.Center, otherPlayer.Center);
                    if (playerDist < distanceRequired && (otherPlayer.statLifeMax2 - otherPlayer.statLife) > lowestHealthCheck)
                    {
                        lowestHealthCheck = otherPlayer.statLifeMax2 - otherPlayer.statLife;
                        healTarget = i;
                    }
                }
            }

            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, healProjectileType, 0, 0f, projectile.owner, healTarget, healAmount);
        }
        #endregion
    }
}
