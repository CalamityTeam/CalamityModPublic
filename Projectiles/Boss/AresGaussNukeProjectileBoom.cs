using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class AresGaussNukeProjectileBoom : ModProjectile
	{
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

		public ScreenShakeSpot CurrentSpot;

		public float Radius
		{
			get => projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public float MaxRadius
		{
			get => projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		public float InterpolationStep
		{
			get => projectile.localAI[1];
			set => projectile.localAI[1] = value;
		}

		public const int Lifetime = 60;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gauss Explosion");
			ProjectileID.Sets.NeedsUUID[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 2;
			projectile.height = 2;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			cooldownSlot = 1;
			projectile.timeLeft = Lifetime;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
			writer.Write(projectile.localAI[1]);
			writer.Write(CurrentSpot.ScreenShakePower);
			writer.WriteVector2(CurrentSpot.Position);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
			projectile.localAI[1] = reader.ReadSingle();
			CurrentSpot = new ScreenShakeSpot(reader.ReadInt32(), reader.ReadVector2());
		}

		public override void AI()
		{
			if (projectile.localAI[0] == 0f)
			{
				CurrentSpot = new ScreenShakeSpot(0, projectile.Center);
				projectile.localAI[0] = 1f;
			}

			CurrentSpot.ScreenShakePower = (float)Math.Sin(MathHelper.Pi * projectile.timeLeft / Lifetime) * 16f;
			CurrentSpot.Position = projectile.Center;
			CalamityWorld.ScreenShakeSpots[Projectile.GetByUUID(projectile.owner, projectile.whoAmI)] = CurrentSpot;

			Lighting.AddLight(projectile.Center, 0.2f, 0.1f, 0f);
			Radius = MathHelper.Lerp(Radius, MaxRadius, 0.25f);
			projectile.scale = MathHelper.Lerp(1.2f, 5f, Utils.InverseLerp(Lifetime, 0f, projectile.timeLeft, true));
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, (int)(Radius * projectile.scale), (int)(Radius * projectile.scale));
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 480);
		}

		public override void Kill(int timeLeft)
		{
			CalamityWorld.ScreenShakeSpots.Remove(Projectile.GetByUUID(projectile.owner, projectile.whoAmI)); // Remove the explosion associated with this projectile's UUID.
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

			float pulseCompletionRatio = Utils.InverseLerp(Lifetime, 0f, projectile.timeLeft, true);
			Vector2 scale = new Vector2(1.5f, 1f);
			DrawData drawData = new DrawData(ModContent.GetTexture("Terraria/Misc/Perlin"),
				projectile.Center - Main.screenPosition + projectile.Size * scale * 0.5f,
				new Rectangle(0, 0, projectile.width, projectile.height),
				new Color(new Vector4(1f - (float)Math.Sqrt(pulseCompletionRatio))) * 0.7f * projectile.Opacity,
				projectile.rotation,
				projectile.Size,
				scale,
				SpriteEffects.None, 0);

			Color pulseColor = Color.Lerp(Color.Yellow * 1.6f, Color.White, MathHelper.Clamp(pulseCompletionRatio * 2.2f, 0f, 1f));
			GameShaders.Misc["ForceField"].UseColor(pulseColor);
			GameShaders.Misc["ForceField"].Apply(drawData);
			drawData.Draw(spriteBatch);

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}

		public override bool CanHitPlayer(Player target)
		{
			Rectangle targetHitbox = target.Hitbox;

			float dist1 = Vector2.Distance(projectile.Center, targetHitbox.TopLeft());
			float dist2 = Vector2.Distance(projectile.Center, targetHitbox.TopRight());
			float dist3 = Vector2.Distance(projectile.Center, targetHitbox.BottomLeft());
			float dist4 = Vector2.Distance(projectile.Center, targetHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist)
				minDist = dist2;
			if (dist3 < minDist)
				minDist = dist3;
			if (dist4 < minDist)
				minDist = dist4;

			return minDist <= Radius * projectile.scale * 0.5f;
		}

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
		{
			target.Calamity().lastProjectileHit = projectile;
		}
	}
}
