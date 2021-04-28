using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Enemy
{
    public class CragmawVibeCheckChain : ModProjectile
    {
		public bool ReelingPlayer = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("You Think You're Safe");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 12;
			projectile.alpha = 255;
            projectile.timeLeft = 420;
			projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
			writer.Write(ReelingPlayer);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
			ReelingPlayer = reader.ReadBoolean();
		}
		public override void AI()
        {
			Vector2 offsetDrawVector = new Vector2(0f, 30f);
			projectile.alpha -= 15;
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}
			int toTarget = (int)projectile.ai[1];
			if (!Main.npc[(int)projectile.ai[0]].active)
			{
				projectile.Kill();
				return;
			}
			projectile.rotation = (Main.npc[(int)projectile.ai[0]].Top - projectile.Center + offsetDrawVector).ToRotation() + MathHelper.PiOver2;
			if (projectile.localAI[0] == 0f)
			{
				projectile.velocity = (projectile.velocity * 10f + projectile.SafeDirectionTo(Main.player[toTarget].Center) * 9f) / 11f;
				if (projectile.timeLeft <= 180f)
				{
					projectile.localAI[0] = 1f;
					projectile.netUpdate = true;
				}
				if (projectile.WithinRange(Main.player[toTarget].Center, 16f))
				{
					projectile.localAI[0] = 1f;
					ReelingPlayer = true;
					projectile.netUpdate = true;
				}
			}
			else
			{
				if (ReelingPlayer)
					Main.player[toTarget].Center = projectile.Center;

				if (Main.player[toTarget].dead)
					ReelingPlayer = false;

				if (!projectile.WithinRange(Main.npc[(int)projectile.ai[0]].Center, 6f))
					projectile.velocity = (projectile.velocity * 16f + projectile.SafeDirectionTo(Main.npc[(int)projectile.ai[0]].Center) * 19f) / 17f;
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D endTexture = ModContent.GetTexture(Texture);
			Texture2D chainTexture = ModContent.GetTexture("CalamityMod/Projectiles/Enemy/CragmawVibeCheckMid");
			Vector2 distanceVectorToStart = Main.npc[(int)projectile.ai[0]].Top + Vector2.UnitY * 30f - projectile.Center;
			float distanceToStart = distanceVectorToStart.Length();
			Vector2 directionToStart = Vector2.Normalize(distanceVectorToStart);
			Rectangle frameRectangle = endTexture.Frame(1, 1, 0, 0);
			frameRectangle.Height /= 4;
			frameRectangle.Y += projectile.frame * frameRectangle.Height;

			spriteBatch.Draw(endTexture, projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(frameRectangle), projectile.GetAlpha(lightColor), projectile.rotation, frameRectangle.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);

			distanceToStart -= (frameRectangle.Height / 2 + chainTexture.Height) * projectile.scale;
			Vector2 chainDrawPosition = projectile.Center;
			chainDrawPosition += directionToStart * projectile.scale * frameRectangle.Height / 2f;
			if (distanceToStart > 0f)
			{
				float distanceMoved = 0f;
				Rectangle chainTextureFrameRectangle = new Rectangle(0, 0, chainTexture.Width, chainTexture.Height);
				while (distanceMoved + 1f < distanceToStart)
				{
					if (distanceToStart - distanceMoved < chainTextureFrameRectangle.Height)
					{
						chainTextureFrameRectangle.Height = (int)(distanceToStart - distanceMoved);
					}
					Point chainPositionTileCoords = chainDrawPosition.ToTileCoordinates();
					Color colorAtChainPosition = Lighting.GetColor(chainPositionTileCoords.X, chainPositionTileCoords.Y);
					colorAtChainPosition = Color.Lerp(colorAtChainPosition, Color.White, 0.3f);
					spriteBatch.Draw(chainTexture, chainDrawPosition - Main.screenPosition, new Rectangle?(chainTextureFrameRectangle), projectile.GetAlpha(colorAtChainPosition), projectile.rotation, chainTextureFrameRectangle.Bottom(), projectile.scale, SpriteEffects.None, 0f);
					distanceMoved += chainTextureFrameRectangle.Height * projectile.scale;
					chainDrawPosition += directionToStart * chainTextureFrameRectangle.Height * projectile.scale;
				}
			}
			return false;
        }
    }
}
