using CalamityMod.Buffs;
using CalamityMod.Items;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class CalamityGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        // Class Types
        public bool rogue = false;
        public bool trueMelee = false;

        // Force Class Types
        public bool forceMelee = false;
        public bool forceRanged = false;
        public bool forceMagic = false;
        public bool forceRogue = false;
        public bool forceMinion = false;
        public bool forceHostile = false;

        // Damage Adjusters
        private bool setDamageValues = true;
        public float spawnedPlayerMinionDamageValue = 1f;
        public int spawnedPlayerMinionProjectileDamageValue = 0;
        public int defDamage = 0;

        // Rogue Stuff
        public bool stealthStrike = false; //Update all existing rogue weapons with this
        public bool momentumCapacitatorBoost = false; //Constant acceleration

        // Iron Heart
        public int ironHeartDamage = 0;

        // Counters and Timers
        private int counter = 0;
        private int counter2 = 0;

        #region SetDefaults
        public override void SetDefaults(Projectile projectile)
        {
            switch (projectile.type)
            {
                case ProjectileID.Spear:
                case ProjectileID.ChainKnife:
                case ProjectileID.TheRottedFork:
                case ProjectileID.BallOHurt:
                case ProjectileID.TheMeatball:
                case ProjectileID.Swordfish:
                case ProjectileID.Arkhalis:
                case ProjectileID.BlueMoon:
                case ProjectileID.DarkLance:
                case ProjectileID.Sunfury:
                case ProjectileID.Anchor:
                case ProjectileID.BoxingGlove:
                case ProjectileID.CobaltNaginata:
                case ProjectileID.PalladiumPike:
                case ProjectileID.MythrilHalberd:
                case ProjectileID.OrichalcumHalberd:
                case ProjectileID.AdamantiteGlaive:
                case ProjectileID.TitaniumTrident:
                case ProjectileID.TheDaoofPow:
                case ProjectileID.GolemFist:
                case ProjectileID.Gungnir:
                case ProjectileID.ObsidianSwordfish:
                case ProjectileID.ChainGuillotine:
                case ProjectileID.MonkStaffT3:
                case ProjectileID.MonkStaffT1:
                case ProjectileID.SolarWhipSword:
                    trueMelee = true;
                    break;
                case ProjectileID.StarWrath:
                    projectile.penetrate = projectile.maxPenetrate = 1;
                    break;
                default:
                    break;
            }

            // Disable Lunatic Cultist's homing resistance globally
            ProjectileID.Sets.Homing[projectile.type] = false;
        }
        #endregion

        #region PreAI
        public override bool PreAI(Projectile projectile)
        {
            if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
            {
                if (projectile.type == ProjectileID.PoisonSeedPlantera)
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
                        Main.PlaySound(SoundID.Item17, projectile.position);
                    }

                    if (projectile.alpha > 0)
                        projectile.alpha -= 30;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    projectile.ai[0] += 1f;
                    if (projectile.ai[0] >= 35f)
                    {
                        projectile.ai[0] = 35f;
                        projectile.velocity.Y += 0.01f;
                    }

                    projectile.tileCollide = false;

                    if (projectile.timeLeft > 300)
                        projectile.timeLeft = 300;

                    projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;

                    if (projectile.velocity.Y > 16f)
                        projectile.velocity.Y = 16f;

                    return false;
                }

                // Phase 1 sharknado
                else if (projectile.type == ProjectileID.SharknadoBolt)
                {
                    if (projectile.ai[1] < 0f)
                    {
                        float num623 = 0.209439516f;
                        float num624 = -2f;
                        float num625 = (float)(Math.Cos((double)(num623 * projectile.ai[0])) - 0.5) * num624;

                        projectile.velocity.Y -= num625;

                        projectile.ai[0] += 1f;

                        num625 = (float)(Math.Cos((double)(num623 * projectile.ai[0])) - 0.5) * num624;

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

                // Large cthulhunadoes
                else if (projectile.type == ProjectileID.Cthulunado)
                {
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
                        projectile.position.X += (float)(projectile.width / 2);
                        projectile.position.Y += (float)(projectile.height / 2);
                        projectile.scale = ((float)(num606 + num607) - projectile.ai[1]) * num608 / (float)(num607 + num606);
                        projectile.width = (int)((float)num609 * projectile.scale);
                        projectile.height = (int)((float)num610 * projectile.scale);
                        projectile.position.X -= (float)(projectile.width / 2);
                        projectile.position.Y -= (float)(projectile.height / 2);
                        projectile.netUpdate = true;
                    }

                    if (projectile.ai[1] != -1f)
                    {
                        projectile.scale = ((float)(num606 + num607) - projectile.ai[1]) * num608 / (float)(num607 + num606);
                        projectile.width = (int)((float)num609 * projectile.scale);
                        projectile.height = (int)((float)num610 * projectile.scale);
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
                        center.Y -= (float)num610 * projectile.scale / 2f;

                        float num611 = ((float)(num606 + num607) - projectile.ai[1] + 1f) * num608 / (float)(num607 + num606);
                        center.Y -= (float)num610 * num611 / 2f;
                        center.Y += 2f;

                        Projectile.NewProjectile(center.X, center.Y, projectile.velocity.X, projectile.velocity.Y, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 10f, projectile.ai[1] - 1f);

                        if ((int)projectile.ai[1] % 3 == 0 && projectile.ai[1] != 0f)
                        {
                            int num614 = NPC.NewNPC((int)center.X, (int)center.Y, NPCID.Sharkron2, 0, 0f, 0f, 0f, 0f, 255);
                            Main.npc[num614].velocity = projectile.velocity;
                            Main.npc[num614].scale = 1.5f;
                            Main.npc[num614].netUpdate = true;
                            Main.npc[num614].ai[2] = (float)projectile.width;
                            Main.npc[num614].ai[3] = -1.5f;
                        }
                    }

                    if (projectile.ai[0] <= 0f)
                    {
                        float num615 = 0.104719758f;
                        float num616 = (float)projectile.width / 5f * 2.5f;
                        float num617 = (float)(Math.Cos((double)(num615 * -(double)projectile.ai[0])) - 0.5) * num616;

                        projectile.position.X -= num617 * (float)-(float)projectile.direction;

                        projectile.ai[0] -= 1f;

                        num617 = (float)(Math.Cos((double)(num615 * -(double)projectile.ai[0])) - 0.5) * num616;
                        projectile.position.X += num617 * (float)-(float)projectile.direction;
                    }
                    return false;
                }

                // Moon Lord Deathray
                else if (projectile.type == ProjectileID.PhantasmalDeathray)
                {
                    Vector2? vector78 = null;

                    if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                    {
                        projectile.velocity = -Vector2.UnitY;
                    }

                    if (Main.npc[(int)projectile.ai[1]].active && Main.npc[(int)projectile.ai[1]].type == NPCID.MoonLordHead)
                    {
                        Vector2 value21 = new Vector2(27f, 59f);
                        Vector2 value22 = Utils.Vector2FromElipse(Main.npc[(int)projectile.ai[1]].localAI[0].ToRotationVector2(), value21 * Main.npc[(int)projectile.ai[1]].localAI[1]);
                        projectile.position = Main.npc[(int)projectile.ai[1]].Center + value22 - new Vector2((float)projectile.width, (float)projectile.height) / 2f;
                    }

                    if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                    {
                        projectile.velocity = -Vector2.UnitY;
                    }

                    if (projectile.localAI[0] == 0f)
                    {
                        Main.PlaySound(29, (int)projectile.position.X, (int)projectile.position.Y, 104, 1f, 0f);
                    }

                    float num801 = 1f;
                    projectile.localAI[0] += 1f;
                    if (projectile.localAI[0] >= 180f)
                    {
                        projectile.Kill();
                        return false;
                    }

                    projectile.scale = (float)Math.Sin((double)(projectile.localAI[0] * 3.14159274f / 180f)) * 10f * num801;
                    if (projectile.scale > num801)
                    {
                        projectile.scale = num801;
                    }

                    float num804 = projectile.velocity.ToRotation();
                    num804 += projectile.ai[0];
                    projectile.rotation = num804 - 1.57079637f;
                    projectile.velocity = num804.ToRotationVector2();

                    float num805 = 3f;
                    float num806 = (float)projectile.width;

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
                        float num810 = projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
                        float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                        Vector2 vector80 = new Vector2((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                        int num812 = Dust.NewDust(vector79, 0, 0, 229, vector80.X, vector80.Y, 0, default, 1f);
                        Main.dust[num812].noGravity = true;
                        Main.dust[num812].scale = 1.7f;
                        num3 = num809;
                    }
                    if (Main.rand.Next(5) == 0)
                    {
                        Vector2 value29 = projectile.velocity.RotatedBy(1.5707963705062866, default) * ((float)Main.rand.NextDouble() - 0.5f) * (float)projectile.width;
                        int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, 31, 0f, 0f, 100, default, 1.5f);
                        Dust dust = Main.dust[num813];
                        dust.velocity *= 0.5f;
                        Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
                    }
                    DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
                    Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], (float)projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

                    return false;
                }
            }
            return true;
        }
        #endregion

        #region AI
        public override void AI(Projectile projectile)
        {
            if (defDamage == 0)
                defDamage = projectile.damage;

            if (NPC.downedMoonlord)
            {
                if (CalamityMod.dungeonProjectileBuffList.Contains(projectile.type))
                {
                    if ((projectile.type == ProjectileID.RocketSkeleton && projectile.ai[1] == 1f) ||
                        (NPC.golemBoss > 0 && (projectile.type == ProjectileID.InfernoHostileBolt || projectile.type == ProjectileID.InfernoHostileBlast)))
                    {
                        projectile.damage = defDamage;
                    }
                    else
                        projectile.damage = defDamage + 60;
                }
            }

            if (CalamityWorld.downedDoG && (Main.pumpkinMoon || Main.snowMoon))
            {
                if (CalamityMod.eventProjectileBuffList.Contains(projectile.type))
                    projectile.damage = defDamage + 90;
            }
            else if (CalamityWorld.buffedEclipse && Main.eclipse)
            {
                if (CalamityMod.eventProjectileBuffList.Contains(projectile.type))
                    projectile.damage = defDamage + 120;
            }

            // Iron Heart damage variable will scale with projectile.damage
            if (CalamityWorld.ironHeart)
            {
                ironHeartDamage = 0;
            }

            if (projectile.modProjectile != null && projectile.modProjectile.mod.Name.Equals("CalamityMod"))
                goto SKIP_CALAMITY;

            if ((projectile.minion || projectile.sentry) && !ProjectileID.Sets.StardustDragon[projectile.type]) //For all other mods and vanilla, exclude dragon due to bugs
            {
                if (setDamageValues)
                {
                    spawnedPlayerMinionDamageValue = Main.player[projectile.owner].minionDamage;
                    spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                    setDamageValues = false;
                }
                if (Main.player[projectile.owner].minionDamage != spawnedPlayerMinionDamageValue)
                {
                    int damage2 = (int)((float)spawnedPlayerMinionProjectileDamageValue / spawnedPlayerMinionDamageValue * Main.player[projectile.owner].minionDamage);
                    projectile.damage = damage2;
                }
            }
            SKIP_CALAMITY:

            if (forceMelee)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = true;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = false;
                rogue = false;
            }
            else if (forceRanged)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = true;
                projectile.magic = false;
                projectile.minion = false;
                rogue = false;
            }
            else if (forceMagic)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = true;
                projectile.minion = false;
                rogue = false;
            }
            else if (forceRogue)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = false;
                rogue = true;
            }
            else if (forceMinion)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = true;
                rogue = false;
            }
            else if (forceHostile)
            {
                projectile.hostile = true;
                projectile.friendly = false;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = false;
                rogue = false;
            }

            if (projectile.type == ProjectileID.NettleBurstRight || projectile.type == ProjectileID.NettleBurstLeft || projectile.type == ProjectileID.NettleBurstEnd)
            {
                if (Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == ModContent.ItemType<ThornBlossom>())
                    projectile.penetrate = 1;
            }
            else if (projectile.type == ProjectileID.VilethornBase || projectile.type == ProjectileID.VilethornTip)
            {
                if (Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == ModContent.ItemType<FeralthornClaymore>())
                {
                    projectile.melee = true;
                    projectile.magic = false;
                    projectile.penetrate = 1;
                }
            }
            else if (projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.Bee)
            {
                if (projectile.timeLeft > 570) //all of these have a time left of 600 or 660
                {
                    if (Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == ItemID.BeesKnees)
                    {
                        projectile.magic = false;
                        projectile.ranged = true;
                    }
                }
            }
            else if (projectile.type == ProjectileID.FallingStar && Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == ItemID.StarCannon)
                projectile.ranged = true;
            else if (projectile.type == ProjectileID.SoulDrain)
                projectile.magic = true;

            if (Main.player[projectile.owner].Calamity().eQuiver && projectile.ranged &&
                projectile.friendly && CalamityMod.rangedProjectileExceptionList.TrueForAll(x => projectile.type != x))
            {
                if (Main.rand.Next(200) > 198)
                {
                    float spread = 180f * 0.0174f;
                    double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                    if (projectile.owner == Main.myPlayer)
                    {
                        int projectile1 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(startAngle) * 8f), (float)(Math.Cos(startAngle) * 8f), projectile.type, (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
                        int projectile2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(startAngle) * 8f), (float)(-Math.Cos(startAngle) * 8f), projectile.type, (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
                        Main.projectile[projectile1].ranged = false;
                        Main.projectile[projectile2].ranged = false;
                        Main.projectile[projectile1].timeLeft = 60;
                        Main.projectile[projectile2].timeLeft = 60;
                        Main.projectile[projectile1].noDropItem = true;
                        Main.projectile[projectile2].noDropItem = true;
                    }
                }
            }

            counter++;
            if (Main.player[projectile.owner].Calamity().fungalSymbiote && trueMelee)
            {
                if (counter % 6 == 0)
                {
                    if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[ProjectileID.Mushroom] < 30)
                    {
                        if (projectile.type == ModContent.ProjectileType<Nebulash>() || projectile.type == ModContent.ProjectileType<CosmicDischarge>() ||
                            projectile.type == ModContent.ProjectileType<Mourningstar>() || projectile.type == ProjectileID.SolarWhipSword)
                        {
                            Vector2 vector24 = Main.OffsetsPlayerOnhand[Main.player[projectile.owner].bodyFrame.Y / 56] * 2f;
                            if (Main.player[projectile.owner].direction != 1)
                            {
                                vector24.X = (float)Main.player[projectile.owner].bodyFrame.Width - vector24.X;
                            }
                            if (Main.player[projectile.owner].gravDir != 1f)
                            {
                                vector24.Y = (float)Main.player[projectile.owner].bodyFrame.Height - vector24.Y;
                            }
                            vector24 -= new Vector2((float)(Main.player[projectile.owner].bodyFrame.Width - Main.player[projectile.owner].width), (float)(Main.player[projectile.owner].bodyFrame.Height - 42)) / 2f;
                            Vector2 newCenter = Main.player[projectile.owner].RotatedRelativePoint(Main.player[projectile.owner].position + vector24, true) + projectile.velocity;
                            Projectile.NewProjectile(newCenter.X, newCenter.Y, 0f, 0f, ProjectileID.Mushroom,
                                (int)((double)projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
                        }
                        else
                        {
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ProjectileID.Mushroom,
                                (int)((double)projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
                        }
                    }
                }
            }

            if (Main.player[projectile.owner].Calamity().nanotech && rogue && projectile.friendly && projectile.type != ModContent.ProjectileType<MoonSigil>() && projectile.type != ModContent.ProjectileType<DragonShit>())
            {
                if (counter % 30 == 0)
                {
                    if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<Nanotech>()] < 30)
                    {
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<Nanotech>(),
                            (int)((double)projectile.damage * 0.15), 0f, projectile.owner, 0f, 0f);
                    }
                }
            }
            if (Main.player[projectile.owner].Calamity().dragonScales && rogue && projectile.friendly && projectile.type != ModContent.ProjectileType<Nanotech>() && projectile.type != ModContent.ProjectileType<DragonShit>())
            {
                if (counter % 50 == 0)
                {
                    if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<DragonShit>()] < 15)
                    {
                        //spawn a dust that does 1/5th of the original damage
                        int projectileID = Projectile.NewProjectile(projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<DragonShit>(),
                            (int)((double)projectile.damage * 0.2), 0f, projectile.owner, 0f, 0f);
                    }
                }
            }

            if (Main.player[projectile.owner].Calamity().daedalusSplit && rogue && projectile.friendly)
            {
                counter2++;
                if (counter2 >= 30)
                {
                    counter2 = 0;
                    if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[90] < 30)
                    {
                        for (int num252 = 0; num252 < 2; num252++)
                        {
                            Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                            while (value15.X == 0f && value15.Y == 0f)
                            {
                                value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                            }
                            value15.Normalize();
                            value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                            int shard = Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), value15.X, value15.Y, 90, (int)((double)projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
                            Main.projectile[shard].ranged = false;
                        }
                    }
                }
            }
            //will always be friendly and rogue if it has this boost
            if (Main.player[projectile.owner].Calamity().momentumCapacitor && projectile.Calamity().momentumCapacitatorBoost)
            {
                if (projectile.type != ModContent.ProjectileType<Malachite>() && projectile.type != ModContent.ProjectileType<DuneHopperProjectile>() && 
                    projectile.velocity.Length() < 3f)
                    projectile.velocity *= 1.025f;
            }

            if (Main.player[projectile.owner].Calamity().theBeeDamage > 0 && projectile.owner == Main.myPlayer && projectile.friendly && projectile.damage > 0 &&
                (projectile.melee || projectile.ranged || projectile.magic || rogue))
            {
                int dust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 91, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 0.5f);
                Main.dust[dust].noGravity = true;
            }

            if (Main.player[projectile.owner].Calamity().providenceLore && projectile.owner == Main.myPlayer && projectile.friendly && projectile.damage > 0 &&
                (projectile.melee || projectile.ranged || projectile.magic || rogue))
            {
                int dust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 0.5f);
                Main.dust[dust].noGravity = true;
            }

            if (rogue)
            {
                if (Main.player[projectile.owner].Calamity().moonCrown)
                {
                    //Summon moon sigils infrequently
                    if (Main.rand.NextBool(300))
                    {
                        Projectile.NewProjectile(projectile.position, Vector2.Zero, ModContent.ProjectileType<MoonSigil>(), (int)((double)projectile.damage * 0.05), 0);
                    }
                }
            }
        }
        #endregion

        #region PostAI
        public override void PostAI(Projectile projectile)
        {
            int x = (int)(projectile.Center.X / 16f);
            int y = (int)(projectile.Center.Y / 16f);
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (projectile.type == ProjectileID.PureSpray)
                    {
                        WorldGenerationMethods.ConvertFromAstral(i, j, ConvertType.Pure);
                    }
                    if (projectile.type == ProjectileID.CorruptSpray)
                    {
                        WorldGenerationMethods.ConvertFromAstral(i, j, ConvertType.Corrupt);
                    }
                    if (projectile.type == ProjectileID.CrimsonSpray)
                    {
                        WorldGenerationMethods.ConvertFromAstral(i, j, ConvertType.Crimson);
                    }
                    if (projectile.type == ProjectileID.HallowSpray)
                    {
                        WorldGenerationMethods.ConvertFromAstral(i, j, ConvertType.Hallow);
                    }
                }
            }
        }
        #endregion

        #region OnHitNPC
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                if (projectile.Calamity().rogue && projectile.Calamity().stealthStrike && Main.player[projectile.owner].Calamity().dragonScales && CalamityUtils.CountProjectiles(ModContent.ProjectileType<InfernadoFriendly>()) < 2)
                {
                    int projTileX = (int)(projectile.Center.X / 16f);
                    int projTileY = (int)(projectile.Center.Y / 16f);
                    int distance = 100;
                    if (projTileX < 10)
                    {
                        projTileX = 10;
                    }
                    if (projTileX > Main.maxTilesX - 10)
                    {
                        projTileX = Main.maxTilesX - 10;
                    }
                    if (projTileY < 10)
                    {
                        projTileY = 10;
                    }
                    if (projTileY > Main.maxTilesY - distance - 10)
                    {
                        projTileY = Main.maxTilesY - distance - 10;
                    }
                    for (int x = projTileX; x < projTileX + distance; x++)
                    {
                        Tile tile = Main.tile[projTileX, projTileY];
                        if (tile.active() && (Main.tileSolid[(int)tile.type] || tile.liquid != 0))
                        {
                            projTileX = x;
                            break;
                        }
                    }
                    int projectileIndex = Projectile.NewProjectile((float)(projTileX * 16 + 8), (float)(projTileY * 16 - 24), 0f, 0f, ModContent.ProjectileType<InfernadoFriendly>(), 420, 15f, Main.myPlayer, 16f, 16f);
                    Main.projectile[projectileIndex].Calamity().forceRogue = true;
                    Main.projectile[projectileIndex].netUpdate = true;
                }
                if (projectile.magic && Main.player[projectile.owner].ghostHeal)
                {
                    float num = 0.1f;
                    num -= (float)projectile.numHits * 0.05f;
                    if (num < 0f)
                    {
                        num = 0f;
                    }
                    float num2 = (float)damage * num;
                    Main.player[Main.myPlayer].lifeSteal -= num2;
                }

                if (projectile.type == ProjectileID.VampireKnife)
                {
                    float num = (float)damage * 0.0375f;
                    if (num < 0f)
                    {
                        num = 0f;
                    }
                    Main.player[Main.myPlayer].lifeSteal -= num;
                }

                if (Main.player[projectile.owner].Calamity().alchFlask &&
                    (projectile.magic || rogue || projectile.melee || projectile.minion || projectile.ranged || projectile.sentry || CalamityMod.projectileMinionList.Contains(projectile.type)) &&
                    Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<PlagueSeeker>()] < 6)
                {
                    int newDamage = (int)((double)projectile.damage * 0.25);
                    if (newDamage > 30)
                    {
                        newDamage = 30;
                    }
                    int plague = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<PlagueSeeker>(), newDamage, 0f, projectile.owner, 0f, 0f);
                    Main.projectile[plague].melee = false;
                }

                if (Main.player[projectile.owner].Calamity().reaverBlast && projectile.melee)
                {
                    int newDamage = (int)((double)projectile.damage * 0.2);
                    if (newDamage > 30)
                    {
                        newDamage = 30;
                    }
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<ReaverBlast>(), newDamage, 0f, projectile.owner, 0f, 0f);
                }

                if (Main.player[projectile.owner].Calamity().auricSet && target.canGhostHeal)
                {
                    float num11 = 0.05f;
                    num11 -= (float)projectile.numHits * 0.025f;
                    if (num11 <= 0f)
                    {
                        return;
                    }
                    float num12 = (float)projectile.damage * num11;
                    if ((int)num12 <= 0)
                    {
                        return;
                    }
                    if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                    {
                        return;
                    }
                    Main.player[Main.myPlayer].lifeSteal -= num12 * 1.5f;
                    float num13 = 0f;
                    int num14 = projectile.owner;
                    for (int i = 0; i < 255; i++)
                    {
                        if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[projectile.owner].hostile && !Main.player[i].hostile) || Main.player[projectile.owner].team == Main.player[i].team))
                        {
                            float num15 = Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                            if (num15 < 1200f && (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife) > num13)
                            {
                                num13 = (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife);
                                num14 = i;
                            }
                        }
                    }
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<AuricOrb>(), 0, 0f, projectile.owner, (float)num14, num12);
                }
                else if (Main.player[projectile.owner].Calamity().silvaSet && target.canGhostHeal)
                {
                    float num11 = 0.03f;
                    num11 -= (float)projectile.numHits * 0.015f;
                    if (num11 <= 0f)
                    {
                        return;
                    }
                    float num12 = (float)projectile.damage * num11;
                    if ((int)num12 <= 0)
                    {
                        return;
                    }
                    if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                    {
                        return;
                    }
                    Main.player[Main.myPlayer].lifeSteal -= num12 * 1.5f;
                    float num13 = 0f;
                    int num14 = projectile.owner;
                    for (int i = 0; i < 255; i++)
                    {
                        if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[projectile.owner].hostile && !Main.player[i].hostile) || Main.player[projectile.owner].team == Main.player[i].team))
                        {
                            float num15 = Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                            if (num15 < 1200f && (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife) > num13)
                            {
                                num13 = (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife);
                                num14 = i;
                            }
                        }
                    }
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<SilvaOrb>(), 0, 0f, projectile.owner, (float)num14, num12);
                }

                if (projectile.magic)
                {
                    if (Main.player[projectile.owner].Calamity().silvaMage && projectile.penetrate == 1 && Main.rand.Next(0, 100) >= 97)
                    {
                        Main.PlaySound(29, (int)projectile.position.X, (int)projectile.position.Y, 103);
                        projectile.position = projectile.Center;
                        projectile.width = projectile.height = 96;
                        projectile.position.X -= (float)(projectile.width / 2);
                        projectile.position.Y -= (float)(projectile.height / 2);
                        for (int num193 = 0; num193 < 3; num193++)
                        {
                            Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.5f);
                        }
                        for (int num194 = 0; num194 < 30; num194++)
                        {
                            int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 157, 0f, 0f, 0, new Color(Main.DiscoR, 203, 103), 2.5f);
                            Main.dust[num195].noGravity = true;
                            Main.dust[num195].velocity *= 3f;
                            num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.5f);
                            Main.dust[num195].velocity *= 2f;
                            Main.dust[num195].noGravity = true;
                        }
                        projectile.damage *= Main.player[projectile.owner].Calamity().auricSet ? 7 : 4;
                        projectile.Damage();
                    }

                    if (Main.player[projectile.owner].Calamity().tarraMage && target.canGhostHeal)
                    {
                        if (Main.player[projectile.owner].Calamity().tarraMageHealCooldown <= 0)
                        {
                            Main.player[projectile.owner].Calamity().tarraMageHealCooldown = 90;
                            float num11 = 0.03f;
                            num11 -= (float)projectile.numHits * 0.015f;
                            if (num11 <= 0f)
                            {
                                return;
                            }
                            float num12 = (float)projectile.damage * num11;
                            if ((int)num12 <= 0)
                            {
                                return;
                            }
                            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                            {
                                return;
                            }
                            Main.player[Main.myPlayer].lifeSteal -= num12 * 1.5f;
                            int healAmount = Main.player[projectile.owner].Calamity().auricSet ? projectile.damage / 100 : projectile.damage / 50;
                            Player player = Main.player[projectile.owner];
                            player.statLife += healAmount;
                            player.HealEffect(healAmount);
                            if (player.statLife > player.statLifeMax2)
                            {
                                player.statLife = player.statLifeMax2;
                            }
                        }
                    }

                    if (Main.player[projectile.owner].Calamity().reaverBurst)
                    {
                        int num251 = Main.rand.Next(2, 5);
                        for (int num252 = 0; num252 < num251; num252++)
                        {
                            Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                            while (value15.X == 0f && value15.Y == 0f)
                            {
                                value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                            }
                            value15.Normalize();
                            value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                            int newDamage = (int)((double)projectile.damage * 0.15);
                            if (newDamage > 15)
                            {
                                newDamage = 15;
                            }
                            int proj = Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), value15.X, value15.Y, 569 + Main.rand.Next(3), newDamage, 0f, projectile.owner, 0f, 0f);
                            Main.projectile[proj].usesLocalNPCImmunity = true;
                            Main.projectile[proj].localNPCHitCooldown = 30;
                        }
                    }
                    else if (Main.player[projectile.owner].Calamity().ataxiaMage && Main.player[projectile.owner].Calamity().ataxiaDmg <= 0)
                    {
                        int num = projectile.damage / 2;
                        Main.player[projectile.owner].Calamity().ataxiaDmg += (float)num;
                        int[] array = new int[200];
                        int num3 = 0;
                        int num4 = 0;
                        for (int i = 0; i < 200; i++)
                        {
                            if (Main.npc[i].CanBeChasedBy(projectile, false))
                            {
                                float num5 = Math.Abs(Main.npc[i].position.X + (float)(Main.npc[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                                if (num5 < 800f)
                                {
                                    if (Collision.CanHit(projectile.position, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height) && num5 > 50f)
                                    {
                                        array[num4] = i;
                                        num4++;
                                    }
                                    else if (num4 == 0)
                                    {
                                        array[num3] = i;
                                        num3++;
                                    }
                                }
                            }
                        }
                        if (num3 == 0 && num4 == 0)
                        {
                            return;
                        }
                        int num6;
                        if (num4 > 0)
                        {
                            num6 = array[Main.rand.Next(num4)];
                        }
                        else
                        {
                            num6 = array[Main.rand.Next(num3)];
                        }
                        float num7 = 20f;
                        float num8 = (float)Main.rand.Next(-100, 101);
                        float num9 = (float)Main.rand.Next(-100, 101);
                        float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        num10 = num7 / num10;
                        num8 *= num10;
                        num9 *= num10;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, ModContent.ProjectileType<AtaxiaOrb>(), (int)((double)num * 1.25), 0f, projectile.owner, (float)num6, 0f);
                        if (target.canGhostHeal)
                        {
                            float num11 = 0.1f; //0.2
                            num11 -= (float)projectile.numHits * 0.05f; //0.05
                            if (num11 <= 0f)
                            {
                                return;
                            }
                            float num12 = (float)projectile.damage * num11;
                            if ((int)num12 <= 0)
                            {
                                return;
                            }
                            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                            {
                                return;
                            }
                            Main.player[Main.myPlayer].lifeSteal -= num12 * 1.5f;
                            float num13 = 0f;
                            int num14 = projectile.owner;
                            for (int i = 0; i < 255; i++)
                            {
                                if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[projectile.owner].hostile && !Main.player[i].hostile) || Main.player[projectile.owner].team == Main.player[i].team))
                                {
                                    float num15 = Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                                    if (num15 < 1200f && (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife) > num13)
                                    {
                                        num13 = (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife);
                                        num14 = i;
                                    }
                                }
                            }
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<AtaxiaHealOrb>(), 0, 0f, projectile.owner, (float)num14, num12);
                        }
                    }
                    else if (Main.player[projectile.owner].Calamity().xerocSet && Main.player[projectile.owner].Calamity().xerocDmg <= 0)
                    {
                        int num = projectile.damage / 2;
                        Main.player[projectile.owner].Calamity().xerocDmg += (float)num;
                        int[] array = new int[200];
                        int num3 = 0;
                        int num4 = 0;
                        for (int i = 0; i < 200; i++)
                        {
                            if (Main.npc[i].CanBeChasedBy(projectile, false))
                            {
                                float num5 = Math.Abs(Main.npc[i].position.X + (float)(Main.npc[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                                if (num5 < 800f)
                                {
                                    if (Collision.CanHit(projectile.position, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height) && num5 > 50f)
                                    {
                                        array[num4] = i;
                                        num4++;
                                    }
                                    else if (num4 == 0)
                                    {
                                        array[num3] = i;
                                        num3++;
                                    }
                                }
                            }
                        }
                        if (num3 == 0 && num4 == 0)
                        {
                            return;
                        }
                        int num6;
                        if (num4 > 0)
                        {
                            num6 = array[Main.rand.Next(num4)];
                        }
                        else
                        {
                            num6 = array[Main.rand.Next(num3)];
                        }
                        float num7 = 30f;
                        float num8 = (float)Main.rand.Next(-100, 101);
                        float num9 = (float)Main.rand.Next(-100, 101);
                        float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        num10 = num7 / num10;
                        num8 *= num10;
                        num9 *= num10;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, ModContent.ProjectileType<XerocOrb>(), (int)((double)num * 1.25), 0f, projectile.owner, (float)num6, 0f);
                        if (target.canGhostHeal)
                        {
                            float num11 = 0.1f;
                            num11 -= (float)projectile.numHits * 0.05f;
                            if (num11 <= 0f)
                            {
                                return;
                            }
                            float num12 = (float)projectile.damage * num11;
                            if ((int)num12 <= 0)
                            {
                                return;
                            }
                            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                            {
                                return;
                            }
                            Main.player[Main.myPlayer].lifeSteal -= num12 * 1.5f;
                            float num13 = 0f;
                            int num14 = projectile.owner;
                            for (int i = 0; i < 255; i++)
                            {
                                if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[projectile.owner].hostile && !Main.player[i].hostile) || Main.player[projectile.owner].team == Main.player[i].team))
                                {
                                    float num15 = Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                                    if (num15 < 1200f && (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife) > num13)
                                    {
                                        num13 = (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife);
                                        num14 = i;
                                    }
                                }
                            }
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<XerocHealOrb>(), 0, 0f, projectile.owner, (float)num14, num12);
                        }
                    }
                    else if (Main.player[projectile.owner].Calamity().godSlayerMage && Main.player[projectile.owner].Calamity().godSlayerDmg <= 0)
                    {
                        int num = projectile.damage / 2;
                        Main.player[projectile.owner].Calamity().godSlayerDmg += (float)num;
                        int[] array = new int[200];
                        int num3 = 0;
                        int num4 = 0;
                        for (int i = 0; i < 200; i++)
                        {
                            if (Main.npc[i].CanBeChasedBy(projectile, false))
                            {
                                float num5 = Math.Abs(Main.npc[i].position.X + (float)(Main.npc[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                                if (num5 < 800f)
                                {
                                    if (Collision.CanHit(projectile.position, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height) && num5 > 50f)
                                    {
                                        array[num4] = i;
                                        num4++;
                                    }
                                    else if (num4 == 0)
                                    {
                                        array[num3] = i;
                                        num3++;
                                    }
                                }
                            }
                        }
                        if (num3 == 0 && num4 == 0)
                        {
                            return;
                        }
                        int num6;
                        if (num4 > 0)
                        {
                            num6 = array[Main.rand.Next(num4)];
                        }
                        else
                        {
                            num6 = array[Main.rand.Next(num3)];
                        }
                        float num7 = 20f;
                        float num8 = (float)Main.rand.Next(-100, 101);
                        float num9 = (float)Main.rand.Next(-100, 101);
                        float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        num10 = num7 / num10;
                        num8 *= num10;
                        num9 *= num10;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, ModContent.ProjectileType<GodSlayerOrb>(),
                            (int)((double)num * (Main.player[projectile.owner].Calamity().auricSet ? 2.0 : 1.5)), 0f, projectile.owner, (float)num6, 0f);
                        if (target.canGhostHeal)
                        {
                            float num11 = Main.player[projectile.owner].Calamity().auricSet ? 0.03f : 0.06f; //0.2
                            num11 -= (float)projectile.numHits * 0.015f; //0.05
                            if (num11 <= 0f)
                            {
                                return;
                            }
                            float num12 = (float)projectile.damage * num11;
                            if ((int)num12 <= 0)
                            {
                                return;
                            }
                            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                            {
                                return;
                            }
                            Main.player[Main.myPlayer].lifeSteal -= num12 * 1.5f;
                            float num13 = 0f;
                            int num14 = projectile.owner;
                            for (int i = 0; i < 255; i++)
                            {
                                if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[projectile.owner].hostile && !Main.player[i].hostile) || Main.player[projectile.owner].team == Main.player[i].team))
                                {
                                    float num15 = Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                                    if (num15 < 1200f && (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife) > num13)
                                    {
                                        num13 = (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife);
                                        num14 = i;
                                    }
                                }
                            }
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<GodSlayerHealOrb>(), 0, 0f, projectile.owner, (float)num14, num12);
                        }
                    }
                }
                else if (projectile.melee)
                {
                    if (Main.player[projectile.owner].Calamity().ataxiaGeyser && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<ChaosGeyser>()] < 3)
                    {
                        int newDamage = (int)((double)projectile.damage * 0.15);
                        if (newDamage > 35)
                        {
                            newDamage = 35;
                        }
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<ChaosGeyser>(), newDamage, 0f, projectile.owner, 0f, 0f);
                    }
                    else if (Main.player[projectile.owner].Calamity().xerocSet && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<XerocBlast>()] < 3)
                    {
                        int newDamage = (int)((double)projectile.damage * 0.2);
                        if (newDamage > 40)
                        {
                            newDamage = 40;
                        }
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<XerocBlast>(), newDamage, 0f, projectile.owner, 0f, 0f);
                    }
                }
                else if (projectile.ranged)
                {
                    if (Main.player[projectile.owner].Calamity().xerocSet && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<XerocFire>()] < 3)
                    {
                        int newDamage = (int)((double)projectile.damage * 0.15);
                        if (newDamage > 40)
                        {
                            newDamage = 40;
                        }
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<XerocFire>(), newDamage, 0f, projectile.owner, 0f, 0f);
                    }
                }
                else if (rogue)
                {
                    if (Main.player[projectile.owner].Calamity().xerocSet && Main.player[projectile.owner].Calamity().xerocDmg <= 0)
                    {
                        int num = projectile.damage / 2;
                        Main.player[projectile.owner].Calamity().xerocDmg += (float)num;
                        int[] array = new int[200];
                        int num3 = 0;
                        int num4 = 0;
                        for (int i = 0; i < 200; i++)
                        {
                            if (Main.npc[i].CanBeChasedBy(projectile, false))
                            {
                                float num5 = Math.Abs(Main.npc[i].position.X + (float)(Main.npc[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                                if (num5 < 800f)
                                {
                                    if (Collision.CanHit(projectile.position, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height) && num5 > 50f)
                                    {
                                        array[num4] = i;
                                        num4++;
                                    }
                                    else if (num4 == 0)
                                    {
                                        array[num3] = i;
                                        num3++;
                                    }
                                }
                            }
                        }
                        if (num3 == 0 && num4 == 0)
                        {
                            return;
                        }
                        int num6;
                        if (num4 > 0)
                        {
                            num6 = array[Main.rand.Next(num4)];
                        }
                        else
                        {
                            num6 = array[Main.rand.Next(num3)];
                        }
                        float num7 = Main.rand.Next(15, 30);
                        float num8 = (float)Main.rand.Next(-100, 101);
                        float num9 = (float)Main.rand.Next(-100, 101);
                        float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        num10 = num7 / num10;
                        num8 *= num10;
                        num9 *= num10;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, ModContent.ProjectileType<XerocStar>(), (int)((double)num * 1.6), 0f, projectile.owner, (float)num6, 0f);
                    }

                    if (Main.player[projectile.owner].Calamity().featherCrown && stealthStrike)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 pos = new Vector2(target.position.X + (float)target.width * 0.5f + (float)Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
                            float speedX = (target.position.X - pos.X) / 30f;
                            float speedY = (target.position.Y - pos.Y) * 8;
                            int feather = Projectile.NewProjectile(pos.X, pos.Y, speedX, speedY, ModContent.ProjectileType<StickyFeather>(), 15, 3, projectile.owner, 0f, (float)Main.rand.Next(15));
                            Main.projectile[feather].magic = false;
                            Main.projectile[feather].Calamity().rogue = true;
                        }
                    }

                    if (Main.player[projectile.owner].Calamity().moonCrown && stealthStrike)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 pos = new Vector2(target.position.X + (float)target.width * 0.5f + (float)Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
                            Vector2 velocity = (target.position - pos) / 10f;
                            int flare = Projectile.NewProjectile(pos, velocity, ProjectileID.LunarFlare, 50, 3, projectile.owner);
                            Main.projectile[flare].magic = false;
                            Main.projectile[flare].Calamity().rogue = true;
                        }
                    }
                }
                else if (projectile.minion || projectile.sentry || CalamityMod.projectileMinionList.Contains(projectile.type))
                {
                    if (Main.player[projectile.owner].Calamity().pArtifact)
                    {
                        target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
                    }

                    if (Main.player[projectile.owner].Calamity().tearMinions)
                    {
                        target.AddBuff(ModContent.BuffType<TemporalSadness>(), 60);
                    }

                    if (Main.player[projectile.owner].Calamity().shadowMinions)
                    {
                        target.AddBuff(BuffID.ShadowFlame, 300);
                    }

                    if (Main.player[projectile.owner].Calamity().godSlayerSummon && Main.player[projectile.owner].Calamity().godSlayerDmg <= 0)
                    {
                        int num = projectile.damage / 2;
                        float ai1 = Main.rand.NextFloat() + 0.5f;
                        Main.player[projectile.owner].Calamity().godSlayerDmg += (float)num;
                        int[] array = new int[200];
                        int num3 = 0;
                        int num4 = 0;
                        for (int i = 0; i < 200; i++)
                        {
                            if (Main.npc[i].CanBeChasedBy(projectile, false))
                            {
                                float num5 = Math.Abs(Main.npc[i].position.X + (float)(Main.npc[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                                if (num5 < 800f)
                                {
                                    if (Collision.CanHit(projectile.position, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height) && num5 > 50f)
                                    {
                                        array[num4] = i;
                                        num4++;
                                    }
                                    else if (num4 == 0)
                                    {
                                        array[num3] = i;
                                        num3++;
                                    }
                                }
                            }
                        }
                        if (num3 == 0 && num4 == 0)
                        {
                            return;
                        }
                        int num6;
                        if (num4 > 0)
                        {
                            num6 = array[Main.rand.Next(num4)];
                        }
                        else
                        {
                            num6 = array[Main.rand.Next(num3)];
                        }
                        float num7 = 15f;
                        float num8 = (float)Main.rand.Next(-100, 101);
                        float num9 = (float)Main.rand.Next(-100, 101);
                        float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        num10 = num7 / num10;
                        num8 *= num10;
                        num9 *= num10;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, ModContent.ProjectileType<GodSlayerPhantom>(), (int)((double)num * 2.0), 0f, projectile.owner, 0f, ai1);
                    }
                    else if (Main.player[projectile.owner].Calamity().xerocSet && Main.player[projectile.owner].Calamity().xerocDmg <= 0)
                    {
                        int num = projectile.damage / 2;
                        Main.player[projectile.owner].Calamity().xerocDmg += (float)num;
                        int[] array = new int[200];
                        int num3 = 0;
                        int num4 = 0;
                        for (int i = 0; i < 200; i++)
                        {
                            if (Main.npc[i].CanBeChasedBy(projectile, false))
                            {
                                float num5 = Math.Abs(Main.npc[i].position.X + (float)(Main.npc[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                                if (num5 < 800f)
                                {
                                    if (Collision.CanHit(projectile.position, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height) && num5 > 50f)
                                    {
                                        array[num4] = i;
                                        num4++;
                                    }
                                    else if (num4 == 0)
                                    {
                                        array[num3] = i;
                                        num3++;
                                    }
                                }
                            }
                        }
                        if (num3 == 0 && num4 == 0)
                        {
                            return;
                        }
                        int num6;
                        if (num4 > 0)
                        {
                            num6 = array[Main.rand.Next(num4)];
                        }
                        else
                        {
                            num6 = array[Main.rand.Next(num3)];
                        }
                        float num7 = 15f;
                        float num8 = (float)Main.rand.Next(-100, 101);
                        float num9 = (float)Main.rand.Next(-100, 101);
                        float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        num10 = num7 / num10;
                        num8 *= num10;
                        num9 *= num10;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, ModContent.ProjectileType<XerocBubble>(), (int)((double)num * 1.2), 0f, projectile.owner, (float)num6, 0f);
                    }
                }
            }
        }
        #endregion

        #region CanDamage
        public override bool CanDamage(Projectile projectile)
        {
            switch (projectile.type)
            {
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
            return true;
        }
        #endregion

        #region Drawing
        public override Color? GetAlpha(Projectile projectile, Color lightColor)
        {
            if (Main.player[Main.myPlayer].Calamity().trippy)
                return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, projectile.alpha);

            if (projectile.type == ProjectileID.PinkLaser)
            {
                if (projectile.alpha < 200)
                    return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);

                return Color.Transparent;
            }

            return null;
        }

        public override bool PreDraw(Projectile projectile, SpriteBatch spriteBatch, Color lightColor)
        {
            if (Main.player[Main.myPlayer].Calamity().trippy)
            {
                Texture2D texture = Main.projectileTexture[projectile.type];
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (projectile.spriteDirection == -1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                float num66 = 0f;
                Vector2 vector11 = new Vector2((float)(texture.Width / 2), (float)(texture.Height / Main.projFrames[projectile.type] / 2));
                Color color9 = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
                Color alpha15 = projectile.GetAlpha(color9);
                float num212 = 0.99f;
                alpha15.R = (byte)((float)alpha15.R * num212);
                alpha15.G = (byte)((float)alpha15.G * num212);
                alpha15.B = (byte)((float)alpha15.B * num212);
                alpha15.A = (byte)((float)alpha15.A * num212);
                for (int num213 = 0; num213 < 4; num213++)
                {
                    Vector2 position9 = projectile.position;
                    float num214 = Math.Abs(projectile.Center.X - Main.player[Main.myPlayer].Center.X);
                    float num215 = Math.Abs(projectile.Center.Y - Main.player[Main.myPlayer].Center.Y);
                    if (num213 == 0 || num213 == 2)
                    {
                        position9.X = Main.player[Main.myPlayer].Center.X + num214;
                    }
                    else
                    {
                        position9.X = Main.player[Main.myPlayer].Center.X - num214;
                    }
                    position9.X -= (float)(projectile.width / 2);
                    if (num213 == 0 || num213 == 1)
                    {
                        position9.Y = Main.player[Main.myPlayer].Center.Y + num215;
                    }
                    else
                    {
                        position9.Y = Main.player[Main.myPlayer].Center.Y - num215;
                    }
                    int frames = texture.Height / Main.projFrames[projectile.type];
                    int y = frames * projectile.frame;
                    position9.Y -= (float)(projectile.height / 2);
                    Main.spriteBatch.Draw(texture,
                        new Vector2(position9.X - Main.screenPosition.X + (float)(projectile.width / 2) - (float)texture.Width * projectile.scale / 2f + vector11.X * projectile.scale, position9.Y - Main.screenPosition.Y + (float)projectile.height - (float)texture.Height * projectile.scale / (float)Main.projFrames[projectile.type] + 4f + vector11.Y * projectile.scale + num66 + projectile.gfxOffY),
                        new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y, texture.Width, frames)), alpha15, projectile.rotation, vector11, projectile.scale, spriteEffects, 0f);
                }
            }
            return true;
        }

        public static void DrawCenteredAndAfterimage(Projectile projectile, Color lightColor, int trailingMode, int afterimageCounter, Texture2D texture = null)
        {
            if (texture == null)
                texture = Main.projectileTexture[projectile.type];

            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int frameY = frameHeight * projectile.frame;

            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (Lighting.NotRetro)
            {
                switch (trailingMode)
                {
                    case 0:
                        for (int i = 0; i < projectile.oldPos.Length; i++)
                        {
                            Vector2 drawPos = projectile.oldPos[i] + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
                            Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
                            Main.spriteBatch.Draw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), color, projectile.rotation, rectangle.Size() / 2f, projectile.scale, spriteEffects, 0f);
                        }
                        break;

                    case 1:
                        Color color25 = Lighting.GetColor((int)(projectile.Center.X / 16), (int)(projectile.Center.Y / 16));
                        int num157 = 8;
                        int num161 = 1;
                        while (num161 < num157)
                        {
                            Color color26 = color25;
                            color26 = projectile.GetAlpha(color26);
                            goto IL_6899;
                            IL_6881:
                            num161 += afterimageCounter;
                            continue;
                            IL_6899:
                            float num164 = (float)(num157 - num161);
                            color26 *= num164 / ((float)ProjectileID.Sets.TrailCacheLength[projectile.type] * 1.5f);
                            Main.spriteBatch.Draw(texture, projectile.oldPos[num161] + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, rectangle.Size() / 2f, projectile.scale, spriteEffects, 0f);
                            goto IL_6881;
                        }
                        break;

                    default:
                        break;
                }
            }
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameY, texture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)frameHeight / 2f), projectile.scale, spriteEffects, 0f);
        }
        #endregion

        #region Kill
        public override void Kill(Projectile projectile, int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                if (Main.player[projectile.owner].Calamity().providenceLore && projectile.friendly && projectile.damage > 0 && (projectile.melee || projectile.ranged || projectile.magic || rogue))
                {
                    Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
                    for (int i = 0; i < 3; i++)
                    {
                        int dust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 1f);
                        Main.dust[dust].noGravity = true;
                    }
                }

                if (projectile.ranged)
                {
                    if (Main.player[projectile.owner].Calamity().tarraRanged && Main.rand.Next(0, 100) >= 88)
                    {
                        int num251 = Main.rand.Next(2, 4);
                        for (int num252 = 0; num252 < num251; num252++)
                        {
                            Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                            while (value15.X == 0f && value15.Y == 0f)
                            {
                                value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                            }
                            value15.Normalize();
                            value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                            int newDamage = (int)((double)projectile.damage * 0.33);
                            if (newDamage > 65)
                            {
                                newDamage = 65;
                            }
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<TarraEnergy>(), newDamage, 0f, projectile.owner, 0f, 0f);
                        }
                    }
                }
            }
        }
        #endregion

        #region CanHit
        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            if (projectile.type == ProjectileID.CultistBossLightningOrb)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
