using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Melee
{
    public class ElementalGlassStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        const float MaxTime = 120;
        public float Timer => MaxTime - Projectile.timeLeft;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = (int)MaxTime;
            Projectile.scale = 1.5f;
        }

        public override void AI()
        {
            if (Timer == 0)
            {
                Particle spark = new FlareShine(Projectile.Center, Vector2.Zero, Color.White, Color.OrangeRed, 0f,new Vector2(0.5f, 1f), new Vector2(1.5f, 3f), 20, 0f, hueShift: 0.01f);
                GeneralParticleHandler.SpawnParticle(spark);

                for (int i = 0; i < 2; i++)
                {
                    Vector2 particleSpeed = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.8f) * 0.1f;
                    Particle energyLeak = new SquishyLightParticle(Projectile.Center, particleSpeed, Main.rand.NextFloat(0.8f, 1.4f), Color.Cyan, 60, 0.3f, 1.5f, hueShift: 0.02f);
                    GeneralParticleHandler.SpawnParticle(energyLeak);
                }
            }

            if (Projectile.timeLeft < MaxTime - 5)
            {
                Projectile.tileCollide = true;
            }

            if (Timer / MaxTime > 0.25f)
            {
                Projectile.velocity *= 0.95f;
            }

            if (Main.rand.Next(3) == 0)
            {
                int dustType = Main.rand.Next(3);
                dustType = dustType == 0 ? 15 : dustType == 1 ? 57 : 58;
                Dust.NewDust(Projectile.Center, 14, 14, dustType, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default, 1.3f);
            }

            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
                }
            }

            if (Main.rand.NextBool(48) && Main.netMode != NetmodeID.Server)
            {
                int starGore = Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f), 16, 1f);
                Main.gore[starGore].velocity *= 0.66f;
                Main.gore[starGore].velocity += Projectile.velocity * 0.3f;
            }

            if (Projectile.velocity.Length() < 2f && Projectile.timeLeft < (int)(MaxTime / 2f))
            {
                Projectile.Kill();
                return;
            }

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 350f, 12f, 20f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D starTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/Sparkle").Value;
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            //Ajust the bloom's texture to be the same size as the star's
            float properBloomSize = (float)starTexture.Height / (float)bloomTexture.Height;

            Color color = Main.hslToRgb((float)Math.Sin(Main.GlobalTimeWrappedHourly * 15f) * 0.05f + 0.08f, 1, 0.55f);
            float rotation = Main.GlobalTimeWrappedHourly * 8f;
            Vector2 sparkCenter = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(bloomTexture, sparkCenter, null, color * 0.6f, 0, bloomTexture.Size() / 2f, 4f * properBloomSize, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(starTexture, sparkCenter, null, color, rotation + MathHelper.PiOver4, starTexture.Size() / 2f, 2f * 0.75f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(starTexture, sparkCenter, null, Color.White, rotation, starTexture.Size() / 2f, 2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }

        public override void OnKill(int timeLeft)
        {
            var breakSound = SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath with { Volume = SoundID.DD2_WitherBeastDeath.Volume * 0.5f }, Projectile.Center);

            for (int i = 0; i < 2; i++)
            {
                Vector2 particleSpeed = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(5.2f, 4.3f);
                Particle spark = new CritSpark(Projectile.Center, particleSpeed, Color.White, Color.Cyan, Main.rand.NextFloat(1.3f, 1.6f), 40, 0.1f, 3.5f, hueShift: 0.02f);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 3; i++)
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, new Vector2(Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 60);
        }
    }
}
