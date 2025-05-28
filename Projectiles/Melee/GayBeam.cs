using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class GayBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        
        private const int Alpha = 100;

        private const int TimeLeft = 600;

        private const int TrailLength = 36;

        private const int TotalAfterimages = 12;

        private const int AfterimageOffset = TrailLength / TotalAfterimages;

        private const float MaxVelocity = 24f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = TrailLength;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = TimeLeft;
            Projectile.alpha = Alpha;
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

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

            if (Projectile.ai[2] == 0f)
                SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
            
            Color color = GetProjectileColor();
            switch ((int)Projectile.ai[0])
            {
                case 0: // Red, normal beam
                    break;

                case 1: // Orange, curve back to player

                    int player = (int)Player.FindClosest(Projectile.Center, 1, 1);
                    Projectile.ai[1] += 1f;
                    if (Projectile.ai[1] < 360f && Projectile.ai[1] > 60f)
                    {
                        float homeSpeed = Projectile.velocity.Length();
                        Vector2 vecToPlayer = Main.player[player].Center - Projectile.Center;
                        vecToPlayer = vecToPlayer.SafeNormalize(Vector2.UnitY);
                        vecToPlayer *= homeSpeed;
                        Projectile.velocity = (Projectile.velocity * 29f + vecToPlayer) / 30f;
                        Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                        Projectile.velocity *= homeSpeed;
                    }

                    if (Projectile.velocity.Length() < MaxVelocity)
                    {
                        Projectile.velocity *= 1.01f;
                        if (Projectile.velocity.Length() > MaxVelocity)
                        {
                            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                            Projectile.velocity *= MaxVelocity;
                        }
                    }

                    break;

                case 2: // Yellow, split after a certain time

                    Projectile.localAI[1] += 1f;
                    if (Projectile.localAI[1] >= 45f)
                    {
                        int numProj = 2;
                        float rotation = MathHelper.ToRadians(10);
                        if (Projectile.owner == Main.myPlayer)
                        {
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed * MaxVelocity, Projectile.type, (int)(Projectile.damage * 0.5), Projectile.knockBack * 0.5f, Projectile.owner);
                            }
                        }

                        Projectile.Kill();
                    }

                    break;

                case 3: // Lime, home in on player within certain distance

                    float inertia = 30f;
                    float homingSpeed = MaxVelocity;
                    float minDist = 300f;
                    if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
                    {
                        if (Projectile.Distance(Main.player[Projectile.owner].Center) > minDist)
                        {
                            Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center, Vector2.UnitY);
                            Projectile.velocity = (Projectile.velocity * (inertia - 1f) + moveDirection * homingSpeed) / inertia;
                        }
                    }
                    else
                    {
                        if (Projectile.timeLeft > 30)
                            Projectile.timeLeft = 30;
                    }

                    break;

                case 4: // Green, home in on enemies

                    CalamityUtils.HomeInOnNPC(Projectile, false, 600f, MaxVelocity, 30f);

                    break;

                case 5: // Turquoise, speed up and don't collide with tiles

                    Projectile.tileCollide = false;
                    if (Projectile.velocity.Length() < MaxVelocity)
                    {
                        Projectile.velocity *= 1.03f;
                        if (Projectile.velocity.Length() > MaxVelocity)
                        {
                            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                            Projectile.velocity *= MaxVelocity;
                        }
                    }

                    break;

                case 6: // Cyan, upon death create several turquoise swords
                case 7: // Light Blue, slow down on hit
                case 8: // Blue, bounce off of tiles and enemies
                    break;

                case 9: // Purple, speed up and slow down over and over

                    if (Projectile.localAI[1] < 30f)
                        Projectile.velocity *= 0.95f;
                    else if (Projectile.localAI[1] < 60f)
                        Projectile.velocity *= 1.07f;
                    else if (Projectile.localAI[1] == 60f)
                        Projectile.localAI[1] = -1f;
                    Projectile.localAI[1] += 1f;

                    break;

                case 10: // Fuschia, start slow then get faster

                    if (Projectile.localAI[1] == 0f)
                    {
                        Projectile.velocity *= 0.1f;
                        Projectile.localAI[1] += 1f;
                    }

                    if (Projectile.velocity.Length() < MaxVelocity)
                    {
                        Projectile.velocity *= 1.05f;
                        if (Projectile.velocity.Length() > MaxVelocity)
                        {
                            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                            Projectile.velocity *= MaxVelocity;
                        }
                    }

                    break;

                case 11: // Hot Pink, split into fuschia while travelling

                    if (Projectile.localAI[1] == 0f)
                        Projectile.velocity *= 0.333f;

                    if (Projectile.localAI[1] >= 45f)
                    {
                        Projectile.localAI[1] = 0f;
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.oldVelocity, Projectile.velocity, Projectile.type, (int)Math.Round(Projectile.damage * 0.5), Projectile.knockBack * 0.5f, Projectile.owner, 10f, 0f);
                    }
                    Projectile.localAI[1] += 1f;

                    break;

                default:
                    break;
            }

            Projectile.ai[2] += MathHelper.Lerp(1f, 2f, Projectile.ai[0] / 11f);
            if (Projectile.ai[2] > 7f)
            {
                Dust rainbow = Main.dust[Dust.NewDust(new Vector2(Projectile.position.X - Projectile.velocity.X + 2f, Projectile.position.Y + 2f - Projectile.velocity.Y), 8, 8, DustID.RainbowMk2, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, Projectile.alpha, default, 1.25f)];
                rainbow.velocity *= -0.25f;
                rainbow.color = color;
                rainbow.color.A = 100;
                rainbow.scale = Main.rand.NextFloat(1f, 1.25f);
                rainbow.fadeIn = Main.rand.NextFloat(0.4f, 1f);
                rainbow.noGravity = true;
                rainbow.noLightEmittence = true;

                rainbow = Main.dust[Dust.NewDust(new Vector2(Projectile.position.X - Projectile.velocity.X + 2f, Projectile.position.Y + 2f - Projectile.velocity.Y), 8, 8, DustID.RainbowMk2, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, Projectile.alpha, default, 1.25f)];
                rainbow.velocity *= -0.25f;
                rainbow.color = color;
                rainbow.color.A = 100;
                rainbow.scale = Main.rand.NextFloat(1f, 1.25f);
                rainbow.fadeIn = Main.rand.NextFloat(0.4f, 1f);
                rainbow.noGravity = true;
                rainbow.position -= Projectile.velocity * 0.5f;
                rainbow.noLightEmittence = true;

                if (Projectile.localAI[0] == 0f)
                {
                    Projectile.scale -= 0.02f;
                    Projectile.alpha += 5;
                    if (Projectile.alpha >= Alpha - 5)
                    {
                        Projectile.alpha = Alpha;
                        Projectile.localAI[0] = 1f;
                    }
                }
                else if (Projectile.localAI[0] == 1f)
                {
                    Projectile.scale += 0.02f;
                    Projectile.alpha -= 5;
                    if (Projectile.alpha <= 0)
                    {
                        Projectile.alpha = 0;
                        Projectile.localAI[0] = 0f;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);

            if (Projectile.ai[0] == 7f)
                Projectile.velocity *= 0.5f;
            else if (Projectile.ai[0] == 8f)
                Projectile.velocity *= -1f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);

            if (Projectile.ai[0] == 7f)
                Projectile.velocity *= 0.5f;
            else if (Projectile.ai[0] == 8f)
                Projectile.velocity *= -1f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 8f)
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;

                return false;
            }

            return true;
        }

        public Color TrailColor(float completionRatio)
        {
            float hue = (Main.GlobalTimeWrappedHourly * -0.62f + completionRatio * 1.5f) % 1f;
            float brightness = MathHelper.SmoothStep(0.5f, 1f, Utils.GetLerpValue(0.3f, 0f, completionRatio, true));
            float opacity = Utils.GetLerpValue(1f, 0.8f, completionRatio, true) * ((255 - Projectile.alpha) / 255f);
            Color color = GetProjectileColor() * opacity;
            color.A = (byte)(int)(Utils.GetLerpValue(0f, 0.2f, completionRatio) * 128);
            return color;
        }

        public float TrailWidth(float completionRatio)
        {
            float widthInterpolant = Utils.GetLerpValue(0f, 0.25f, completionRatio, true) * Utils.GetLerpValue(1.1f, 0.7f, completionRatio, true);
            return MathHelper.SmoothStep(0f, 14f, widthInterpolant) * Projectile.scale;
        }

        public override Color? GetAlpha(Color lightColor) => GetProjectileColor();

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > TimeLeft - 5)
                return false;

            Main.spriteBatch.EnterShaderRegion();

            GameShaders.Misc["CalamityMod:ArtAttack"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            GameShaders.Misc["CalamityMod:ArtAttack"].Apply();

            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(TrailWidth, TrailColor, (_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:ArtAttack"]), TrailLength);

            Main.spriteBatch.ExitShaderRegion();

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;
            float scale = Projectile.scale;
            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = rectangle.Size() / 2f;
            Color alphaColor = Projectile.GetAlpha(lightColor);

            if (CalamityConfig.Instance.Afterimages)
            {
                Vector2 centerOffset = Projectile.Size / 2f;
                int totalAfterimages = TotalAfterimages;
                for (int i = 0; i < totalAfterimages; i++)
                {
                    int arrayPos = i * AfterimageOffset;
                    float afterimageRot = Projectile.oldRot[arrayPos];
                    SpriteEffects sfxForThisAfterimage = Projectile.oldSpriteDirection[arrayPos] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    Vector2 drawPos = Projectile.oldPos[arrayPos] + centerOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                    float colorAndSizeScalar = (float)(totalAfterimages - i) / (float)totalAfterimages;
                    Color color = alphaColor * colorAndSizeScalar;
                    float afterimageScale = scale * MathHelper.Lerp(0.5f, 1f, colorAndSizeScalar);
                    Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, afterimageRot, origin, afterimageScale, sfxForThisAfterimage, 0f);
                }
            }
            else
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (Projectile.spriteDirection == -1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, alphaColor, Projectile.rotation, origin, scale, spriteEffects, 0f);
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

            Color color = GetProjectileColor();
            switch ((int)Projectile.ai[0])
            {
                case 0: // Red
                case 1: // Orange
                case 2: // Yellow
                case 3: // Lime
                case 4: // Green
                case 5: // Turquoise
                    break;

                case 6: // Cyan

                    for (int x = 0; x < 2; x++)
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            var source = Projectile.GetSource_FromThis();
                            CalamityUtils.ProjectileBarrage(source, Projectile.Center, Projectile.Center, x == 1, 500f, 500f, 0f, 500f, MaxVelocity, Projectile.type, (int)Math.Round(Projectile.damage * 0.5), Projectile.knockBack * 0.5f, Projectile.owner, false, 0f).ai[0] = 5f;
                        }
                    }

                    break;

                case 7: // Light Blue
                case 8: // Blue
                case 9: // Purple
                case 10: // Fuschia
                case 11: // Hot Pink
                default:
                    break;
            }

            int totalDust = 60;
            for (int i = 4; i < totalDust + 1; i++)
            {
                Vector2 oldVel = Projectile.oldVelocity * (totalDust / (float)i);
                Dust rainbow = Main.dust[Dust.NewDust(Projectile.oldPosition - oldVel * 0.5f, 8, 8, DustID.RainbowMk2, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, Projectile.alpha)];
                rainbow.color = color;
                rainbow.color.A = 100;
                rainbow.scale = Main.rand.NextFloat(2.4f, 3.6f);
                rainbow.fadeIn = Main.rand.NextFloat(0.4f, 1f);
                rainbow.noGravity = true;
                rainbow.velocity *= (Main.rand.NextBool() ? 0.05f : 0.5f);
            }
        }

        private Color GetProjectileColor()
        {
            Color rainbow = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Alpha);
            Color color = Color.White;
            switch ((int)Projectile.ai[0])
            {
                case 0: // Red
                    color = new Color(255, 0, 0, Alpha);
                    break;
                case 1: // Orange
                    color = new Color(255, 128, 0, Alpha);
                    break;
                case 2: // Yellow
                    color = new Color(255, 255, 0, Alpha);
                    break;
                case 3: // Lime
                    color = new Color(128, 255, 0, Alpha);
                    break;
                case 4: // Green
                    color = new Color(0, 255, 0, Alpha);
                    break;
                case 5: // Turquoise
                    color = new Color(0, 255, 128, Alpha);
                    break;
                case 6: // Cyan
                    color = new Color(0, 255, 255, Alpha);
                    break;
                case 7: // Light Blue
                    color = new Color(0, 128, 255, Alpha);
                    break;
                case 8: // Blue
                    color = new Color(0, 0, 255, Alpha);
                    break;
                case 9: // Purple
                    color = new Color(128, 0, 255, Alpha);
                    break;
                case 10: // Fuschia
                    color = new Color(255, 0, 255, Alpha);
                    break;
                case 11: // Hot Pink
                    color = new Color(255, 0, 128, Alpha);
                    break;
                default:
                    break;
            }
            return Color.Lerp(rainbow, color, (float)Math.Sqrt((MathF.Cos(Projectile.ai[2] / 90f) + 1f) * 0.5f));
        }
    }
}
