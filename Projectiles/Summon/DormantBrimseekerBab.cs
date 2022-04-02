using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class DormantBrimseekerBab : ModProjectile
    {
        public const float DistanceToCheck = 1600f;
        public const float TurnTime = 25f;
        public bool SeekingTarget = false;
        public float MaxChargeTime
        {
            get
            {
                return (projectile.localAI[1] == 1f) ? 21 : 38;
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dormant Brimseeker");

            Main.projFrames[projectile.type] = 8;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;

            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 46;
            projectile.height = 36;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 1f;
            projectile.ignoreWater = true;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
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
            bool isProperProjectile = projectile.type == ModContent.ProjectileType<DormantBrimseekerBab>();
            player.AddBuff(ModContent.BuffType<DormantBrimseekerBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.brimseeker = false;
                }
                if (modPlayer.brimseeker)
                {
                    projectile.timeLeft = 2;
                }
            }

            NPC potentialTarget = projectile.Center.MinionHoming(DistanceToCheck, player);
            if (potentialTarget == null)
            {
                if (projectile.ai[0] != 0f)
                {
                    projectile.ai[0] = 0f; // Disable charge animation when not attacking
                }
                projectile.rotation = projectile.rotation.AngleTowards(0f, 0.1f);
                projectile.direction = projectile.spriteDirection = (projectile.velocity.X < 0).ToDirectionInt();
                projectile.ai[1] += MathHelper.ToRadians(3f);
                if (projectile.ai[1] >= MathHelper.Pi * 4f)
                {
                    projectile.ai[1] = 0f;
                }
                Vector2 destination = player.Center + projectile.ai[1].ToRotationVector2() * new Vector2(1f, (float)Math.Cos(projectile.ai[1])) * 200f;

                projectile.velocity = (projectile.velocity * 18f + projectile.SafeDirectionTo(destination) * 14f) / 20f;

                projectile.frameCounter++;
                if (projectile.frameCounter >= 6)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                }
                if (projectile.frame >= 4 + (projectile.localAI[1] == 1f).ToInt() * 4)
                {
                    projectile.frame = (projectile.localAI[1] == 1f).ToInt() * 4;
                }

                projectile.MinionAntiClump(0.1f);
                if (projectile.Distance(player.Center) > 2700f)
                {
                    projectile.Center = player.Center;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                if (projectile.Distance(potentialTarget.Center) < 1100f && projectile.ai[0] == 0f)
                {
                    Main.PlaySound(SoundID.DD2_DrakinShot, projectile.Center);
                    projectile.ai[0]++;
                    float acceleration = (projectile.localAI[1] == 1f) ? 1.6f : 0.8f;
                    float minSpeed = (projectile.localAI[1] == 1f) ? 13f : 8f;
                    float maxSpeed = (projectile.localAI[1] == 1f) ? 21f : 18f;
                    projectile.velocity = projectile.SafeDirectionTo(potentialTarget.Center) * MathHelper.Clamp(projectile.velocity.Length() + acceleration, minSpeed, maxSpeed);
                    projectile.rotation = projectile.AngleTo(potentialTarget.Center) + (projectile.spriteDirection == 1).ToInt() * MathHelper.Pi;
                }
                else if (projectile.ai[0] > 0)
                {
                    projectile.ai[0]++;
                    SeekingTarget = false;
                }
                else if (projectile.Distance(potentialTarget.Center) >= 1100f)
                {
                    projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(potentialTarget.Center) + (projectile.spriteDirection == 1).ToInt() * MathHelper.Pi, 0.1f);
                    projectile.velocity = projectile.SafeDirectionTo(potentialTarget.Center) * MathHelper.Clamp(projectile.velocity.Length() + 2f, 6f, 15f);
                    SeekingTarget = true;
                    projectile.ai[0] = 0f;
                }

                if (projectile.ai[0] >= MaxChargeTime)
                {
                    projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(potentialTarget.Center) + (projectile.spriteDirection == 1).ToInt() * MathHelper.Pi, 0.1f);
                }

                projectile.frameCounter++;
                if (projectile.frameCounter >= 6)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                }
                if (projectile.frame >= 8)
                {
                    projectile.frame = 4;
                }

                if (projectile.ai[0] >= MaxChargeTime + TurnTime)
                    projectile.ai[0] = 0f;
            }
            Lighting.AddLight(projectile.Center, Color.Red.ToVector3());
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if ((projectile.ai[0] > 0f && projectile.ai[0] <= MaxChargeTime && projectile.velocity.Length() >= 8f) || SeekingTarget)
            {
                Texture2D projectileTexture = ModContent.GetTexture(Texture);
                Texture2D flameTexture = Main.extraTexture[ExtrasID.MeteorHeadFlame];
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (projectile.spriteDirection == -1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                float completionRatio = projectile.ai[0] / 30f;
                if (projectile.ai[0] > 30f)
                    completionRatio = 1f - (30f - projectile.ai[0]) / 30f;

                Vector2 modifiedProjectileTexture = new Vector2(projectileTexture.Width / 2,projectileTexture.Height / Main.projFrames[projectile.type] / 2);
                for (int oldPositionDrawIndex = 6; oldPositionDrawIndex >= 0; oldPositionDrawIndex--)
                {
                    Color drawColor = Color.Lerp(Color.LightGoldenrodYellow, new Color(142, 24, 67), completionRatio);
                    drawColor = Color.Lerp(drawColor, new Color(142, 24, 67), oldPositionDrawIndex / 9.7f);
                    drawColor.A = (byte)(64f * completionRatio);
                    drawColor.R = (byte)(drawColor.R * (10 - oldPositionDrawIndex) / 20);
                    drawColor.G = (byte)(drawColor.G * (10 - oldPositionDrawIndex) / 20);
                    drawColor.B = (byte)(drawColor.B * (10 - oldPositionDrawIndex) / 20);
                    drawColor.A = (byte)(drawColor.A * (10 - oldPositionDrawIndex) / 20);
                    drawColor *= completionRatio;
                    int yFrame = ((int)projectile.ai[0] / 2 - oldPositionDrawIndex) % 4;
                    if (yFrame < 0)
                    {
                        yFrame += 4;
                    }
                    Rectangle flameFrameRectangle = flameTexture.Frame(1, 4, 0, yFrame);
                    Vector2 flameOrigin = new Vector2(flameTexture.Width / 2, flameTexture.Height / 8 + 14);
                    spriteBatch.Draw(flameTexture,
                                          new Vector2(projectile.oldPos[oldPositionDrawIndex].X - Main.screenPosition.X + projectile.width / 2 - projectileTexture.Width * projectile.scale / 2f + modifiedProjectileTexture.X * projectile.scale, projectile.oldPos[oldPositionDrawIndex].Y - Main.screenPosition.Y + projectile.height - projectileTexture.Height * projectile.scale / Main.projFrames[projectile.type] + 4f + modifiedProjectileTexture.Y * projectile.scale + projectile.gfxOffY),
                                          new Rectangle?(flameFrameRectangle),
                                          drawColor,
                                          projectile.oldRot[oldPositionDrawIndex] + projectile.oldSpriteDirection[oldPositionDrawIndex] * MathHelper.PiOver2,
                                          flameOrigin,
                                          MathHelper.Lerp(0.1f, 1.2f, (10f - oldPositionDrawIndex) / 10f),
                                          spriteEffects,
                                          0f);
                }
            }
        }
    }
}
