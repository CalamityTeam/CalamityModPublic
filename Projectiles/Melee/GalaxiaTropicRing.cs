using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Melee
{
    public class GalaxiaTropicRing : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public ref float Mode => ref projectile.ai[0];
        public ref float Fade => ref projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Ring");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Mode == 0f && Main.rand.NextFloat() < FourSeasonsGalaxia.CancerPassiveLifeStealProc)
            {
                Owner.statLife += FourSeasonsGalaxia.CancerPassiveLifeSteal;
                Owner.HealEffect(FourSeasonsGalaxia.CancerPassiveLifeSteal);
            }
            else
            {
                target.AddBuff(BuffType<ArmorCrunch>(), FourSeasonsGalaxia.CapricornPassiveDebuffTime);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - Vector2.One * 95 * projectile.scale, Vector2.One * 190 * projectile.scale);
        }

        public override void AI()
        {
            projectile.velocity *= 0.95f;

            if (Mode == 0f)
                projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center, 0.4f * MathHelper.Clamp((projectile.timeLeft - 150) / 150f, 0, 1));
            else
                projectile.velocity = Utils.SafeNormalize(projectile.Center - Owner.Center, Vector2.Zero) * MathHelper.Clamp((projectile.timeLeft - 150) / 150f, 0, 1) * 3f * (5f - MathHelper.Clamp((projectile.Center - Owner.Center).Length() / 150f, 0, 4f));

            Fade = projectile.timeLeft > 250 ? (float)Math.Sin((300 - projectile.timeLeft) / 50f * MathHelper.PiOver2) * 0.6f + 0.4f : projectile.timeLeft > 50 ? 1f : (float)Math.Sin((projectile.timeLeft) / 50f * MathHelper.PiOver2);
            Lighting.AddLight(projectile.Center, 0.75f, 1f, 0.24f);

        }

        public Vector2 starPosition(int MaxStars, int StarIndex, float diameter)
        {
            float starHeight = (float)Math.Sin(Main.GlobalTime * 3 + StarIndex * MathHelper.TwoPi / (float)MaxStars);
            float starWidth = (float)Math.Cos(Main.GlobalTime * 3 + StarIndex * MathHelper.TwoPi / (float)MaxStars);

            return projectile.Center + projectile.rotation.ToRotationVector2() * starWidth * (projectile.scale * diameter * 0.4f) + (projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * starHeight * (projectile.scale * diameter * 0.4f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sigil = GetTexture(Mode == 0 ? "CalamityMod/Projectiles/Melee/GalaxiaCancer" : "CalamityMod/Projectiles/Melee/GalaxiaCapricorn"); //OMG Karkat and Gamzee from homestuck i am a big fan
            Texture2D ring = GetTexture("CalamityMod/Particles/BloomRing");
            Texture2D starTexture = GetTexture("CalamityMod/Particles/Sparkle");
            Texture2D lineTexture = GetTexture("CalamityMod/Particles/BloomLine");
            Texture2D bloomTexture = GetTexture("CalamityMod/Particles/BloomCircle");

            Color ringColor = Mode == 0 ? new Color(255, 0, 0) * Fade : new Color(66, 0, 176) * Fade;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(bloomTexture, projectile.Center - Main.screenPosition, null, ringColor * 0.5f, 0, bloomTexture.Size() / 2f, 2.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(ring, projectile.Center - Main.screenPosition, null, ringColor * 0.8f, 0f, ring.Size() / 2f, projectile.scale, 0f, 0f);

            spriteBatch.Draw(sigil, projectile.Center - Main.screenPosition, null, ringColor * MathHelper.Lerp(0.7f, 0f, ((Main.GlobalTime * 5f) % 10) / 10f), 0f, sigil.Size() / 2f, projectile.scale + ((Main.GlobalTime * 5f) % 10) / 10f * 2f, 0f, 0f);
            spriteBatch.Draw(sigil, projectile.Center - Main.screenPosition, null, Color.White * Fade, 0f, sigil.Size() / 2f, projectile.scale, 0f, 0f);

            for (int i = 0; i < 5; i++)
            {

                Vector2 starPos = starPosition(5, i, ring.Width);

                Vector2 LineVector = starPosition(5, i + 1, ring.Width) - starPos;
                float rot = LineVector.ToRotation() + MathHelper.PiOver2;
                Vector2 origin = new Vector2(lineTexture.Width / 2f, lineTexture.Height);
                Vector2 scale = new Vector2(0.3f, LineVector.Length() / lineTexture.Height);

                spriteBatch.Draw(lineTexture, starPos - Main.screenPosition, null, Color.White * Fade * 0.7f, rot, origin, scale, SpriteEffects.None, 0);

                spriteBatch.Draw(bloomTexture, starPos - Main.screenPosition, null, ringColor * 0.5f, 0, bloomTexture.Size() / 2f, 0.1f, SpriteEffects.None, 0);
                spriteBatch.Draw(starTexture, starPos - Main.screenPosition, null, ringColor * 0.5f, Main.GlobalTime * 5 + MathHelper.PiOver4 * i, starTexture.Size() / 2f, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(starTexture, starPos - Main.screenPosition, null, Color.White * Fade, Main.GlobalTime * 5, starTexture.Size() / 2f, 1.4f, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}