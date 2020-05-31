using CalamityMod.Buffs.Summon;
using CalamityMod.Dusts;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class GammaHead : ModProjectile
    {
        public Vector2 DeltaPosition;
        public int BulletShootCounter;
        public float AngularMultiplier1;
        public float AngularMultiplier2;
        public const int MaximumLaserCount = 6; // Otherwise intense lag ensues.
        public const float AttackStartWait = 30f;
        public const float SuperchargeTimeMax = 540f;
        public const float DistanceToCheck = 1000f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamma Head");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 36;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
			projectile.coldDamage = true;
        }
        public Vector2 DrawStartPosition
        {
            get
            {
                if (projectile.owner < 0 || projectile.owner >= Main.player.Length)
                    return Vector2.Zero;
                return Main.player[projectile.owner].Top + Vector2.UnitY * 8f;
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BulletShootCounter = reader.ReadInt32();
            AngularMultiplier1 = reader.ReadSingle();
            AngularMultiplier2 = reader.ReadSingle();
            DeltaPosition = reader.ReadVector2();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(BulletShootCounter);
            writer.Write(AngularMultiplier1);
            writer.Write(AngularMultiplier2);
            writer.WriteVector2(DeltaPosition);
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (projectile.localAI[0] == 0f)
            {
                int totalHeads = CalamityUtils.CountProjectiles(projectile.type);
                DeltaPosition = new Vector2(Main.rand.NextFloat(-42f - 6f * totalHeads, 42f + 6f * totalHeads), -Main.rand.NextFloat(8f, 64f + 7f * totalHeads));

                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;

                AngularMultiplier1 = Main.rand.NextFloat(3f);
                AngularMultiplier2 = Main.rand.NextFloat(3f);
                projectile.netUpdate = true;
                projectile.localAI[0] = 1f;
			}

            bool isProperProjectile = projectile.type == ModContent.ProjectileType<GammaHead>();
            player.AddBuff(ModContent.BuffType<GammaHeadBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.gammaHead = false;
                }
                if (modPlayer.gammaHead)
                {
                    projectile.timeLeft = 2;
                }
            }

            if (modPlayer.GammaCanisters.Count > 0)
            {
                for (int i = 0; i < modPlayer.GammaCanisters.Count; i++)
                {
                    if (projectile.Hitbox.Intersects(Main.projectile[modPlayer.GammaCanisters[i]].Hitbox) &&
                        Main.projectile[modPlayer.GammaCanisters[i]].active)
                    {
                        int laserCount = 0;
                        for (int j = 0; j < Main.projectile.Length; j++)
                        {
                            if (laserCount < MaximumLaserCount)
                            {
                                if (Main.projectile[j].type == projectile.type && Main.projectile[j].owner == projectile.owner &&
                                    Main.projectile[j].ai[0] == 0f && Main.projectile[j].active && Main.projectile[j].ai[1] <= 0)
                                {
                                    Main.projectile[j].ai[0] = Projectile.NewProjectile(projectile.Center, Vector2.UnitY, ModContent.ProjectileType<GammaDeathray>(),
                                                                                (int)(projectile.damage * 1.2f), projectile.knockBack, projectile.owner, Main.projectile[j].whoAmI);

                                    Main.projectile[j].ai[1] = SuperchargeTimeMax;
                                    Main.projectile[j].netUpdate = true;
                                    laserCount++;
                                }
                            }
                            else
                            {
                                Main.projectile[j].ai[1] = SuperchargeTimeMax;
                                Main.projectile[j].netUpdate = true;
                            }
                        }

                        // Kill removes the respective canister from the list. No need to do this manually.
                        for (int j = 0; j < modPlayer.GammaCanisters.Count; j++)
                        {
                            Main.projectile[modPlayer.GammaCanisters[i]].Kill();
                        }
                        break;
                    }
                }
            }

            // Spawn gamma canisters
            else if (player.ownedProjectileCounts[projectile.type] >= 7)
            {
                int ownedGammaHeads = player.ownedProjectileCounts[projectile.type];
                int canisterSpawnChance = 520 - (int)(200 * (1 - 1f / (ownedGammaHeads + 6f)));
                if (Main.rand.NextBool(canisterSpawnChance * ownedGammaHeads))
                {
                    Vector2 spawnPosition = player.Center + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2) * Main.rand.NextFloat(-1f, 1f);
                    Projectile.NewProjectileDirect(spawnPosition, Vector2.Zero, ModContent.ProjectileType<GammaCanister>(), 0, 0f, projectile.owner);
                }
            }

            // Omega-complex dust.
            projectile.localAI[1]++;
            if (projectile.localAI[1] < 60f)
            {
                for (int i = 0; i < 5; i++)
                {
                    float angle = projectile.localAI[1] / 60f * MathHelper.TwoPi;
                    float x = (float)Math.Sin(angle * AngularMultiplier1) * (float)Math.Cos(angle);
                    float y = (float)Math.Cos(angle * (float)Math.Cos(MathHelper.PiOver2 * AngularMultiplier2 + angle * 2f)) * (float)Math.Sin(angle);
                    Vector2 velocity = new Vector2(x * 4f, y * 4f) + player.velocity;
                    velocity = velocity.RotatedBy(MathHelper.TwoPi / 5f * i);

                    Dust dust = Dust.NewDustPerfect(projectile.Center + angle.ToRotationVector2() * 8f, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.velocity = velocity;
                    dust.scale = (float)Math.Cos(angle) + 1.3f;
                    dust.noGravity = true;
                }
            }
            // Explosion
            if (projectile.localAI[1] == 60f)
            {
                for (int i = 0; i < 60; i++)
                {
                    float angle = MathHelper.TwoPi / 60f * i;
                    angle += projectile.AngleFrom(player.Center) / 60f;
                    angle += (float)Math.Sin(angle) * MathHelper.PiOver2;
                    Dust dust = Dust.NewDustPerfect(projectile.Center + angle.ToRotationVector2() * 8f, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.velocity = angle.ToRotationVector2() * new Vector2(4f, 3f * (float)Math.Cos(angle)) * 3f;
                    dust.scale = 1.3f;
                    dust.noGravity = true;
                }
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }

            Vector2 returnPosition = DrawStartPosition + (player.direction > 0).ToDirectionInt() * 6f * Vector2.UnitX + DeltaPosition;

            NPC potentialTarget = projectile.Center.MinionHoming(DistanceToCheck, player);

            // Kill laser when done
            if (projectile.ai[1] == GammaDeathray.TotalFadeoutTime && projectile.ai[0] != 0f)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].ai[1] >= 0f && Main.projectile[i].type == projectile.type && Main.projectile[i].owner == projectile.owner)
                    {
                        Main.projectile[i].spriteDirection = 1;
                        Main.projectile[i].ai[1] = 0f;
                        Main.projectile[i].netUpdate = true;
                        Main.projectile[(int)Main.projectile[i].ai[0]].timeLeft = GammaDeathray.TotalFadeoutTime;
                    }
                }
            }
            // Bullet/deathray
            if (projectile.ai[1] > 0)
            {
                projectile.direction = projectile.spriteDirection = 1;
                projectile.rotation = projectile.AngleTo(Main.MouseWorld);
                projectile.ai[1]--;
            }
            else if (potentialTarget != null)
            {
                if (projectile.Distance(player.Center) < DistanceToCheck &&
                    Collision.CanHit(projectile.Center, 1, 1, player.Center, 1, 1) &&
                    Main.myPlayer == projectile.owner)
                {
                    BulletShootCounter++;
                    if (BulletShootCounter % 20f == 14f)
                    {
                        Projectile bullet = Projectile.NewProjectileDirect(projectile.Center,
                            projectile.DirectionTo(potentialTarget.Center) * 2.5f,
                            ModContent.ProjectileType<GammaBullet>(), projectile.damage, 2f, projectile.owner);
                        bullet.ai[0] = potentialTarget.whoAmI;
                    }

                    if (BulletShootCounter % 20f >= 17f)
                        projectile.frame = Main.projFrames[projectile.type] - 1;
                    else if (BulletShootCounter % 20f >= 11f)
                        projectile.frame = Main.projFrames[projectile.type] - 2;
                    else if (BulletShootCounter % 20f >= 6f)
                        projectile.frame = Main.projFrames[projectile.type] - 3;

                    projectile.direction = projectile.spriteDirection = (potentialTarget.Center.X - projectile.Center.X > 0).ToDirectionInt();
                    returnPosition.X -= 48f * (player.Center.X - projectile.Center.X > 0).ToDirectionInt();
                }
                else
                {
                    projectile.direction = projectile.spriteDirection = (player.Center.X - projectile.Center.X > 0).ToDirectionInt();
                    projectile.frame = 0;
                }
                projectile.rotation = projectile.rotation.AngleTowards(0f, 0.05f);
            }
            else
            {
                projectile.direction = projectile.spriteDirection = (player.Center.X - projectile.Center.X > 0).ToDirectionInt();
                projectile.rotation = projectile.rotation.AngleTowards(0f, 0.05f);
                projectile.frame = 0;
            }

            if (projectile.Distance(returnPosition) > 28f)
            {
                projectile.velocity += new Vector2(Math.Sign(returnPosition.X - projectile.Center.X), Math.Sign(returnPosition.Y - projectile.Center.Y)) * new Vector2(0.015f, 0.025f) * Main.rand.NextFloat(0.98f, 1.02f);
                if (Math.Abs(projectile.velocity.X) > 2.5f)
                {
                    projectile.velocity.X = 2.1f * Math.Sign(projectile.velocity.X);
                }
                if (Math.Abs(projectile.velocity.Y) > 1.6f)
                {
                    projectile.velocity.Y = 1.4f * Math.Sign(projectile.velocity.Y);
                }
                projectile.velocity += (returnPosition - projectile.Center) / 10f;
            }
            else projectile.velocity *= 0.98f;
            projectile.Center = new Vector2(projectile.Center.X, MathHelper.Clamp(projectile.Center.Y, 1f, returnPosition.Y - 8f));
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.SulfurousSeaAcid, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 4f).noGravity = true;
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player player = Main.player[projectile.owner];

            Texture2D chain = ModContent.GetTexture("CalamityMod/Projectiles/Summon/GammaHeadChain");
            Vector2 start = projectile.Center + (projectile.spriteDirection == -1).ToInt() * 10f * Vector2.UnitX;
            Vector2 end = DrawStartPosition - projectile.DirectionTo(DrawStartPosition) * 11f;
            Vector2 bodyTop = DrawStartPosition + new Vector2(player.direction == 1 ? 6f : -6f, 0f);

            for (int i = 0; i <= (int)projectile.Distance(end) / 10 + 1; i++)
            {
                float ratio = i / (projectile.Distance(end) / 10 + 1);
                Vector2 positionAtPoint = start + (bodyTop - start) * ratio;
                if (projectile.Distance(positionAtPoint) > 9)
                {
                    float angleAtPoint = (start - positionAtPoint).ToRotation();
                    if (i < (int)projectile.Distance(end) / 10 + 1)
                    {
                        float nextRatio = (i + 1f) / (projectile.Distance(end) / 10 + 1);
                        Vector2 nextPositionAtPoint = start + (bodyTop - start) * nextRatio;

                        angleAtPoint = (nextPositionAtPoint - positionAtPoint).ToRotation();
                    }
                    angleAtPoint += MathHelper.PiOver2;
                    spriteBatch.Draw(chain,
                        positionAtPoint - Main.screenPosition,
                        null,
                        Color.Lerp(Color.White, Color.Transparent, 0.5f),
                        angleAtPoint,
                        chain.Size() / 2f,
                        1f,
                        SpriteEffects.None,
                        0f);
                }
            }
        }
        public override bool CanDamage() => false;
    }
}
