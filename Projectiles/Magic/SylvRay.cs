using System;
using System.IO;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SylvRay : ModProjectile, ILocalizedModType
    {
        /// <summary>
        ///     The position from which this ray's glow effect appears.
        /// </summary>
        public Vector2 GlowCenter
        {
            get;
            set;
        }

        /// <summary>
        ///     How many natural frames have elapsed so far throughout the duration of this ray's life. Extra updates do not affect this timer.
        /// </summary>
        public int Time
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 54;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 70;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 60 * Projectile.extraUpdates;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.WriteVector2(GlowCenter);

        public override void ReceiveExtraAI(BinaryReader reader) => GlowCenter = reader.ReadVector2();

        public override void AI()
        {
            if (Projectile.FinalExtraUpdate())
                Time++;

            Lighting.AddLight(Projectile.Center, 0.2f, 0.01f, 0.1f);
            Projectile.Opacity = Utils.GetLerpValue(0f, Projectile.MaxUpdates * 10f, Projectile.timeLeft, true);
            Projectile.scale = Utils.GetLerpValue(1f, 9.5f, Time, true) * Projectile.Opacity;

            if (GlowCenter == Vector2.Zero)
                GlowCenter = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 132f;

            CreateGlowyDust();

            int shootRate = 12 / Projectile.MaxUpdates;
            if (Projectile.owner == Main.myPlayer && Time % shootRate == 0 && Projectile.FinalExtraUpdate())
            {
                NPC potentialTarget = Projectile.Center.ClosestNPCAt(180f, false);
                if (potentialTarget != null)
                {
                    Vector2 shootVelocity = Projectile.SafeDirectionTo(potentialTarget.Center) * 13f;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVelocity, ModContent.ProjectileType<SylvBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    if (p.WithinBounds(Main.maxProjectiles))
                    {
                        if (Projectile.hostile)
                        {
                            Main.projectile[p].hostile = true;
                            Main.projectile[p].friendly = false;
                            Main.projectile[p].DamageType = DamageClass.Default;
                        }
                    }
                }

                for (int i = 0; i < Projectile.oldPos.Length / 4; i += 3)
                {
                    potentialTarget = Projectile.oldPos[i].ClosestNPCAt(280f, false);
                    if (potentialTarget != null)
                    {
                        Vector2 shootVelocity = (potentialTarget.Center - Projectile.oldPos[i]).SafeNormalize(Vector2.UnitY) * 13f;
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.oldPos[i], shootVelocity, ModContent.ProjectileType<SylvBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        if (p.WithinBounds(Main.maxProjectiles))
                        {
                            if (Projectile.hostile)
                            {
                                Main.projectile[p].hostile = true;
                                Main.projectile[p].friendly = false;
                                Main.projectile[p].DamageType = DamageClass.Default;
                            }
                        }
                        break;
                    }
                }
            }

            // Emit light.
            if (!Projectile.hostile)
                Lighting.AddLight(Projectile.Center, Vector3.One * Projectile.Opacity * 0.7f);
        }

        /// <summary>
        ///     Emits dust particles at the glow center of this ray, to help sell the notion that it's appearing from a concentrated ball of queer magic energy.
        /// </summary>
        private void CreateGlowyDust()
        {
            if (Time <= 9 && Main.rand.NextBool())
            {
                Color colorAccent = Main.rand.NextBool() ? Color.HotPink : Color.Aqua;

                Dust transMagic = Dust.NewDustPerfect(GlowCenter, 264);
                transMagic.velocity = Main.rand.NextVector2Circular(4.6f, 4.6f) + Projectile.velocity * 0.25f;
                transMagic.color = Color.Lerp(Color.White, colorAccent, Main.rand.NextFloat(0.23f));
                transMagic.scale *= 1.1f;
                transMagic.fadeIn = 0.75f;
                transMagic.noGravity = true;
            }
        }

        internal Color ColorFunction(float completionRatio)
        {
            float opacity = MathF.Pow(Utils.GetLerpValue(1f, 0.64f, completionRatio, true), 3f) * Projectile.Opacity;
            float colorInterpolant = MathF.Cos(MathHelper.Pi * completionRatio - Main.GlobalTimeWrappedHourly * 7.2f) * 0.5f + 0.5f;

            Color pink = new Color(255, 147, 255);
            Color blue = new Color(109, 224, 255);
            Color baseColor = CalamityUtils.MulticolorLerp(colorInterpolant, pink, Color.White, blue, Color.White);

            return baseColor * opacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            float expansionCompletion = 1f - (float)Math.Pow(1f - Utils.GetLerpValue(0f, 0.3f, completionRatio, true), 2D);
            float undulation = MathF.Cos(MathHelper.Pi * completionRatio * 5f - Main.GlobalTimeWrappedHourly * 23f) * 2.4f;
            float maxWidth = undulation + 32f;

            return MathHelper.Lerp(0f, Projectile.scale * maxWidth, expansionCompletion);
        }

        internal Vector2 OffsetFunction(float completionRatio) => Projectile.Size * 0.5f;

        private void RenderGlow()
        {
            float glowAnimationProgress = Utils.GetLerpValue(0f, 9.5f, Time, true);
            float glowBump = CalamityUtils.Convert01To010(glowAnimationProgress);
            float glowRotation = Projectile.velocity.ToRotation();
            Vector2 glowScale = new Vector2(1f + glowBump * 0.8f, 1f) * glowBump;

            Vector2 startingPosition = GlowCenter - Main.screenPosition;
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/BloomCirclePinpoint").Value;
            Main.spriteBatch.Draw(lightTexture, startingPosition, null, ColorFunction(0f) with { A = 0 }, glowRotation, lightTexture.Size() * 0.5f, glowScale, 0, 0f);
            Main.spriteBatch.Draw(lightTexture, startingPosition, null, ColorFunction(0f) with { A = 0 }, glowRotation, lightTexture.Size() * 0.5f, glowScale * 0.4f, 0, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            RenderGlow();

            MiscShaderData rayShader = GameShaders.Misc["CalamityMod:SylvestaffProjectile"];

            rayShader.SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new PrimitiveSettings(WidthFunction, ColorFunction, OffsetFunction, pixelate: false, shader: rayShader), 32);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.hostile)
                target.AddBuff(ModContent.BuffType<FabsolVodkaBuff>(), 54000);
        }
    }
}
