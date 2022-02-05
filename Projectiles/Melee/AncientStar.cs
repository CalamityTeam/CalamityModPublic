using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class AncientStar : ModProjectile
    {
		const float MaxTime = 120;
		public float Timer => MaxTime - projectile.timeLeft;

		public ref float Shine => ref projectile.ai[0];

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Star");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.melee = true;
            projectile.penetrate = 1;
            projectile.timeLeft = (int)MaxTime;
        }

        public override void AI()
        {
			if (Timer == 0)
            {
				Particle spark = new CritSpark(projectile.Center, Vector2.Zero, Color.White, Color.Cyan, Main.rand.NextFloat(2.3f, 2.6f), 20, 0.1f, 1.5f, hueShift: 0.02f);
				GeneralParticleHandler.SpawnParticle(spark);

				for (int i = 0; i < 4; i++)
				{
					Vector2 particleSpeed = projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.8f) * 0.1f;
					Particle energyLeak = new SquishyLightParticle(projectile.Center, particleSpeed, Main.rand.NextFloat(0.8f, 1.4f), Color.Cyan, 60, 0.3f, 1.5f, hueShift: 0.02f);
					GeneralParticleHandler.SpawnParticle(energyLeak);
				}
			}

			if (projectile.timeLeft < MaxTime - 5)
            {
				projectile.tileCollide = true;
            }

			if (Timer / MaxTime > 0.25f)
            {
				projectile.velocity *= 0.95f;
            }

			int dustType = Main.rand.Next(3);
			dustType = dustType == 0 ? 15 : dustType == 1 ? 57 : 58;
			Dust.NewDust(projectile.Center, 14, 14, dustType, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, default, 1.3f);

			if (projectile.soundDelay == 0)
			{
				projectile.soundDelay = 20 + Main.rand.Next(40);
				if (Main.rand.NextBool(5))
				{
					Main.PlaySound(SoundID.Item9, projectile.position);
				}
			}

			if (Main.rand.NextBool(48))
			{
				int starGore = Gore.NewGore(projectile.Center, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), 16, 1f);
				Main.gore[starGore].velocity *= 0.66f;
				Main.gore[starGore].velocity += projectile.velocity * 0.3f;
			}

			if (projectile.velocity.Length() < 2f) 
			{
				projectile.Kill();
				return;
			}
				
			projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;

			CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 200f, 12f, 20f);
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);

			if (Shine == 1f)
            {
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

				Texture2D starTexture = ModContent.GetTexture("CalamityMod/Particles/Sparkle");
				Texture2D bloomTexture = ModContent.GetTexture("CalamityMod/Particles/BloomCircle");
				//Ajust the bloom's texture to be the same size as the star's
				float properBloomSize = (float)starTexture.Height / (float)bloomTexture.Height;

				Color color = Main.hslToRgb((Main.GlobalTime * 0.6f) % 1, 1, 0.85f);
				float rotation = Main.GlobalTime * 8f;
				Vector2 sparkCenter = projectile.Center - Main.screenPosition;

				spriteBatch.Draw(bloomTexture, sparkCenter, null, color * 0.5f, 0, bloomTexture.Size() / 2f, 2 * properBloomSize, SpriteEffects.None, 0);
				spriteBatch.Draw(starTexture, sparkCenter, null, color * 0.5f, rotation + MathHelper.PiOver4, starTexture.Size() / 2f, 1 * 0.75f, SpriteEffects.None, 0);
				spriteBatch.Draw(starTexture, sparkCenter, null, Color.White, rotation, starTexture.Size() / 2f, 1, SpriteEffects.None, 0);

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
			}

			return false;
		}

		public override void Kill(int timeLeft)
		{
			var breakSound = Main.PlaySound(SoundID.DD2_WitherBeastDeath, projectile.Center);
			if (breakSound != null)
				breakSound.Volume *= 0.5f;

			for (int i = 0; i < 5; i++)
			{
				Vector2 particleSpeed = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(5.2f, 4.3f);
				Particle spark = new CritSpark(projectile.Center, particleSpeed, Color.White, Color.Cyan, Main.rand.NextFloat(1.3f, 1.6f), 40, 0.1f, 3.5f, hueShift: 0.02f);
				GeneralParticleHandler.SpawnParticle(spark);
			}

			for (int i = 0; i < 3; i++)
			{
				Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
			}
		}
    }
}
