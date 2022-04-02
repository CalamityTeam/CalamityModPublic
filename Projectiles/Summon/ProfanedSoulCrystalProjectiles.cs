using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.NPCs.SupremeCalamitas;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    #region Mage Projectiles

    #region Main Fireball
    public class ProfanedCrystalMageFireball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Bab Fireball");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        private void Split(bool hit, bool chaseable)
        {
            Player player = Main.player[projectile.owner];
            bool enrage = player.statLife <= (int)((double)player.statLifeMax2 * 0.5);
            player.Calamity().rollBabSpears(hit ? 1 : 0, chaseable);
            int outerSplits = enrage ? 20 : 10;
            int innerSplits = enrage ? 16 : 8;
            float mult = enrage ? 0.4f : 0.6f;
            if (!hit)
                mult = enrage ? 0.2f : 0.1f; //punishing to miss
            int damage = (int)((projectile.damage * 0.2f) * mult);

            float outerAngleVariance = MathHelper.TwoPi / (float)outerSplits;
            float outerOffsetAngle = MathHelper.Pi / (2f * outerSplits);
            float innerAngleVariance = MathHelper.TwoPi / (float)innerSplits;
            float innerOffsetAngle = MathHelper.Pi / (2f * innerSplits);
            Vector2 outerPosVec = new Vector2(8f, 0f).RotatedByRandom(MathHelper.TwoPi);
            Vector2 innerPosVec = new Vector2(5f, 0f).RotatedByRandom(MathHelper.TwoPi); //Two vector variables since they get reassigned during the loop

            for (int i = 0; i < outerSplits; i++) //use outer since it has the higher number regardless
            {
                outerPosVec = outerPosVec.RotatedBy(outerAngleVariance);
                Vector2 velocity = new Vector2(outerPosVec.X, outerPosVec.Y).RotatedBy(outerOffsetAngle);
                velocity.Normalize();
                velocity *= 8f;
                int proj = Projectile.NewProjectile(projectile.Center + outerPosVec, velocity, ModContent.ProjectileType<ProfanedCrystalMageFireballSplit>(), damage, projectile.knockBack, projectile.owner, 0f, 0f);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().forceMinion = true;
                if (innerSplits > 0) //only runs if there's still inner splits to create
                {
                    innerPosVec = innerPosVec.RotatedBy(innerAngleVariance);
                    velocity = new Vector2(innerPosVec.X, innerPosVec.Y).RotatedBy(innerOffsetAngle);
                    velocity.Normalize();
                    velocity *= 5f;
                    proj = Projectile.NewProjectile(projectile.Center + innerPosVec, velocity, ModContent.ProjectileType<ProfanedCrystalMageFireballSplit>(), damage, projectile.knockBack, projectile.owner, 0f, 0f);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().forceMinion = true;
                }
                innerSplits--;
            }

        }

        public override void SetDefaults()
        {
            projectile.width = 180;
            projectile.height = 180;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 75;
            projectile.minion = true;
            projectile.scale = 0.6f;
        }

        public override bool PreAI()
        {
            projectile.Calamity().overridesMinionDamagePrevention = true;
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            if (projectile.timeLeft == 50)
            {
                projectile.tileCollide = true;
            }
            if (projectile.wet && !projectile.lavaWet && projectile.timeLeft < 70) //timeleft check because projectile lavawet isn't set until after the first frame
            {
                projectile.Kill();
            }
            if ((double)Math.Abs(projectile.velocity.X) > 0.2)
            {
                projectile.rotation = projectile.direction;
            }
            if (projectile.velocity.X < 0f)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
            }
            else
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
            }
            return false;
        }

        public override void AI()
        {
            int num469 = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 3, texture2D13);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer == projectile.owner)
            {
                Split(true, target.chaseable);
            }
            projectile.active = false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.myPlayer == projectile.owner)
            {
                Split(true, true);
            }
            projectile.active = false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == projectile.owner)
            {
                Split(false, false);
            }
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 20);
            for (int num193 = 0; num193 < 6; num193++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 50, default, 1.5f);
            }
            for (int num194 = 0; num194 < 60; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 0, default, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 50, default, 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }
    }
    #endregion

    #region Fireball Split
    public class ProfanedCrystalMageFireballSplit : ModProjectile
    {
        private int damage;
        private int hits = 0;
        private NPC target = null;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Bab Fire");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 600;
            projectile.minion = true;
            projectile.gfxOffY = -25f;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }

        public override bool PreAI()
        {
            projectile.Calamity().overridesMinionDamagePrevention = true;
            return true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 5, 1, 0);
            if (projectile.timeLeft == 600)
            {
                damage = projectile.damage;
                projectile.damage = 0;

            }
            if (projectile.timeLeft > 550)
                projectile.velocity *= 0.95f;
            int num469 = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;


            if (projectile.timeLeft <= 550)
            {
                if (projectile.penetrate == -1)
                    projectile.damage = damage;
                projectile.penetrate = 1;
                if (projectile.timeLeft > 500)
                    projectile.velocity *= 1.06f;
                float num535 = projectile.position.X;
                float num536 = projectile.position.Y;
                float num537 = 3000f;
                bool flag19 = false;
                NPC ownerMinionAttackTargetNPC2 = projectile.OwnerMinionAttackTargetNPC;
                if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(projectile, false))
                {
                    float num539 = ownerMinionAttackTargetNPC2.position.X + (float)(ownerMinionAttackTargetNPC2.width / 2);
                    float num540 = ownerMinionAttackTargetNPC2.position.Y + (float)(ownerMinionAttackTargetNPC2.height / 2);
                    float num541 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num539) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num540);
                    if (num541 < num537)
                    {
                        num537 = num541;
                        num535 = num539;
                        num536 = num540;
                        flag19 = true;
                        target = ownerMinionAttackTargetNPC2;
                    }
                }
                if (!flag19)
                {
                    int num3;
                    for (int num542 = 0; num542 < Main.npc.Length; num542 = num3 + 1)
                    {
                        if (Main.npc[num542].CanBeChasedBy(projectile, false))
                        {
                            float num543 = Main.npc[num542].position.X + (float)(Main.npc[num542].width / 2);
                            float num544 = Main.npc[num542].position.Y + (float)(Main.npc[num542].height / 2);
                            float num545 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num543) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num544);
                            if (num545 < num537)
                            {
                                num537 = num545;
                                num535 = num543;
                                num536 = num544;
                                flag19 = true;
                                target = Main.npc[num542];
                            }
                        }
                        num3 = num542;
                    }
                }
                if (flag19)
                {
                    if (projectile.ai[1] == 0f)
                    {
                        float num550 = 24f; //12
                        Vector2 vector43 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                        float num551 = num535 - vector43.X;
                        float num552 = num536 - vector43.Y;
                        float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
                        if (num553 < 100f)
                        {
                            num550 = 28f; //14
                        }
                        num553 = num550 / num553;
                        num551 *= num553;
                        num552 *= num553;
                        projectile.velocity.X = (projectile.velocity.X * 14f + num551) / 15f;
                        projectile.velocity.Y = (projectile.velocity.Y * 14f + num552) / 15f;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[projectile.owner].Calamity().rollBabSpears(35, target.chaseable);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Main.player[projectile.owner].Calamity().rollBabSpears(35, true);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (hits > 0)
            {
                damage -= (int)(damage * (0.1f * (hits / 10))); //loses 10% damage per 10 hits
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            bool instakill = target.type == ModContent.NPCType<SCalWormHead>() || target.type == ModContent.NPCType<SCalWormBody>() || target.type == ModContent.NPCType<SCalWormBodyWeak>() || target.type == ModContent.NPCType<SCalWormTail>();
            if (!instakill && this.target != null && target != this.target)
            {
                if (projectile.getRect().Intersects(target.getRect()))
                {
                    hits++;
                    if (hits >= 100)
                        projectile.Kill();
                }
                return false;
            }
            return null;
        }

        public override void Kill(int timeLeft)
        {

            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 14);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = projectile.height = 200;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 4; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 12; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;

            }
        }
    }
    #endregion

    #endregion

    #region Melee Projectile
    public class ProfanedCrystalMeleeSpear : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Friendly Spear");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 50;
            projectile.minion = true;
            projectile.alpha = 100;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            projectile.velocity *= 1.06f;
            projectile.rotation = projectile.velocity.ToRotation() + 1.57079637f;
            Lighting.AddLight(projectile.Center, 5, 1, 0);
        }

        public override bool PreAI()
        {
            projectile.Calamity().overridesMinionDamagePrevention = true;
            if (projectile.timeLeft == 50 && projectile.ai[1] == 1f)
                projectile.penetrate = 3;
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.timeLeft > 133 && projectile.ai[0] < 2f)
                return false;
            return null;
        }

        public override bool CanHitPvp(Player target)
        {
            return projectile.timeLeft <= 133 || projectile.ai[0] == 2f;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.ai[0] == 1f && projectile.penetrate == 1)
            {
                handleSpecialHit(target.Center);
            }
            int chance = projectile.ai[0] == 2f ? 20 : 10 * projectile.penetrate;
            Main.player[projectile.owner].Calamity().rollBabSpears(chance, target.chaseable);
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (projectile.ai[0] == 1f && projectile.penetrate == 1)
            {
                handleSpecialHit(target.Center);
            }
            int chance = projectile.ai[0] == 2f ? 20 : 10 * projectile.penetrate;
            Main.player[projectile.owner].Calamity().rollBabSpears(chance, true);
        }

        private void handleSpecialHit(Vector2 targCenter)
        {
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 10; ++i)
                {
                    float startDist = Main.rand.NextFloat(450f, 500f);
                    Vector2 startDir = Main.rand.NextVector2Unit();
                    Vector2 startPoint = targCenter + (startDir * startDist);

                    float speed = Main.rand.NextFloat(15f, 18f);
                    Vector2 velocity = startDir * (-speed);
                    Projectile.NewProjectile(startPoint, velocity, ModContent.ProjectileType<ProfanedCrystalMeleeSpear>(), (int)(0.25f * projectile.damage), 0f, projectile.owner, 2f, 0f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft > 133)
            {
                projectile.localAI[0] += 5f;
                byte b2 = (byte)(((int)projectile.localAI[0]) * 3);
                byte a2 = (byte)((float)projectile.alpha * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, projectile.alpha);
        }

        private void onHit()
        {

            Main.PlaySound(new LegacySoundStyle(2, 74, Terraria.Audio.SoundType.Sound), (int)projectile.position.X, (int)projectile.position.Y);
            if (Main.rand.NextBool(2) || Main.rand.NextBool(3)) //so it's not exactly 1 in 2, but it's not more or less consistently either.
            {
                projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
                projectile.width = projectile.height = 200;
                projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
                for (int num621 = 0; num621 < 4; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 12; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;

                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            onHit();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            onHit();
        }
    }
    #endregion

    #region Ranger Projectiles

    #region Meteors
    public class ProfanedCrystalRangedHuges : ModProjectile
    {
        bool boomerSwarm = false;
        bool kill = false;
        NPC target = null;

        private void swarmAI()
        {
            float num535 = projectile.position.X;
            float num536 = projectile.position.Y;
            float num537 = 2000f;
            bool flag19 = false;
            NPC ownerMinionAttackTargetNPC2 = projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(projectile, false))
            {
                float num539 = ownerMinionAttackTargetNPC2.position.X + (float)(ownerMinionAttackTargetNPC2.width / 2);
                float num540 = ownerMinionAttackTargetNPC2.position.Y + (float)(ownerMinionAttackTargetNPC2.height / 2);
                float num541 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num539) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num540);
                if (num541 < num537)
                {
                    num537 = num541;
                    num535 = num539;
                    num536 = num540;
                    flag19 = true;
                    target = ownerMinionAttackTargetNPC2;
                }
            }
            if (!flag19)
            {
                for (int num542 = 0; num542 < Main.npc.Length; num542++)
                {
                    if (Main.npc[num542].CanBeChasedBy(projectile, false))
                    {
                        float num543 = Main.npc[num542].position.X + (float)(Main.npc[num542].width / 2);
                        float num544 = Main.npc[num542].position.Y + (float)(Main.npc[num542].height / 2);
                        float num545 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num543) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num544);
                        if (num545 < num537)
                        {
                            num537 = num545;
                            num535 = num543;
                            num536 = num544;
                            flag19 = true;
                            target = Main.npc[num542];
                        }
                    }
                }
            }
            if (flag19)
            {
                projectile.tileCollide = false;
                if (projectile.timeLeft < 20)
                {
                    projectile.timeLeft = 50;
                    kill = true;
                }

                float num550 = 40f;
                Vector2 vector43 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float num551 = num535 - vector43.X;
                float num552 = num536 - vector43.Y;
                float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
                if (num553 < 100f)
                {
                    num550 = 28f; //14
                }
                num553 = num550 / num553;
                num551 *= num553;
                num552 *= num553;
                projectile.velocity.X = (projectile.velocity.X * 14f + num551) / 15f;
                projectile.velocity.Y = (projectile.velocity.Y * 14f + num552) / 15f;

            }
            else
            {
                if (kill)
                    projectile.Kill();
                projectile.tileCollide = true;
            }
        }

        private void swarm(Vector2 targetPos)
        {
            if (projectile.owner == Main.myPlayer)
            {
                float swarmAmount = Main.rand.Next(6, 10); //6-9
                for (float i = 0; i < swarmAmount; i++)
                {
                    float x = targetPos.X + Main.rand.Next(-500, 501);
                    float y = targetPos.Y - 500 + Main.rand.Next(-200, 1);
                    Vector2 pos = new Vector2(x, y);
                    Vector2 correctedVelocity = projectile.position - pos;
                    correctedVelocity.Normalize();
                    correctedVelocity *= 25f;
                    int proj = Projectile.NewProjectile(pos, correctedVelocity, ModContent.ProjectileType<ProfanedCrystalRangedHuges>(), (int)((double)projectile.damage * 1.5f), projectile.knockBack, projectile.owner, 2);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().forceMinion = true;
                    ((ProfanedCrystalRangedHuges)Main.projectile[proj].modProjectile).boomerSwarm = true;
                }
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Bab Boomer");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.penetrate = 1;
            projectile.timeLeft = 175;
            projectile.localNPCHitCooldown = -1;
            projectile.usesLocalNPCImmunity = true;
        }

        public override bool PreAI()
        {
            projectile.Calamity().overridesMinionDamagePrevention = true;
            bool begin = projectile.timeLeft == 175;
            if (projectile.ai[0] >= 1 && begin)
            {
                projectile.scale = 1.5f;
                projectile.width += 25;
                projectile.height += 25;
                boomerSwarm = projectile.ai[0] == 2;
                if (boomerSwarm && projectile.ai[1] == 0f)
                {
                    projectile.timeLeft = 200;
                    projectile.ai[1] = 1f;
                }
            }
            return true;
        }

        public override void AI()
        {
            if (projectile.timeLeft == 175 && projectile.scale == 1.5f)
                Main.PlayTrackedSound(SoundID.DD2_BetsyFireballShot, projectile.Center);

            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 2)
            {
                projectile.frame = 0;
            }
            if (projectile.timeLeft == 145 && projectile.ai[0] < 2f)
            {
                projectile.tileCollide = true;
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;

            projectile.velocity *= boomerSwarm ? 1.03f : 1.02f;

            int num469 = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
            if (boomerSwarm)
                swarmAI();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            bool instakill = target.type == ModContent.NPCType<SCalWormHead>() || target.type == ModContent.NPCType<SCalWormBody>() || target.type == ModContent.NPCType<SCalWormBodyWeak>() || target.type == ModContent.NPCType<SCalWormTail>();
            if (!instakill && this.target != null && target != this.target)
                return false;
            return null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[projectile.owner].Calamity().rollBabSpears(projectile.ai[0] == 1f ? 1 : 10, target.chaseable);
            if (projectile.scale == 1.5f && projectile.ai[0] != 2f)
                swarm(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Main.player[projectile.owner].Calamity().rollBabSpears(projectile.scale == 1.5f ? 1 : 10, true);
            if (projectile.scale == 1.5f && projectile.ai[0] != 2f)
                swarm(target.Center);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 14);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = projectile.height = 200;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 4; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 12; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
            if (projectile.scale != 1.5f)
                return;
            for (int num625 = 0; num625 < 3; num625++)
            {
                float scaleFactor10 = 0.33f;
                if (num625 == 1)
                {
                    scaleFactor10 = 0.66f;
                }
                if (num625 == 2)
                {
                    scaleFactor10 = 1f;
                }
                for (int i = 0; i < 4; i++)
                {
                    int num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
                    Gore gore = Main.gore[num626];
                    gore.velocity *= scaleFactor10;
                    if (i == 0 || i == 2)
                        gore.velocity.X += 1f;
                    else
                        gore.velocity.X -= 1f;
                    if (i < 2)
                        gore.velocity.Y += 1f;
                    else
                        gore.velocity.Y -= 1f;
                }
            }
        }
    }
    #endregion

    #region Fireballs

    public class ProfanedCrystalRangedSmalls : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Bab Boomer Orb");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.scale = 1.5f;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 240;
            projectile.minion = true;
        }

        public override bool PreAI()
        {
            projectile.Calamity().overridesMinionDamagePrevention = true;
            return true;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            if (projectile.ai[0] == 0f) {
            }
            projectile.velocity.X *= 1.01f;
            projectile.velocity.Y *= 1.01f;
            if (projectile.timeLeft == 210)
                projectile.tileCollide = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.ai[0] == 1f && projectile.timeLeft > 210)
                return false;
            return null;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 14);
            if (!Main.rand.NextBool(3)) //1 in 3 chance for dust
                return;
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 50;
            projectile.height = 50;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 10; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 15; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 247, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[projectile.owner].Calamity().rollBabSpears(50, target.chaseable);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Main.player[projectile.owner].Calamity().rollBabSpears(50, true);
        }
    }

    #endregion

    #endregion

    #region Rogue Projectile

    public class ProfanedCrystalRogueShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Bab Crystal Shard");
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 150;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.scale = 0.8f;
        }

        private void ai()
        {
            if (projectile.timeLeft > 90)
            {
                projectile.rotation += 1f;
                projectile.velocity.X *= 0.985f;
                projectile.velocity.Y *= 0.985f;
            }
            else
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
                if (projectile.spriteDirection == -1)
                {
                    projectile.rotation -= 1.57f;
                }
                float num535 = projectile.position.X;
                float num536 = projectile.position.Y;
                float num537 = 2000f;
                bool flag19 = false;
                NPC ownerMinionAttackTargetNPC2 = projectile.OwnerMinionAttackTargetNPC;
                if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(projectile, false))
                {
                    float num539 = ownerMinionAttackTargetNPC2.position.X + (float)(ownerMinionAttackTargetNPC2.width / 2);
                    float num540 = ownerMinionAttackTargetNPC2.position.Y + (float)(ownerMinionAttackTargetNPC2.height / 2);
                    float num541 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num539) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num540);
                    if (num541 < num537)
                    {
                        num537 = num541;
                        num535 = num539;
                        num536 = num540;
                        flag19 = true;
                    }
                }
                if (!flag19)
                {
                    int num3;
                    for (int num542 = 0; num542 < 200; num542 = num3 + 1)
                    {
                        if (Main.npc[num542].CanBeChasedBy(projectile, false))
                        {
                            float num543 = Main.npc[num542].position.X + (float)(Main.npc[num542].width / 2);
                            float num544 = Main.npc[num542].position.Y + (float)(Main.npc[num542].height / 2);
                            float num545 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num543) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num544);
                            if (num545 < num537)
                            {
                                num537 = num545;
                                num535 = num543;
                                num536 = num544;
                                flag19 = true;
                            }
                        }
                        num3 = num542;
                    }
                }
                if (flag19) //target found
                {
                    if (Main.rand.NextBool())
                        projectile.timeLeft++;
                    projectile.tileCollide = false;
                    float num550 = 40f;
                    Vector2 vector43 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num551 = num535 - vector43.X;
                    float num552 = num536 - vector43.Y;
                    float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
                    if (num553 < 100f)
                    {
                        num550 = 28f; //14
                    }
                    num553 = num550 / num553;
                    num551 *= num553;
                    num552 *= num553;
                    projectile.velocity.X = (projectile.velocity.X * 14f + num551) / 15f;
                    projectile.velocity.Y = (projectile.velocity.Y * 14f + num552) / 15f;

                }
                else
                {
                    projectile.tileCollide = true;
                }
            }

        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override bool PreAI()
        {
            projectile.Calamity().overridesMinionDamagePrevention = true;
            return true;
        }

        public override void AI()
        {
            ai();
            Color newColor2 = Main.hslToRgb(projectile.ai[0], 1f, 0.5f);
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 8;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.alpha == 0)
            {
                Lighting.AddLight(projectile.Center, newColor2.ToVector3() * 0.5f);
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) - 1.57f;
            int num3;
            for (int num979 = 0; num979 < 2; num979 = num3 + 1)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value55 = Vector2.UnitY.RotatedBy((double)((float)num979 * 3.14159274f), default).RotatedBy((double)projectile.rotation, default);
                    Dust dust24 = Main.dust[Dust.NewDust(projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    dust24.noGravity = true;
                    dust24.noLight = true;
                    dust24.scale = projectile.Opacity * projectile.localAI[0];
                    dust24.position = projectile.Center;
                    dust24.velocity = value55 * 2.5f;
                }
                num3 = num979;
            }
            for (int num980 = 0; num980 < 2; num980 = num3 + 1)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value56 = Vector2.UnitY.RotatedBy((double)((float)num980 * 3.14159274f), default);
                    Dust dust25 = Main.dust[Dust.NewDust(projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    dust25.noGravity = true;
                    dust25.noLight = true;
                    dust25.scale = projectile.Opacity * projectile.localAI[0];
                    dust25.position = projectile.Center;
                    dust25.velocity = value56 * 2.5f;
                }
                num3 = num980;
            }
            if (Main.rand.NextBool(10))
            {
                float scaleFactor13 = 1f + Main.rand.NextFloat() * 2f;
                float fadeIn = 1f + Main.rand.NextFloat();
                float num981 = 1f + Main.rand.NextFloat();
                Vector2 vector136 = Utils.RandomVector2(Main.rand, -1f, 1f);
                if (vector136 != Vector2.Zero)
                {
                    vector136.Normalize();
                }
                vector136 *= 20f + Main.rand.NextFloat() * 100f;
                Vector2 vector137 = projectile.Center + vector136;
                Point point3 = vector137.ToTileCoordinates();
                bool flag52 = true;
                if (!WorldGen.InWorld(point3.X, point3.Y, 0))
                {
                    flag52 = false;
                }
                if (flag52 && WorldGen.SolidTile(point3.X, point3.Y))
                {
                    flag52 = false;
                }
                if (flag52)
                {
                    Dust dust26 = Main.dust[Dust.NewDust(vector137, 0, 0, 267, 0f, 0f, 127, newColor2, 1f)];
                    dust26.noGravity = true;
                    dust26.position = vector137;
                    dust26.velocity = -Vector2.UnitY * scaleFactor13 * (Main.rand.NextFloat() * 0.9f + 1.6f);
                    dust26.fadeIn = fadeIn;
                    dust26.scale = num981;
                    dust26.noLight = true;
                    Dust dust27 = Dust.CloneDust(dust26);
                    Dust dust = dust27;
                    dust.scale *= 0.65f;
                    dust = dust27;
                    dust.fadeIn *= 0.65f;
                    dust27.color = new Color(255, 255, 255, 255);
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.timeLeft > 90)
                return false;
            return null;
        }

        public override bool CanHitPvp(Player target)
        {
            return projectile.timeLeft <= 90;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Main.player[projectile.owner].Calamity().rollBabSpears(projectile.ai[0] == 0f ? 0 : 10, target.chaseable);
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            Main.player[projectile.owner].Calamity().rollBabSpears(projectile.ai[0] == 0f ? 0 : 10, true);
        }



        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 27);
            Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(3.1415927410125732);
            float num69 = (float)Main.rand.Next(7, 13);
            Vector2 value5 = new Vector2(2.1f, 2f);
            Color newColor = Main.hslToRgb(projectile.ai[0], 1f, 0.5f);
            newColor.A = 255;
            float num72;
            for (float num70 = 0f; num70 < num69; num70 = num72 + 1f)
            {
                int num71 = Dust.NewDust(projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[num71].position = projectile.Center;
                Main.dust[num71].velocity = spinningpoint.RotatedBy((double)(6.28318548f * num70 / num69), default) * value5 * (0.8f + Main.rand.NextFloat() * 0.4f);
                Main.dust[num71].noGravity = true;
                Main.dust[num71].scale = 2f;
                Main.dust[num71].fadeIn = Main.rand.NextFloat() * 2f;
                Dust dust11 = Dust.CloneDust(num71);
                Dust dust = dust11;
                dust.scale /= 2f;
                dust = dust11;
                dust.fadeIn /= 2f;
                dust11.color = new Color(255, 255, 255, 255);
                num72 = num70;
            }
            for (float num73 = 0f; num73 < num69; num73 = num72 + 1f)
            {
                int num74 = Dust.NewDust(projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[num74].position = projectile.Center;
                Main.dust[num74].velocity = spinningpoint.RotatedBy((double)(6.28318548f * num73 / num69), default) * value5 * (0.8f + Main.rand.NextFloat() * 0.4f);
                Dust dust = Main.dust[num74];
                dust.velocity *= Main.rand.NextFloat() * 0.8f;
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat() * 1f;
                dust.fadeIn = Main.rand.NextFloat() * 2f;
                Dust dust12 = Dust.CloneDust(num74);
                dust = dust12;
                dust.scale /= 2f;
                dust = dust12;
                dust.fadeIn /= 2f;
                dust12.color = new Color(255, 255, 255, 255);
                num72 = num73;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
        }
    }

    #endregion

    #region Bab Spears

    public class MiniGuardianSpear : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bab Spear");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.alpha = 100;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 1;
            projectile.minion = true;
            projectile.scale = 0.9f;
        }

        public override void AI()
        {
            int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
            if (projectile.timeLeft == 300)
            {
                projectile.velocity *= 0.01f;
            }
            else if (projectile.timeLeft > 250)
            {
                projectile.velocity *= 1.1f;
            }
            else if (projectile.timeLeft == 250)
            {
                projectile.velocity *= 5f;
            }
            if (projectile.timeLeft <= 250)
            {
                float num535 = projectile.position.X;
                float num536 = projectile.position.Y;
                float num537 = 3000f;
                bool flag19 = false;
                NPC ownerMinionAttackTargetNPC2 = projectile.OwnerMinionAttackTargetNPC;
                if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(projectile, false))
                {
                    float num539 = ownerMinionAttackTargetNPC2.position.X + (float)(ownerMinionAttackTargetNPC2.width / 2);
                    float num540 = ownerMinionAttackTargetNPC2.position.Y + (float)(ownerMinionAttackTargetNPC2.height / 2);
                    float num541 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num539) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num540);
                    if (num541 < num537)
                    {
                        num537 = num541;
                        num535 = num539;
                        num536 = num540;
                        flag19 = true;
                    }
                }
                if (!flag19)
                {
                    for (int num542 = 0; num542 < Main.npc.Length; num542++)
                    {
                        if (Main.npc[num542].CanBeChasedBy(projectile, false))
                        {
                            float num543 = Main.npc[num542].position.X + (float)(Main.npc[num542].width / 2);
                            float num544 = Main.npc[num542].position.Y + (float)(Main.npc[num542].height / 2);
                            float num545 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num543) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num544);
                            if (num545 < num537)
                            {
                                num537 = num545;
                                num535 = num543;
                                num536 = num544;
                                flag19 = true;
                            }
                        }
                    }
                }
                if (flag19)
                {
                    if (projectile.ai[1] == 0f)
                    {
                        float num550 = 24f; //12
                        Vector2 vector43 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                        float num551 = num535 - vector43.X;
                        float num552 = num536 - vector43.Y;
                        float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
                        if (num553 < 100f)
                        {
                            num550 = 28f; //14
                        }
                        num553 = num550 / num553;
                        num551 *= num553;
                        num552 *= num553;
                        projectile.velocity.X = (projectile.velocity.X * 14f + num551) / 15f;
                        projectile.velocity.Y = (projectile.velocity.Y * 14f + num552) / 15f;
                    }
                }
            }
            projectile.rotation = projectile.velocity.ToRotation() + 1.57079637f;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.timeLeft > 250)
                return false;
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 14);
            if (Main.rand.NextBool(3))
            {
                projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
                projectile.width = projectile.height = 200;
                projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
                for (int num621 = 0; num621 < 4; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 12; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;

                }
            }
        }
    }

    #endregion
}
