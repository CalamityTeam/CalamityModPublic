using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.NPCs.SupremeCalamitas;
using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Buffs.Summon.Whips;
using static CalamityMod.Items.Accessories.ProfanedSoulCrystal;
using CalamityMod.NPCs.Providence;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace CalamityMod.Projectiles.Summon
{
    #region Mage Projectiles

    #region Main Fireball
    public class ProfanedCrystalMageFireball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Boss/HolyBlast";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        private void Split(bool hit, bool chaseable)
        {
            Player player = Main.player[Projectile.owner];
            bool enrage = player.Calamity().pscState >= (int)ProfanedSoulCrystalState.Empowered;
            player.Calamity().rollBabSpears(hit ? 1 : 0, chaseable);
            int outerSplits = enrage ? 16 : 10;
            int innerSplits = enrage ? 12 : 8;
            float mult = enrage ? 0.3f : 0.6f;
            if (!hit)
                mult = enrage ? 0.2f : 0.1f; //punishing to miss
            int damage = (int)((Projectile.damage * 0.2f) * mult);
            int origDmg = (int)((Projectile.originalDamage * 0.2f) * mult);

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
                {
                    Main.projectile[proj].originalDamage = origDmg;
                    Main.projectile[proj].DamageType = DamageClass.Summon;
                }
                if (innerSplits > 0) //only runs if there's still inner splits to create
                {
                    innerPosVec = innerPosVec.RotatedBy(innerAngleVariance);
                    velocity = new Vector2(innerPosVec.X, innerPosVec.Y).RotatedBy(innerOffsetAngle);
                    velocity.Normalize();
                    velocity *= 5f;
                    proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + innerPosVec, velocity, ModContent.ProjectileType<ProfanedCrystalMageFireballSplit>(), damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].originalDamage = origDmg;
                        Main.projectile[proj].DamageType = DamageClass.Summon;
                    }
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
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            int dustID = ProvUtils.GetDustID(pscState);
            int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, dustID, 0f, 0f, 100, default, 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
        }
        
        public override Color? GetAlpha(Color lightColor)
        {
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            return ProvUtils.GetProjectileColor(pscState, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Main.dayTime ? ModContent.Request<Texture2D>(Texture).Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyBlastNight").Value;
            int num214 = texture.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(pscState, Projectile.alpha, true), 4f, texture);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Split(true, target.chaseable);
            }
            Projectile.active = false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            if (Main.myPlayer == Projectile.owner)
            {
                Split(true, true);
            }
            Projectile.active = false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner && Projectile.ai[1] == 0f)
            {
                Split(false, false);
            }
            SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            int dustID = ProvUtils.GetDustID(pscState);
            for (int num193 = 0; num193 < 6; num193++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustID, 0f, 0f, 50, default, 1.5f);
            }
            for (int num194 = 0; num194 < 60; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustID, 0f, 0f, 0, default, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustID, 0f, 0f, 50, default, 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }
    }
    #endregion

    #region Fireball Split
    public class ProfanedCrystalMageFireballSplit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Boss/HolyFire2";
        private int damage;
        private int hits = 0;
        private NPC target = null;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.SummonTagDamageMultiplier[Type] = 0.3f;
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
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool PreAI()
        {
            Projectile.Calamity().overridesMinionDamagePrevention = true;
            return true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.1f, 0f);
            if (Projectile.timeLeft == 600)
            {
                damage = Projectile.damage;
                Projectile.damage = 0;

            }
            if (Projectile.timeLeft > 550)
                Projectile.velocity *= 0.95f;
            int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, ProvUtils.GetDustID((float)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night)), 0f, 0f, 100, default, Main.dayTime ? 1f : 0.75f);
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
        
        public override Color? GetAlpha(Color lightColor)
        {
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            return ProvUtils.GetProjectileColor(pscState, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            Texture2D texture = Main.dayTime ? ModContent.Request<Texture2D>(Texture).Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyFire2Night").Value;
            int num214 = texture.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(pscState, Projectile.alpha, true), 4f, texture);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].Calamity().rollBabSpears(35, target.chaseable);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Main.player[Projectile.owner].Calamity().rollBabSpears(35, true);
        }

        public override bool? CanHitNPC(NPC target)
        {
            bool instakill = target.type == ModContent.NPCType<SepulcherHead>() || target.type == ModContent.NPCType<SepulcherBody>() || target.type == ModContent.NPCType<SepulcherBodyEnergyBall>() || target.type == ModContent.NPCType<SepulcherTail>();
            if (!instakill && this.target != null && target != this.target)
            {
                if (Projectile.getRect().Intersects(target.getRect()))
                {
                    hits++;
                    if (hits >= 25)
                        Projectile.Kill();
                    
                }
                return false;
            }
            return null;
        }

        public override void OnKill(int timeLeft)
        {

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = Projectile.height = 200;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            int dust = ProvUtils.GetDustID(pscState);
            for (int num621 = 0; num621 < 4; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 2f : 0.5f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 12; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 3f : 0.75f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 2f : 0.5f);
                Main.dust[num624].velocity *= 2f;

            }
        }
    }
    #endregion

    #endregion

    #region Melee Projectile
    public class ProfanedCrystalMeleeSpear : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public override string Texture => "CalamityMod/Projectiles/Boss/HolySpear";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.SummonTagDamageMultiplier[Type] = 0.6f;
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
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.06f;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57079637f;
            Lighting.AddLight(Projectile.Center, 1f, 0.2f, 0f);
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


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] == 1f && Projectile.penetrate == 1)
            {
                handleSpecialHit(target.Center);
            }
            int chance = Projectile.ai[0] == 2f ? 20 : 10 * Projectile.penetrate;
            Main.player[Projectile.owner].Calamity().rollBabSpears(chance, target.chaseable);
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
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(pscState, Projectile.alpha, true), 4f, ModContent.Request<Texture2D>(Texture).Value);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], ProvUtils.GetProjectileColor(pscState, Projectile.alpha), 1);
            return false;
        }

        private void onHit()
        {

            SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
            if (Main.rand.NextBool() || Main.rand.NextBool(3)) //so it's not exactly 1 in 2, but it's not more or less consistently either.
            {
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = Projectile.height = 200;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
                int dust = ProvUtils.GetDustID(pscState);
                for (int num621 = 0; num621 < 4; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 2f : 0.5f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 12; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 3f : 0.75f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 2f : 0.5f);
                    Main.dust[num624].velocity *= 2f;

                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            onHit();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            onHit();
        }
    }
    #endregion

    #region Ranger Projectiles

    #region Meteors
    public class ProfanedCrystalRangedHuges : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
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
                    {
                        Main.projectile[proj].originalDamage = (int)(Projectile.originalDamage * 1.5);
                        Main.projectile[proj].DamageType = DamageClass.Summon;
                    }
                    ((ProfanedCrystalRangedHuges)Main.projectile[proj].ModProjectile).boomerSwarm = true;
                }
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.SummonTagDamageMultiplier[Type] = 0.4f;
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
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.Center);

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
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            int dust = ProvUtils.GetDustID(pscState);
            int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
            if (boomerSwarm)
                swarmAI();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            return ProvUtils.GetProjectileColor(pscState, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Main.dayTime ? ModContent.Request<Texture2D>(Texture).Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/ProfanedCrystalRangedHugesNight").Value;
            int num214 = texture.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(pscState, Projectile.alpha, true), 4f, texture);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            bool instakill = target.type == ModContent.NPCType<SepulcherHead>() || target.type == ModContent.NPCType<SepulcherBody>() || target.type == ModContent.NPCType<SepulcherBodyEnergyBall>() || target.type == ModContent.NPCType<SepulcherTail>();
            if (!instakill && this.target != null && target != this.target)
                return false;
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].Calamity().rollBabSpears(Projectile.ai[0] == 1f ? 1 : 10, target.chaseable);
            if (Projectile.scale == 1.5f && Projectile.ai[0] != 2f)
                swarm(target.Center);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Main.player[Projectile.owner].Calamity().rollBabSpears(Projectile.scale == 1.5f ? 1 : 10, true);
            if (Projectile.scale == 1.5f && Projectile.ai[0] != 2f)
                swarm(target.Center);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = Projectile.height = 200;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            int dust = ProvUtils.GetDustID(pscState);
            for (int num621 = 0; num621 < 4; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 2f : 0.5f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 12; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 3f : 0.75f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 2f : 0.5f);
                Main.dust[num624].velocity *= 2f;
            }

            if (Projectile.scale != 1.5f)
                return;

            if (Main.netMode != NetmodeID.Server)
            {
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
                        int num626 = Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 0.75f);
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
    }
    #endregion

    #region Fireballs

    public class ProfanedCrystalRangedSmalls : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
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
            Projectile.DamageType = DamageClass.Summon;
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

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            if (!Main.rand.NextBool(3)) //1 in 3 chance for dust
                return;
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            int dust = ProvUtils.GetDustID(pscState);
            for (int num621 = 0; num621 < 10; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 2f : 0.5f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 15; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 3f : 0.75f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 2f : 0.5f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].Calamity().rollBabSpears(50, target.chaseable);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Main.player[Projectile.owner].Calamity().rollBabSpears(50, true);
        }
        
        public override Color? GetAlpha(Color lightColor)
        {
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            return ProvUtils.GetProjectileColor(pscState, Projectile.alpha);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = texture.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(pscState, Projectile.alpha, true), 4f, texture);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    #endregion

    #endregion

    #region Rogue Projectile

    public class ProfanedCrystalRogueShard : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SummonTagDamageMultiplier[Type] = 0.25f;
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
            Projectile.DamageType = DamageClass.Summon;
        }

        private void ai()
        {
            if (Projectile.timeLeft > 120)
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
                    for (int num542 = 0; num542 < Main.maxNPCs; num542 = num3 + 1)
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
                    Projectile.velocity *= 1.05f;
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
                    Dust dust24 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1f)];
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
                    Dust dust25 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1f)];
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
            if (Projectile.timeLeft > 120)
                return false;
            return null;
        }

        public override bool CanHitPvp(Player target)
        {
            return Projectile.timeLeft <= 120;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            var calPlayer = Main.player[Projectile.owner].Calamity();
            var empowered = calPlayer.pscState == (int)ProfanedSoulCrystalState.Empowered;
            calPlayer.rollBabSpears(Projectile.ai[0] == 0f ? 0 : empowered ? 30 : 10, target.chaseable); //empowered fires three times as many projectiles, keeps things consistent and easier to balance
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
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
    
    #region Whip

    public class ProfanedCrystalWhip : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        private Color specialColor = Color.Orange;
        public Color SpecialDrawColor => specialColor;

        public override void SetStaticDefaults() {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.Segments = 20;
            Projectile.WhipSettings.RangeMultiplier = 2.25f;
        }

        public override bool PreAI()
        {
            ExtraBehavior();
            return true;
        }

        public void ExtraBehavior()
        {
            var player = Main.player[Projectile.owner];
            specialColor = GetColorForPsc(Main.dayTime ? (int)ProfanedSoulCrystalState.Vanity : player.Calamity().pscState, Main.dayTime);
        }
        
        private float Timer {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.ai[1] == 0f)
            {
                owner.itemAnimation = owner.itemAnimationMax;
                owner.ApplyItemAnimation(owner.HeldItem);
                Projectile.ai[1] = 1f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;
            Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

            float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
            if (Timer >= swingTime) {
                Projectile.Kill();
                return;
            }

            owner.heldProj = Projectile.whoAmI;
            if (Timer == swingTime / 2) {
                List<Vector2> points = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, points);
                SoundEngine.PlaySound(SoundID.Item153, points[points.Count - 1]);
            }
            
            float swingProgress = Timer / swingTime;
            if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f && !Main.rand.NextBool(3)) {
                List<Vector2> points = Projectile.WhipPointsForCollision;
                points.Clear();
                Projectile.FillWhipControlPoints(Projectile, points);
                int pointIndex = points.Count - 1;
                Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));
                int dustType = DustID.VenomStaff;
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, dustType, 0f, 0f, 100, Color.White);
                    dust.position = points[pointIndex];
                    dust.fadeIn = 0.3f;
                    dust.scale = 2f;
                    Vector2 spinningpoint = points[pointIndex] - points[pointIndex - 1];
                    dust.noGravity = true;
                    dust.velocity *= 0.5f;
                    dust.velocity += spinningpoint.RotatedBy(owner.direction * ((float)Math.PI / 2f));
                    dust.velocity *= 0.5f;
                }
            }
        }
        
        private void DrawLine(List<Vector2> list) {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 0);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 2; i++) {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Vector2 scale = new Vector2(1, (diff.Length()) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, SpecialDrawColor, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }
        public override bool PreDraw(ref Color lightColor) {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);

            SpriteEffects flip = SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++) {
                Rectangle frame = new Rectangle(0, 0, 16, 22); // The size of the Handle (measured in pixels)
                Vector2 origin = new Vector2(5, 8); // Offset for where the player's hand will start measured from the top left of the image.
                float scale = 1;
                
                if (i == list.Count - 2) {
                    frame.Y = 126; // Distance from the top of the sprite to the start of the frame.
                    frame.Height = 34; // Height of the frame.
                    
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 6) {
                    // Third segment
                    frame.Y = 102;
                    frame.Height = 18;
                }
                else if (i > 3) {
                    // Second Segment
                    frame.Y = 70;
                    frame.Height = 18;
                }
                else if (i > 0) {
                    // First Segment
                    frame.Y = 38;
                    frame.Height = 18;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, Color.White, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int buffTime = CalamityUtils.SecondsToFrames(30); //30 second debuff to allow for time to swap and use other weapons
            target.AddBuff(ModContent.BuffType<ProfanedCrystalWhipDebuff>(), buffTime); 
            Main.player[Projectile.owner].AddBuff(ModContent.BuffType<ProfanedCrystalWhipBuff>(), buffTime);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * 0.7f); // multihit penalty
        }
    }
    
    #endregion
    
    #region Animation Projectiles
    
    public class PscTransformAnimation : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.NoLiquidDistortion[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = maxPscAnimTime;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.3f, 0.225f, 0f);

            var owner = Main.player[Projectile.owner];
            owner.Calamity().profanedCrystalAnim = Projectile.timeLeft;
            
            Projectile.Center = owner.Center;

            if (!owner.Calamity().profanedCrystal)
            {

                owner.Calamity().profanedCrystalAnim = -1;
                Projectile.active = false;
                return;
            }
            
            if (Projectile.timeLeft > 1)
            {
                int dustCount = (int)Math.Round(MathHelper.SmoothStep(1f, 3f, ((float)maxPscAnimTime - (float)owner.Calamity().profanedCrystalAnim) / (float)maxPscAnimTime));
                float outwardness = MathHelper.SmoothStep(40f, 75f, ((float)maxPscAnimTime - (float)owner.Calamity().profanedCrystalAnim) / (float)maxPscAnimTime);
                float dustScale = MathHelper.Lerp(0.45f, 1f, ((float)maxPscAnimTime - (float)owner.Calamity().profanedCrystalAnim) / (float)maxPscAnimTime);
                int[] validRockTypes = new int[] { 1, 3, 4, 5, 6 };
                int projectileCount = owner.ownedProjectileCounts[ModContent.ProjectileType<PscTransformRocks>()];
                bool shouldStickAround = projectileCount <= 20;
                for (int i = 0; i < dustCount; i++)
                {
                    Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Unit() * outwardness * Main.rand.NextFloat(0.75f, 1.1f);
                    Vector2 dustVelocity = (Projectile.Center - spawnPosition) * 0.085f + owner.velocity;
                    
                    int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
                    
                    Dust dust = Dust.NewDustPerfect(spawnPosition, ProvUtils.GetDustID(pscState));
                    dust.velocity = dustVelocity;
                    dust.scale = dustScale * Main.rand.NextFloat(0.75f, 1.15f);
                    dust.noGravity = true;
                    dust.noLight = true;

                    if (Projectile.timeLeft % 3 == 0)
                    {
                        if (!Main.dedServ)
                        {
                            var startVec = Main.rand.NextVector2CircularEdge(250f, 250f);
                            var finalDist = Main.rand.NextFloat(50f, 50f);
                            var startColor = Main.dayTime ? Color.Orange : Color.Aquamarine;
                            Particle wtfIsAParticle = new ManaDrainStreak(owner, Main.rand.NextFloat(0.3f, 0.6f), startVec, finalDist, startColor, startColor * 1.5f, Main.rand.Next(20, 31), owner.Center);
                            GeneralParticleHandler.SpawnParticle(wtfIsAParticle);
                        }

                        if (owner.whoAmI == Main.myPlayer)
                        {
                            spawnPosition = owner.Center;
                            spawnPosition.X += Main.rand.NextFloat(-500f, 500f);
                            spawnPosition.Y += Main.rand.NextFloat(-500f, 500f);
                            dustVelocity = (Projectile.Center - spawnPosition) * 0.085f + owner.velocity;
                            dustVelocity.Normalize();
                            dustVelocity *= 16f;
                            int rockType = validRockTypes[Main.rand.Next(0, validRockTypes.Length)];
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, dustVelocity, ModContent.ProjectileType<PscTransformRocks>(), 0, 0f, Projectile.owner, shouldStickAround ? 1f : 0f, rockType);
                        }
                    }
                }
            }
            else
            {
                owner.Calamity().profanedCrystalAnim = -1;
                owner.Calamity().GeneralScreenShakePower = 5f;
                DetermineTransformationEligibility(owner);
                if (!Main.dedServ)
                {
                    var color = GetColorForPsc(owner.Calamity().pscState, Main.dayTime) with {A = 255}; //WHY THE FUCK IS THE ALPHA INVERTED FOR THIS
                    Particle wtfIsAParticle = new DirectionalPulseRing(owner.Center, Vector2.Zero, color, Vector2.One, 0f, 0f, 2.5f, 75);
                    GeneralParticleHandler.SpawnParticle(wtfIsAParticle);
                }
                OnKill(1);
            }
        }

        public override bool? CanDamage() => false;
        

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(Providence.SpawnSound, Projectile.position);
            var Owner = Main.player[Projectile.owner];
            for (int i = 0; i < 20; i++)
            {
                Vector2 dustPos = new Vector2(Owner.Center.X + Main.rand.NextFloat(-10, 10), Owner.Center.Y + Main.rand.NextFloat(-10, 10));
                Vector2 velocity = (Owner.Center - dustPos).SafeNormalize(Vector2.Zero);
                velocity *= Main.dayTime ? 3f : 6.9f;
                var dust = Dust.NewDustPerfect(Owner.Center, ProvUtils.GetDustID((float)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night)), velocity, 0, default(Color), 2f);
                if (!Main.dayTime)
                    dust.noGravity = true;
            }

            Projectile.active = false;
        }
    }
        
    public class PscTransformRocks : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/Typeless/ArtifactOfResilienceShard1";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.NoLiquidDistortion[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = maxPscAnimTime;
            Projectile.alpha = 0;
            Projectile.hide = true;
            Projectile.scale = 0.1f;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            //Ah yes, overriding drawbehind, to draw in front of the player, flawless logic
            overPlayers.Add(index);
        }

        public override void AI()
        {
            var owner = Main.player[Projectile.owner];

            if (!owner.Calamity().profanedCrystal)
            {
                Projectile.active = false;
                return;
            }
            //scale up the proj
            if (Projectile.scale < 1.25f)
                Projectile.scale += 0.1f;
            
            //if it's not allowed to stick around, fade into oblivion
            if ((Projectile.velocity.Length() <= 3f && Projectile.ai[0] == 0f) || Projectile.timeLeft <= 25)
                Projectile.alpha += Projectile.ai[0] == 0f ? 20 : 10;

            //if the anim has finished, fade, regardless of where the projectile is
            if (owner.Calamity().profanedCrystalAnim == -1)
                Projectile.alpha += 15;
            
            //kill any projectiles that are fully faded
            if (Projectile.alpha >= 255)
                Projectile.active = false;
            
            //slow down upon reaching the player
            if (Projectile.Hitbox.Intersects(owner.Hitbox))
                Projectile.velocity *= 0.1f;
            else
            {
                var target = owner.Center - Projectile.Center;
                target.Normalize();
                Projectile.velocity = target * 18f;
            }

        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            int rockType = (int)MathHelper.Clamp(Projectile.ai[1], 1f, 6f);
            Texture2D texture = ModContent.Request<Texture2D>(Texture[..^1] + rockType.ToString()).Value;
            
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            drawPos -= new Vector2(texture.Width, texture.Height) * Projectile.scale / 2f;
            drawPos += drawOrigin * Projectile.scale + new Vector2(0f, Projectile.gfxOffY);
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            if (CalamityConfig.Instance.Afterimages)  //handle afterimages manually since the utility broke it and didn't render correctly
            {
                for (int i = 0; i < Projectile.oldPos.Length; ++i)
                {
                    drawPos = Projectile.oldPos[i] + (Projectile.Size / 2f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                    // DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS. THIS WILL BREAK THE AFTERIMAGES.
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
                }
            }
            else
            {
                Main.EntitySpriteDraw(texture, drawPos, frame, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
        
    
    #endregion

    #region Bab Projectiles

    #region Bab Spears
    public class MiniGuardianSpear : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Boss/HolySpear";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 2;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.SummonTagDamageMultiplier[Type] = 0.5f;
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
            Projectile.DamageType = DamageClass.Summon;
        }

        private void handleAI(bool miniGuardianPscAttack)
        {
            if (!miniGuardianPscAttack)
            {
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
            }
        }

        public override void AI()
        {
            var psc = Projectile.ai[0] > 0f;
            //Ensure that psa's spears are not coloured by night
            int pscState = (int)((!Main.dayTime && psc) ? Providence.BossMode.Night : Providence.BossMode.Day);
            int num469 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ProvUtils.GetDustID(pscState), 0f, 0f, 100, default, !Main.dayTime && psc ? 0.5f : 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
            handleAI(psc && Projectile.ai[1] > 0f);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57079637f;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[1] == 0f && Projectile.timeLeft > 250)
                return false;
            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var psc = Projectile.ai[0] > 0f;
            int pscState = (int)((!Main.dayTime && psc) ? Providence.BossMode.Night : Providence.BossMode.Day);
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(pscState, Projectile.alpha, true), 4f, ModContent.Request<Texture2D>(Texture).Value);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], ProvUtils.GetProjectileColor(pscState, Projectile.alpha), 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            if (Main.rand.NextBool(3))
            {
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = Projectile.height = 200;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                var psc = Projectile.ai[0] > 0f;
                var shouldAdjust = !Main.dayTime && psc;
                int pscState = (int)(shouldAdjust ? Providence.BossMode.Night : Providence.BossMode.Day);
                int dustID = ProvUtils.GetDustID(pscState);
                for (int num621 = 0; num621 < 4; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustID, 0f, 0f, 100, default, !Main.dayTime && psc ? 0.5f : 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = shouldAdjust ? 0.9f : 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < (shouldAdjust ? 8 : 12); num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustID, 0f, 0f, 100, default, !Main.dayTime && psc ? 1.25f : 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    Main.dust[num624].fadeIn = shouldAdjust ? 0.9f : Main.dust[num624].fadeIn;
                    num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustID, 0f, 0f, 100, default, !Main.dayTime && psc ? 1f : 2f);
                    Main.dust[num624].velocity *= 2f;
                    Main.dust[num624].fadeIn = shouldAdjust ? 0.9f : Main.dust[num624].fadeIn;
                }
            }
        }
    }
    #endregion

    #region Bab Fireball

    public class MiniGuardianFireball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Boss/HolyBlast";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        private void Split()
        {
            int totalProjectiles = Projectile.ai[0] == 0f ? 8 : 4;

            float radians = MathHelper.TwoPi / totalProjectiles;
            int type = ModContent.ProjectileType<MiniGuardianFireballSplit>();
            float velocity = 5f;
            Vector2 spinningPoint = new Vector2(0f, -velocity);
            for (int k = 0; k < totalProjectiles; k++)
            {
                Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity2 + Projectile.velocity * 0.25f, type, (int)Math.Round(Projectile.originalDamage * 0.75), 0f, Projectile.owner, 1f);
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
            Projectile.scale = 0.4f;
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
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, ProvUtils.GetDustID(pscState), 0f, 0f, 100, default, 1f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
        }
        
        public override Color? GetAlpha(Color lightColor)
        {
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            return ProvUtils.GetProjectileColor(pscState, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Main.dayTime ? ModContent.Request<Texture2D>(Texture).Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyBlastNight").Value;
            int num214 = texture.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(pscState, Projectile.alpha, true), 4f, texture);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Split();
            }
            SoundEngine.PlaySound(HolyBlast.ImpactSound, Projectile.Center);
            Projectile.active = false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            if (Main.myPlayer == Projectile.owner)
            {
                Split();
            }
            SoundEngine.PlaySound(HolyBlast.ImpactSound, Projectile.Center);
            Projectile.active = false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Split();
            }
            SoundEngine.PlaySound(HolyBlast.ImpactSound, Projectile.Center);
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            int dustID = ProvUtils.GetDustID(pscState);
            for (int num193 = 0; num193 < 6; num193++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustID, 0f, 0f, 50, default, Main.dayTime ? 1.5f : 0.5f);
            }
            for (int num194 = 0; num194 < 60; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustID, 0f, 0f, 0, default, Main.dayTime ? 2.5f : 0.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustID, 0f, 0f, 50, default, Main.dayTime ? 1.5f : 0.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }
    }
    
    #endregion

    #region Bab Fireball Split
    
    
    public class MiniGuardianFireballSplit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Boss/HolyFire2";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.SummonTagDamageMultiplier[Type] = 0.3f;
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
            Projectile.timeLeft = 300;
            Projectile.minion = true;
            Projectile.gfxOffY = -25f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool PreAI()
        {
            Projectile.Calamity().overridesMinionDamagePrevention = true;
            return true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.3f, 0.225f, 0f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            if (Math.Abs(Projectile.velocity.X) < 8f)
                Projectile.velocity.X *= 1.05f;

            NPC target = Projectile.Center.MinionHoming(2000f, Main.player[Projectile.owner], true);
            
            if (target != null)
            {
                float scaleFactor2 = Projectile.velocity.Length();
                Vector2 vector11 = target.Center - Projectile.Center;
                vector11.Normalize();
                vector11 *= scaleFactor2;
                float inertia = 15f;
                Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vector11) / inertia;
                Projectile.velocity.Normalize();
                Projectile.velocity *= scaleFactor2;
            }

            Projectile.rotation = Projectile.velocity.X * 0.025f;
        }
        
        public override Color? GetAlpha(Color lightColor)
        {
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            return ProvUtils.GetProjectileColor(pscState, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            Texture2D texture = Main.dayTime ? ModContent.Request<Texture2D>(Texture).Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyFire2Night").Value;
            int num214 = texture.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(pscState, Projectile.alpha, true), 4f, texture);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = Projectile.height = 200;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            int dust = ProvUtils.GetDustID(pscState);
            
            for (int num621 = 0; num621 < 4; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 2f : 0.5f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 12; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 3f : 0.5f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, 0f, 0f, 100, default, Main.dayTime ? 2f : 0.5f);
                Main.dust[num624].velocity *= 2f;

            }
        }
    }
    
    #endregion
    
    #endregion
}
