using System;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Particles;
using CalamityMod.Graphics.Primitives;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace CalamityMod.Projectiles.Magic
{
    public class IgnisSigilFireball : ModProjectile, ILocalizedModType
    {
        public ref float Time => ref Projectile.ai[0];

        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";


        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 9;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 92;
            Projectile.height = 92;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1; // Explodes on a delay after hitting
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Time++;

            Projectile.Opacity = Utils.GetLerpValue(0f, 10f, Time, true);

            // Hasnt hit anything
            if (Projectile.ai[1] == 0)
            {
                // Set rotation to face forward
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

                Projectile.frameCounter++;
                if (Projectile.frameCounter % 4 == 3)
                    Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }
            else // After hit
            {
                Projectile.velocity = Vector2.Zero;

                // Scale up and die
                Projectile.scale += 0.2f;
                if (Projectile.scale >= 1.75f)
                {
                    Projectile.Kill();
                }
                // Failsafe
                if (Projectile.timeLeft > 5)
                {
                    Projectile.timeLeft = 5;
                }
            }

            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.22f / 255f, (255 - Projectile.alpha) * 0.05f / 255f, (255 - Projectile.alpha) * 0.05f / 255f);
            Projectile.scale -= 0.01f;

            if (Projectile.scale <= 0f)
            {
                Projectile.Kill();
            }

            if (Projectile.ai[0] <= 3f)
            {
                Projectile.ai[0] += 1f;
                return;
            }

            GeneralParticleHandler.SpawnParticle(new SquishyLightParticle(Projectile.Center, Projectile.velocity, Projectile.scale * 1.5f, Color.OrangeRed, 3, 1f, 0f, 1f));

            // Gravity!
            Projectile.velocity.Y = Projectile.velocity.Y + 0.125f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 180);
            Projectile.ai[1] = 1; // Has hit an enemy
            Projectile.penetrate = -1;
        }

        // Standard trail drawing logic
        private float WidthFunction(float completionRatio, Vector2 vertexPos) => MathHelper.Lerp(0f, MathHelper.Lerp(Projectile.scale * 132f, 0f, completionRatio), MathF.Pow(completionRatio, 1f / 2.5f));
        private Color ColorFunction(float completionRatio, Vector2 vertexPos)
        {
            float offsetTime = Main.GlobalTimeWrappedHourly;
            float fadeOpacity = Utils.GetLerpValue(0.5f, 0f, completionRatio, true) * Projectile.Opacity;
            Color endColor = Color.IndianRed;
            return Color.Lerp(endColor, Color.Orange, completionRatio) * fadeOpacity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new PrimitiveSettings(WidthFunction, ColorFunction, (_,_) => Projectile.Size * 0.5f, pixelate: false, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 340);


            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Type];
            Rectangle sourceRect = new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRect, Color.White, Projectile.rotation, sourceRect.Size() * 0.5f, 1f, 0);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/UnstableCastersGauntlet/IgnisSigilHit") { Volume = 0.7f, PitchVariance = 0.1f }, Projectile.Center);

            for (int i = 0; i < 40; i++)
            {
                Vector2 vel = new Vector2(10, 10).RotatedByRandom(100) * Main.rand.NextFloat(0.85f, 1.2f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + (new Vector2(Main.rand.NextFloat(-22, -30), Main.rand.NextFloat(-4, 4)).RotatedBy(Projectile.rotation - MathHelper.PiOver2)), Main.rand.NextBool(5) ? 174 : 35, vel * Main.rand.NextFloat(0.1f, 0.9f) + new Vector2(0, -2));
                dust.noGravity = false;
                dust.scale = Main.rand.NextFloat(1.1f, 1.7f);
            }

            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = new Vector2(10, 10).RotatedByRandom(100) * Main.rand.NextFloat(0.85f, 1.2f);
                Projectile embers = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + (new Vector2(Main.rand.NextFloat(-22, -30), Main.rand.NextFloat(-4, 4)).RotatedBy(Projectile.rotation - MathHelper.PiOver2)), vel * 0.8f, ModContent.ProjectileType<IgnisSigilEmbers>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center + (new Vector2(-26, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2)), Vector2.Zero, Color.OrangeRed, "CalamityMod/Particles/FlameExplosion", Vector2.One, Main.rand.NextFloat(-10, 10), 0f, 0.265f, 21, true, 1f));

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile explosion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + (new Vector2(-26, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2)), Vector2.Zero, ModContent.ProjectileType<IgnisSigilFireballExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                explosion.ai[1] = 145f;
                explosion.localAI[1] = Main.rand.NextFloat(0.1f, 0.2f); // Interpolate
                explosion.netUpdate = true;
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(Color.LightGoldenrodYellow, Color.Orange, 0.5f) with { A = 0 } * Projectile.Opacity;
    }
}
