using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;

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
                return (Projectile.localAI[1] == 1f) ? 21 : 38;
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dormant Brimseeker");

            Main.projFrames[Projectile.type] = 8;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 36;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            bool isProperProjectile = Projectile.type == ModContent.ProjectileType<DormantBrimseekerBab>();
            player.AddBuff(ModContent.BuffType<BrimseekerBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.brimseeker = false;
                }
                if (modPlayer.brimseeker)
                {
                    Projectile.timeLeft = 2;
                }
            }

            NPC potentialTarget = Projectile.Center.MinionHoming(DistanceToCheck, player);
            if (potentialTarget == null)
            {
                if (Projectile.ai[0] != 0f)
                {
                    Projectile.ai[0] = 0f; // Disable charge animation when not attacking
                }
                Projectile.rotation = Projectile.rotation.AngleTowards(0f, 0.1f);
                Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X < 0).ToDirectionInt();
                Projectile.ai[1] += MathHelper.ToRadians(3f);
                if (Projectile.ai[1] >= MathHelper.Pi * 4f)
                {
                    Projectile.ai[1] = 0f;
                }
                Vector2 destination = player.Center + Projectile.ai[1].ToRotationVector2() * new Vector2(1f, (float)Math.Cos(Projectile.ai[1])) * 200f;

                Projectile.velocity = (Projectile.velocity * 18f + Projectile.SafeDirectionTo(destination) * 14f) / 20f;

                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame >= 4 + (Projectile.localAI[1] == 1f).ToInt() * 4)
                {
                    Projectile.frame = (Projectile.localAI[1] == 1f).ToInt() * 4;
                }

                Projectile.MinionAntiClump(0.1f);
                if (Projectile.Distance(player.Center) > 2700f)
                {
                    Projectile.Center = player.Center;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                if (Projectile.Distance(potentialTarget.Center) < 1100f && Projectile.ai[0] == 0f)
                {
                    SoundEngine.PlaySound(SoundID.DD2_DrakinShot, Projectile.Center);
                    Projectile.ai[0]++;
                    float acceleration = (Projectile.localAI[1] == 1f) ? 1.6f : 0.8f;
                    float minSpeed = (Projectile.localAI[1] == 1f) ? 13f : 8f;
                    float maxSpeed = (Projectile.localAI[1] == 1f) ? 21f : 18f;
                    Projectile.velocity = Projectile.SafeDirectionTo(potentialTarget.Center) * MathHelper.Clamp(Projectile.velocity.Length() + acceleration, minSpeed, maxSpeed);
                    Projectile.rotation = Projectile.AngleTo(potentialTarget.Center) + (Projectile.spriteDirection == 1).ToInt() * MathHelper.Pi;
                }
                else if (Projectile.ai[0] > 0)
                {
                    Projectile.ai[0]++;
                    SeekingTarget = false;
                }
                else if (Projectile.Distance(potentialTarget.Center) >= 1100f)
                {
                    Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(potentialTarget.Center) + (Projectile.spriteDirection == 1).ToInt() * MathHelper.Pi, 0.1f);
                    Projectile.velocity = Projectile.SafeDirectionTo(potentialTarget.Center) * MathHelper.Clamp(Projectile.velocity.Length() + 2f, 6f, 15f);
                    SeekingTarget = true;
                    Projectile.ai[0] = 0f;
                }

                if (Projectile.ai[0] >= MaxChargeTime)
                {
                    Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(potentialTarget.Center) + (Projectile.spriteDirection == 1).ToInt() * MathHelper.Pi, 0.1f);
                }

                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame >= 8)
                {
                    Projectile.frame = 4;
                }

                if (Projectile.ai[0] >= MaxChargeTime + TurnTime)
                    Projectile.ai[0] = 0f;
            }
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3());
        }
        public override void PostDraw(Color lightColor)
        {
            if ((Projectile.ai[0] > 0f && Projectile.ai[0] <= MaxChargeTime && Projectile.velocity.Length() >= 8f) || SeekingTarget)
            {
                Texture2D projectileTexture = ModContent.Request<Texture2D>(Texture).Value;
                Texture2D flameTexture = TextureAssets.Extra[ExtrasID.MeteorHeadFlame].Value;
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (Projectile.spriteDirection == -1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                float completionRatio = Projectile.ai[0] / 30f;
                if (Projectile.ai[0] > 30f)
                    completionRatio = 1f - (30f - Projectile.ai[0]) / 30f;

                Vector2 modifiedProjectileTexture = new Vector2(projectileTexture.Width / 2,projectileTexture.Height / Main.projFrames[Projectile.type] / 2);
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
                    int yFrame = ((int)Projectile.ai[0] / 2 - oldPositionDrawIndex) % 4;
                    if (yFrame < 0)
                    {
                        yFrame += 4;
                    }
                    Rectangle flameFrameRectangle = flameTexture.Frame(1, 4, 0, yFrame);
                    Vector2 flameOrigin = new Vector2(flameTexture.Width / 2, flameTexture.Height / 8 + 14);
                    Main.EntitySpriteDraw(flameTexture,
                                          new Vector2(Projectile.oldPos[oldPositionDrawIndex].X - Main.screenPosition.X + Projectile.width / 2 - projectileTexture.Width * Projectile.scale / 2f + modifiedProjectileTexture.X * Projectile.scale, Projectile.oldPos[oldPositionDrawIndex].Y - Main.screenPosition.Y + Projectile.height - projectileTexture.Height * Projectile.scale / Main.projFrames[Projectile.type] + 4f + modifiedProjectileTexture.Y * Projectile.scale + Projectile.gfxOffY),
                                          new Rectangle?(flameFrameRectangle),
                                          drawColor,
                                          Projectile.oldRot[oldPositionDrawIndex] + Projectile.oldSpriteDirection[oldPositionDrawIndex] * MathHelper.PiOver2,
                                          flameOrigin,
                                          MathHelper.Lerp(0.1f, 1.2f, (10f - oldPositionDrawIndex) / 10f),
                                          spriteEffects,
                                          0);
                }
            }
        }
    }
}
