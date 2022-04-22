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
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        private void Split(bool hit, bool chaseable)
        {
            Player player = Main.player[Projectile.owner];
            bool enrage = player.statLife <= (int)((double)player.statLifeMax2 * 0.5);
            player.Calamity().rollBabSpears(hit ? 1 : 0, chaseable);
            int outerSplits = enrage ? 20 : 10;
            int innerSplits = enrage ? 16 : 8;
            float mult = enrage ? 0.4f : 0.6f;
            if (!hit)
                mult = enrage ? 0.2f : 0.1f; //punishing to miss
            int damage = (int)((Projectile.damage * 0.2f) * mult);

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
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + outerPosVec, velocity, ModContent.ProjectileType<ProfanedCrystalMageFireballSplit>(), damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().forceMinion = true;
                if (innerSplits > 0) //only runs if there's still inner splits to create
                {
                    innerPosVec = innerPosVec.RotatedBy(innerAngleVariance);
                    velocity = new Vector2(innerPosVec.X, innerPosVec.Y).RotatedBy(innerOffsetAngle);
                    velocity.Normalize();
                    velocity *= 5f;
                    proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + innerPosVec, velocity, ModContent.ProjectileType<ProfanedCrystalMageFireballSplit>(), damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().forceMinion = true;
                }
                innerSplits--;
            }

        }

        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 180;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 75;
            Projectile.minion = true;
            Projectile.scale = 0.6f;
        }

        public override bool PreAI()
        {
            Projectile.Calamity().overridesMinionDamagePrevention = true;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            if (Projectile.timeLeft == 50)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.wet && !Projectile.lavaWet && Projectile.timeLeft < 70) //timeleft check because projectile lavawet isn't set until after the first frame
            {
                Projectile.Kill();
            }
            if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
            {
                Projectile.rotation = Projectile.direction;
            }
            if (Projectile.velocity.X < 0f)
            {
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X);
            }
            else
            {
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X);
            }
            return false;
        }

        public override void AI()
        {
            int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 3, texture2D13);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Split(true, target.chaseable);
            }
            Projectile.active = false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Split(true, true);
            }
            Projectile.active = false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Split(false, false);
            }
            SoundEngine.PlaySound(SoundID.Item, (int)Projectile.position.X, (int)Projectile.position.Y, 20);
            for (int num193 = 0; num193 < 6; num193++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 50, default, 1.5f);
            }
            for (int num194 = 0; num194 < 60; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 0, default, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 50, default, 1.5f);
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
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 600;
            Projectile.minion = true;
            Projectile.gfxOffY = -25f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreAI()
        {
            Projectile.Calamity().overridesMinionDamagePrevention = true;
            return true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 5, 1, 0);
            if (Projectile.timeLeft == 600)
            {
                damage = Projectile.damage;
                Projectile.damage = 0;

            }
            if (Projectile.timeLeft > 550)
                Projectile.velocity *= 0.95f;
            int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;


            if (Projectile.timeLeft <= 550)
            {
                if (Projectile.penetrate == -1)
                    Projectile.damage = damage;
                Projectile.penetrate = 1;
                if (Projectile.timeLeft > 500)
                    Projectile.velocity *= 1.06f;
                float num535 = Projectile.position.X;
                float num536 = Projectile.position.Y;
                float num537 = 3000f;
                bool flag19 = false;
                NPC ownerMinionAttackTargetNPC2 = Projectile.OwnerMinionAttackTargetNPC;
                if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(Projectile, false))
                {
                    float num539 = ownerMinionAttackTargetNPC2.position.X + (float)(ownerMinionAttackTargetNPC2.width / 2);
                    float num540 = ownerMinionAttackTargetNPC2.position.Y + (float)(ownerMinionAttackTargetNPC2.height / 2);
                    float num541 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num539) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num540);
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
                        if (Main.npc[num542].CanBeChasedBy(Projectile, false))
                        {
                            float num543 = Main.npc[num542].position.X + (float)(Main.npc[num542].width / 2);
                            float num544 = Main.npc[num542].position.Y + (float)(Main.npc[num542].height / 2);
                            float num545 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num543) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num544);
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
                    if (Projectile.ai[1] == 0f)
                    {
                        float num550 = 24f; //12
                        Vector2 vector43 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
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
                        Projectile.velocity.X = (Projectile.velocity.X * 14f + num551) / 15f;
                        Projectile.velocity.Y = (Projectile.velocity.Y * 14f + num552) / 15f;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[Projectile.owner].Calamity().rollBabSpears(35, target.chaseable);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Main.player[Projectile.owner].Calamity().rollBabSpears(35, true);
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
                if (Projectile.getRect().Intersects(target.getRect()))
                {
                    hits++;
                    if (hits >= 100)
                        Projectile.Kill();
                }
                return false;
            }
            return null;
        }

        public override void Kill(int timeLeft)
        {

            SoundEngine.PlaySound(SoundID.Item, (int)Projectile.position.X, (int)Projectile.position.Y, 14);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = Projectile.height = 200;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int num621 = 0; num621 < 4; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 12; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 50;
            Projectile.minion = true;
            Projectile.alpha = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.06f;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57079637f;
            Lighting.AddLight(Projectile.Center, 5, 1, 0);
        }

        public override bool PreAI()
        {
            Projectile.Calamity().overridesMinionDamagePrevention = true;
            if (Projectile.timeLeft == 50 && Projectile.ai[1] == 1f)
                Projectile.penetrate = 3;
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft > 133 && Projectile.ai[0] < 2f)
                return false;
            return null;
        }

        public override bool CanHitPvp(Player target)
        {
            return Projectile.timeLeft <= 133 || Projectile.ai[0] == 2f;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Projectile.ai[0] == 1f && Projectile.penetrate == 1)
            {
                handleSpecialHit(target.Center);
            }
            int chance = Projectile.ai[0] == 2f ? 20 : 10 * Projectile.penetrate;
            Main.player[Projectile.owner].Calamity().rollBabSpears(chance, target.chaseable);
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (Projectile.ai[0] == 1f && Projectile.penetrate == 1)
            {
                handleSpecialHit(target.Center);
            }
            int chance = Projectile.ai[0] == 2f ? 20 : 10 * Projectile.penetrate;
            Main.player[Projectile.owner].Calamity().rollBabSpears(chance, true);
        }

        private void handleSpecialHit(Vector2 targCenter)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 10; ++i)
                {
                    float startDist = Main.rand.NextFloat(450f, 500f);
                    Vector2 startDir = Main.rand.NextVector2Unit();
                    Vector2 startPoint = targCenter + (startDir * startDist);

                    float speed = Main.rand.NextFloat(15f, 18f);
                    Vector2 velocity = startDir * (-speed);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), startPoint, velocity, ModContent.ProjectileType<ProfanedCrystalMeleeSpear>(), (int)(0.25f * Projectile.damage), 0f, Projectile.owner, 2f, 0f);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft > 133)
            {
                Projectile.localAI[0] += 5f;
                byte b2 = (byte)(((int)Projectile.localAI[0]) * 3);
                byte a2 = (byte)((float)Projectile.alpha * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, Projectile.alpha);
        }

        private void onHit()
        {

            SoundEngine.PlaySound(new LegacySoundStyle(2, 74, Terraria.Audio.SoundType.Sound), (int)Projectile.position.X, (int)Projectile.position.Y);
            if (Main.rand.NextBool(2) || Main.rand.NextBool(3)) //so it's not exactly 1 in 2, but it's not more or less consistently either.
            {
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = Projectile.height = 200;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                for (int num621 = 0; num621 < 4; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 12; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
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
            float num535 = Projectile.position.X;
            float num536 = Projectile.position.Y;
            float num537 = 2000f;
            bool flag19 = false;
            NPC ownerMinionAttackTargetNPC2 = Projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(Projectile, false))
            {
                float num539 = ownerMinionAttackTargetNPC2.position.X + (float)(ownerMinionAttackTargetNPC2.width / 2);
                float num540 = ownerMinionAttackTargetNPC2.position.Y + (float)(ownerMinionAttackTargetNPC2.height / 2);
                float num541 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num539) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num540);
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
                    if (Main.npc[num542].CanBeChasedBy(Projectile, false))
                    {
                        float num543 = Main.npc[num542].position.X + (float)(Main.npc[num542].width / 2);
                        float num544 = Main.npc[num542].position.Y + (float)(Main.npc[num542].height / 2);
                        float num545 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num543) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num544);
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
                Projectile.tileCollide = false;
                if (Projectile.timeLeft < 20)
                {
                    Projectile.timeLeft = 50;
                    kill = true;
                }

                float num550 = 40f;
                Vector2 vector43 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
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
                Projectile.velocity.X = (Projectile.velocity.X * 14f + num551) / 15f;
                Projectile.velocity.Y = (Projectile.velocity.Y * 14f + num552) / 15f;

            }
            else
            {
                if (kill)
                    Projectile.Kill();
                Projectile.tileCollide = true;
            }
        }

        private void swarm(Vector2 targetPos)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                float swarmAmount = Main.rand.Next(6, 10); //6-9
                for (float i = 0; i < swarmAmount; i++)
                {
                    float x = targetPos.X + Main.rand.Next(-500, 501);
                    float y = targetPos.Y - 500 + Main.rand.Next(-200, 1);
                    Vector2 pos = new Vector2(x, y);
                    Vector2 correctedVelocity = Projectile.position - pos;
                    correctedVelocity.Normalize();
                    correctedVelocity *= 25f;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, correctedVelocity, ModContent.ProjectileType<ProfanedCrystalRangedHuges>(), (int)((double)Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner, 2);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().forceMinion = true;
                    ((ProfanedCrystalRangedHuges)Main.projectile[proj].ModProjectile).boomerSwarm = true;
                }
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Bab Boomer");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 175;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool PreAI()
        {
            Projectile.Calamity().overridesMinionDamagePrevention = true;
            bool begin = Projectile.timeLeft == 175;
            if (Projectile.ai[0] >= 1 && begin)
            {
                Projectile.scale = 1.5f;
                Projectile.width += 25;
                Projectile.height += 25;
                boomerSwarm = Projectile.ai[0] == 2;
                if (boomerSwarm && Projectile.ai[1] == 0f)
                {
                    Projectile.timeLeft = 200;
                    Projectile.ai[1] = 1f;
                }
            }
            return true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 175 && Projectile.scale == 1.5f)
                SoundEngine.PlayTrackedSound(SoundID.DD2_BetsyFireballShot, Projectile.Center);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 0;
            }
            if (Projectile.timeLeft == 145 && Projectile.ai[0] < 2f)
            {
                Projectile.tileCollide = true;
            }
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;

            Projectile.velocity *= boomerSwarm ? 1.03f : 1.02f;

            int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
            if (boomerSwarm)
                swarmAI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
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
            Main.player[Projectile.owner].Calamity().rollBabSpears(Projectile.ai[0] == 1f ? 1 : 10, target.chaseable);
            if (Projectile.scale == 1.5f && Projectile.ai[0] != 2f)
                swarm(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Main.player[Projectile.owner].Calamity().rollBabSpears(Projectile.scale == 1.5f ? 1 : 10, true);
            if (Projectile.scale == 1.5f && Projectile.ai[0] != 2f)
                swarm(target.Center);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item, (int)Projectile.position.X, (int)Projectile.position.Y, 14);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = Projectile.height = 200;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int num621 = 0; num621 < 4; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 12; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
            if (Projectile.scale != 1.5f)
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
                    int num626 = Gore.NewGore(new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
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
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.scale = 1.5f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.minion = true;
        }

        public override bool PreAI()
        {
            Projectile.Calamity().overridesMinionDamagePrevention = true;
            return true;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            if (Projectile.ai[0] == 0f) {
            }
            Projectile.velocity.X *= 1.01f;
            Projectile.velocity.Y *= 1.01f;
            if (Projectile.timeLeft == 210)
                Projectile.tileCollide = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1f && Projectile.timeLeft > 210)
                return false;
            return null;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item, (int)Projectile.position.X, (int)Projectile.position.Y, 14);
            if (!Main.rand.NextBool(3)) //1 in 3 chance for dust
                return;
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int num621 = 0; num621 < 10; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 15; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 247, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[Projectile.owner].Calamity().rollBabSpears(50, target.chaseable);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Main.player[Projectile.owner].Calamity().rollBabSpears(50, true);
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
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 0.8f;
        }

        private void ai()
        {
            if (Projectile.timeLeft > 90)
            {
                Projectile.rotation += 1f;
                Projectile.velocity.X *= 0.985f;
                Projectile.velocity.Y *= 0.985f;
            }
            else
            {
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 2.355f;
                if (Projectile.spriteDirection == -1)
                {
                    Projectile.rotation -= 1.57f;
                }
                float num535 = Projectile.position.X;
                float num536 = Projectile.position.Y;
                float num537 = 2000f;
                bool flag19 = false;
                NPC ownerMinionAttackTargetNPC2 = Projectile.OwnerMinionAttackTargetNPC;
                if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(Projectile, false))
                {
                    float num539 = ownerMinionAttackTargetNPC2.position.X + (float)(ownerMinionAttackTargetNPC2.width / 2);
                    float num540 = ownerMinionAttackTargetNPC2.position.Y + (float)(ownerMinionAttackTargetNPC2.height / 2);
                    float num541 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num539) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num540);
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
                        if (Main.npc[num542].CanBeChasedBy(Projectile, false))
                        {
                            float num543 = Main.npc[num542].position.X + (float)(Main.npc[num542].width / 2);
                            float num544 = Main.npc[num542].position.Y + (float)(Main.npc[num542].height / 2);
                            float num545 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num543) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num544);
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
                        Projectile.timeLeft++;
                    Projectile.tileCollide = false;
                    float num550 = 40f;
                    Vector2 vector43 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
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
                    Projectile.velocity.X = (Projectile.velocity.X * 14f + num551) / 15f;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 14f + num552) / 15f;

                }
                else
                {
                    Projectile.tileCollide = true;
                }
            }

        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override bool PreAI()
        {
            Projectile.Calamity().overridesMinionDamagePrevention = true;
            return true;
        }

        public override void AI()
        {
            ai();
            Color newColor2 = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f);
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 8;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.alpha == 0)
            {
                Lighting.AddLight(Projectile.Center, newColor2.ToVector3() * 0.5f);
            }
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) - 1.57f;
            int num3;
            for (int num979 = 0; num979 < 2; num979 = num3 + 1)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value55 = Vector2.UnitY.RotatedBy((double)((float)num979 * 3.14159274f), default).RotatedBy((double)Projectile.rotation, default);
                    Dust dust24 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    dust24.noGravity = true;
                    dust24.noLight = true;
                    dust24.scale = Projectile.Opacity * Projectile.localAI[0];
                    dust24.position = Projectile.Center;
                    dust24.velocity = value55 * 2.5f;
                }
                num3 = num979;
            }
            for (int num980 = 0; num980 < 2; num980 = num3 + 1)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value56 = Vector2.UnitY.RotatedBy((double)((float)num980 * 3.14159274f), default);
                    Dust dust25 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    dust25.noGravity = true;
                    dust25.noLight = true;
                    dust25.scale = Projectile.Opacity * Projectile.localAI[0];
                    dust25.position = Projectile.Center;
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
                Vector2 vector137 = Projectile.Center + vector136;
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
            if (Projectile.timeLeft > 90)
                return false;
            return null;
        }

        public override bool CanHitPvp(Player target)
        {
            return Projectile.timeLeft <= 90;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Main.player[Projectile.owner].Calamity().rollBabSpears(Projectile.ai[0] == 0f ? 0 : 10, target.chaseable);
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            Main.player[Projectile.owner].Calamity().rollBabSpears(Projectile.ai[0] == 0f ? 0 : 10, true);
        }



        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item, (int)Projectile.position.X, (int)Projectile.position.Y, 27);
            Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(3.1415927410125732);
            float num69 = (float)Main.rand.Next(7, 13);
            Vector2 value5 = new Vector2(2.1f, 2f);
            Color newColor = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f);
            newColor.A = 255;
            float num72;
            for (float num70 = 0f; num70 < num69; num70 = num72 + 1f)
            {
                int num71 = Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[num71].position = Projectile.Center;
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
                int num74 = Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[num74].position = Projectile.Center;
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
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
        }
    }

    #endregion

    #region Bab Spears

    public class MiniGuardianSpear : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bab Spear");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.alpha = 100;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.minion = true;
            Projectile.scale = 0.9f;
        }

        public override void AI()
        {
            int num469 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
            if (Projectile.timeLeft == 300)
            {
                Projectile.velocity *= 0.01f;
            }
            else if (Projectile.timeLeft > 250)
            {
                Projectile.velocity *= 1.1f;
            }
            else if (Projectile.timeLeft == 250)
            {
                Projectile.velocity *= 5f;
            }
            if (Projectile.timeLeft <= 250)
            {
                float num535 = Projectile.position.X;
                float num536 = Projectile.position.Y;
                float num537 = 3000f;
                bool flag19 = false;
                NPC ownerMinionAttackTargetNPC2 = Projectile.OwnerMinionAttackTargetNPC;
                if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(Projectile, false))
                {
                    float num539 = ownerMinionAttackTargetNPC2.position.X + (float)(ownerMinionAttackTargetNPC2.width / 2);
                    float num540 = ownerMinionAttackTargetNPC2.position.Y + (float)(ownerMinionAttackTargetNPC2.height / 2);
                    float num541 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num539) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num540);
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
                        if (Main.npc[num542].CanBeChasedBy(Projectile, false))
                        {
                            float num543 = Main.npc[num542].position.X + (float)(Main.npc[num542].width / 2);
                            float num544 = Main.npc[num542].position.Y + (float)(Main.npc[num542].height / 2);
                            float num545 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num543) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num544);
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
                    if (Projectile.ai[1] == 0f)
                    {
                        float num550 = 24f; //12
                        Vector2 vector43 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
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
                        Projectile.velocity.X = (Projectile.velocity.X * 14f + num551) / 15f;
                        Projectile.velocity.Y = (Projectile.velocity.Y * 14f + num552) / 15f;
                    }
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57079637f;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft > 250)
                return false;
            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item, (int)Projectile.position.X, (int)Projectile.position.Y, 14);
            if (Main.rand.NextBool(3))
            {
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = Projectile.height = 200;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                for (int num621 = 0; num621 < 4; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 12; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;

                }
            }
        }
    }

    #endregion
}
