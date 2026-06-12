using System;
using System.Linq;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Rogue
{
    public class WulfrumKnifeProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        internal Color PrimColorMult = Color.White;
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/WulfrumKnife";

        private bool beginStretchAnim = false;
        private float progress = -1f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public static int Lifetime = 1440;
        public float LifetimeCompletion => MathHelper.Clamp((Lifetime - Projectile.timeLeft) / (float)Lifetime, 0f, 1f);

        public float StuckEnemyID 
        { 
            get { return Projectile.ai[0]; } 
            set { Projectile.ai[0] = value; }
        }
        public float StuckEnemyDistance
        {
            get { return Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }
        public float StuckEnemyRotation
        {
            get { return Projectile.ai[2]; }
            set { Projectile.ai[2] = value; }
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = RogueDamageClass.Instance;

            Projectile.timeLeft = Lifetime;
            Projectile.extraUpdates = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override void AI()
        {
            if (Projectile.timeLeft >= 50)
                Projectile.Opacity = MathHelper.Clamp(LifetimeCompletion * 90f, 0f, 1f);
            else
                Projectile.Opacity = Projectile.timeLeft / 50f; // Fade out over last 50 frames

            // Update squash and stretch animation
            if (progress >= 0f && progress < 1f)
                progress += 0.06f;

            if (StuckEnemyID > 0)
            {
                Projectile.tileCollide = false;
                if (!Main.npc[(int)StuckEnemyID-1].active)
                {
                    StuckEnemyID = 0;
                    Projectile.velocity = -Vector2.UnitY.RotatedByRandom(0.25f) * Main.rand.NextFloat(0, 1f);
                    Projectile.tileCollide = true;
                    return;
                }
                Projectile.Center = Main.npc[(int)StuckEnemyID-1].Center + Vector2.UnitX.RotatedBy(StuckEnemyRotation) * StuckEnemyDistance;
                return;
            }
            if (StuckEnemyID == -1)
            {
                var player = Main.player[Projectile.owner];
                Projectile.velocity = Projectile.DirectionTo(player.Center) * 10f;
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
                if (Main.rand.NextBool(3))
                {
                    Vector2 dustCenter = Projectile.Center + Main.rand.NextVector2Circular(4f, 4f);

                    Dust chust = Dust.NewDustPerfect(dustCenter, DustID.MagicMirror, -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.1f), Scale: Main.rand.NextFloat(1.2f, 1.8f));
                    chust.noGravity = true;
                }
                if (Projectile.Distance(player.Center) < 16)
                {
                    //Gives 1 second of armorless stealth usage
                    player.Calamity().temporaryStealthTimer = 60;
                    if (player.Calamity().rogueStealthMax < 0.1f)
                        player.Calamity().rogueStealthMax = 0.1f;
                    player.Calamity().rogueStealth += player.Calamity().rogueStealthMax * 0.084f;
                    Projectile.Kill();
                }
                return;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity *= 0.998f;

            if (Projectile.timeLeft < Lifetime - 100)
                Projectile.velocity.Y += 0.01f;


            if (!Projectile.Calamity().stealthStrike)
            {
                if (Main.rand.NextBool(12))
                {
                    Vector2 dustCenter = Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(-3f, 3f);

                    Dust chust = Dust.NewDustPerfect(dustCenter, DustID.MagicMirror, -Projectile.velocity * Main.rand.NextFloat(0.6f, 1.5f), Scale: Main.rand.NextFloat(1f, 1.4f));
                    chust.noGravity = true;

                    if (!Main.rand.NextBool(5))
                        chust.noLightEmittence = true;
                }
            }

            else
            {
                Lighting.AddLight(Projectile.Center, (Main.rand.NextBool() ? Color.GreenYellow : Color.DeepSkyBlue).ToVector3());

                if (Main.rand.NextBool(7))
                {
                    Vector2 center = Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(-3f, 3f);
                    Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(MathHelper.Pi / 6f) * Main.rand.NextFloat(4, 10);
                    GeneralParticleHandler.SpawnParticle(new TechyHoloysquareParticle(center, velocity, Main.rand.NextFloat(1f, 2f), Main.rand.NextBool() ? new Color(99, 255, 229) : new Color(25, 132, 247), 25, Projectile.Opacity));

                }

                if (Main.rand.NextBool(8))
                {
                    Vector2 dustCenter = Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(-3f, 3f);

                    Dust chust = Dust.NewDustPerfect(dustCenter, DustID.MagicMirror, -Projectile.velocity * Main.rand.NextFloat(0.6f, 1.5f), Scale: Main.rand.NextFloat(1f, 1.4f));
                    chust.noGravity = true;
                    chust.noLightEmittence = true;
                }
            }

            if (Projectile.timeLeft < 120)
                Projectile.Opacity -= 0.03f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(WulfrumKnife.TileHitSound, Projectile.Center);
            Projectile.timeLeft = Math.Max(Projectile.timeLeft, 300);
            StuckEnemyID = target.whoAmI+1;
            StuckEnemyDistance = Projectile.Distance(target.Center);
            StuckEnemyRotation = Projectile.DirectionFrom(target.Center).ToRotation();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            beginStretchAnim = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(WulfrumKnife.TileHitSound, Projectile.Center);

            return base.OnTileCollide(oldVelocity);
        }

        internal Color ColorFunction(float completionRatio, Vector2 vertexPos)
        {
            float fadeOpacity = (float)MathHelper.Clamp(LifetimeCompletion * 30f, 0f, 1f);
            return Color.GreenYellow.MultiplyRGB(PrimColorMult) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio, Vector2 vertexPos)
        {
            return 9.4f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;

            // Squash and Stretch Scale Logic
            if (beginStretchAnim)
            {
                progress = 0f;
                beginStretchAnim = false;
            }

            float stretchFactorX = 1f;
            float stretchFactorY = 1f;

            if (progress >= 0f)
            {
                if (progress < 0.5f) // Stretch phase
                {
                    float completion = progress / 0.5f;
                    stretchFactorX = MathHelper.Lerp(1.6f, 0.7f, completion);
                    stretchFactorY = MathHelper.Lerp(0.7f, 1.6f, completion);
                }
                else // Squash phase
                {
                    float completion = (progress - 0.5f) / 0.5f;
                    stretchFactorX = MathHelper.Lerp(0.7f, 1f, completion);
                    stretchFactorY = MathHelper.Lerp(1.6f, 1f, completion);
                }

                if (progress >= 1f)
                {
                    progress = -1f;
                }
            }

            Vector2 drawScale = new Vector2(stretchFactorX, stretchFactorY) * Projectile.scale;
            Vector2 offset = Projectile.Size * 0.5f; 
            Vector2[] oldPosWithOffset = Projectile.oldPos.Select(p => p - offset).ToArray();

            if (Projectile.Calamity().stealthStrike && StuckEnemyID == 0)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/Trails/BasicTrail"));
                  
                CalamityUtils.DrawChromaticAberration(Vector2.UnitX, 1f, delegate (Vector2 offset, Color colorMod)
                {
                    PrimColorMult = colorMod;
                    PrimitiveRenderer.RenderTrail(oldPosWithOffset, new(WidthFunction, ColorFunction, (_,_) => Projectile.Size + offset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 30);
                });

                CalamityUtils.DrawChromaticAberration(Vector2.UnitX, 3f, delegate (Vector2 offset, Color colorMod)
                {
                    Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + offset, null, Color.GreenYellow.MultiplyRGB(colorMod) * Projectile.Opacity, Projectile.rotation, tex.Size() * 0.5f, drawScale, SpriteEffects.None, 0);
                });

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, tex.Size() * 0.5f, drawScale, SpriteEffects.None, 0);
            }
            else
            {
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, tex.Size() * 0.5f, drawScale, SpriteEffects.None, 0);
            }
            return false;
        }

    }
}
