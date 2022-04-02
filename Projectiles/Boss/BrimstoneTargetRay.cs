using CalamityMod.NPCs.BrimstoneElemental;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
	public class BrimstoneTargetRay : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Target Ray");
        }

        public override void SetDefaults()
        {
			projectile.width = 10;
			projectile.height = 10;
			projectile.hostile = true;
			projectile.alpha = 255;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.timeLeft = 600;
			projectile.scale = 0.1f;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
			writer.Write(projectile.localAI[1]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
			projectile.localAI[1] = reader.ReadSingle();
		}

		public override void AI()
		{
			Vector2? vector78 = null;

			if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
				projectile.velocity = -Vector2.UnitY;

			if (Main.npc[(int)projectile.ai[1]].active && Main.npc[(int)projectile.ai[1]].type == ModContent.NPCType<BrimstoneElemental>())
			{
				Vector2 fireFrom = new Vector2(Main.npc[(int)projectile.ai[1]].Center.X + (Main.npc[(int)projectile.ai[1]].spriteDirection > 0 ? 34f : -34f), Main.npc[(int)projectile.ai[1]].Center.Y - 74f);
				projectile.position = fireFrom - new Vector2(projectile.width, projectile.height) / 2f;
			}
			else
				projectile.Kill();

			Vector2 laserVelocity = new Vector2(Main.npc[(int)projectile.ai[1]].Calamity().newAI[1], Main.npc[(int)projectile.ai[1]].Calamity().newAI[2]);
			float rotationVelocity = projectile.ai[0] == 0f ? laserVelocity.ToRotation() : projectile.velocity.ToRotation();
			projectile.rotation = rotationVelocity - MathHelper.PiOver2;
			projectile.velocity = rotationVelocity.ToRotationVector2();

			if (projectile.ai[0] == 0f)
			{
				if (Main.npc[(int)projectile.ai[1]].ai[1] >= 150f)
				{
					projectile.Kill();
					return;
				}
			}
			else
			{
				if (Main.npc[(int)projectile.ai[1]].ai[1] >= 180f)
				{
					projectile.Kill();
					return;
				}
			}

			float num805 = 3f; //3f
			float num806 = projectile.width;

			Vector2 samplingPoint = projectile.Center;
			if (vector78.HasValue)
			{
				samplingPoint = vector78.Value;
			}

			float[] array3 = new float[(int)num805];
			Collision.LaserScan(samplingPoint, projectile.velocity, num806 * projectile.scale, 2400f, array3);
			float num807 = 0f;
			for (int num808 = 0; num808 < array3.Length; num808++)
			{
				num807 += array3[num808];
			}
			num807 /= num805;

			// Fire laser through walls at max length if target cannot be seen
			if (!Collision.CanHitLine(Main.npc[(int)projectile.ai[1]].Center, 1, 1, Main.player[Main.npc[(int)projectile.ai[1]].target].Center, 1, 1))
			{
				num807 = 2400f;
			}

			float amount = 0.5f; //0.5f
			projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], num807, amount); //length of laser, linear interpolation

			DelegateMethods.v3_1 = new Vector3(0.9f, 0.3f, 0.3f);
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (projectile.velocity == Vector2.Zero)
			{
				return false;
			}
			Texture2D texture2D19 = Main.projectileTexture[projectile.type];
			Texture2D texture2D20 = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/BrimstoneRayMid");
			Texture2D texture2D21 = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/BrimstoneRayEnd");
			float num223 = projectile.localAI[1]; //length of laser
			Color color44 = new Color(255, 255, 255, 0) * 0.9f;
			Vector2 vector = projectile.Center - Main.screenPosition;
			Rectangle? sourceRectangle2 = null;
			spriteBatch.Draw(texture2D19, vector, sourceRectangle2, color44, projectile.rotation, texture2D19.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
			num223 -= (texture2D19.Height / 2 + texture2D21.Height) * projectile.scale;
			Vector2 value20 = projectile.Center;
			value20 += projectile.velocity * projectile.scale * texture2D19.Height / 2f;
			if (num223 > 0f)
			{
				float num224 = 0f;
				Rectangle rectangle7 = new Rectangle(0, 16 * (projectile.timeLeft / 3 % 5), texture2D20.Width, 16);
				while (num224 + 1f < num223)
				{
					if (num223 - num224 < rectangle7.Height)
					{
						rectangle7.Height = (int)(num223 - num224);
					}
					spriteBatch.Draw(texture2D20, value20 - Main.screenPosition, new Rectangle?(rectangle7), color44, projectile.rotation, new Vector2(rectangle7.Width / 2, 0f), projectile.scale, SpriteEffects.None, 0f);
					num224 += rectangle7.Height * projectile.scale;
					value20 += projectile.velocity * rectangle7.Height * projectile.scale;
					rectangle7.Y += 16;
					if (rectangle7.Y + rectangle7.Height > texture2D20.Height)
					{
						rectangle7.Y = 0;
					}
				}
			}
			Vector2 vector2 = value20 - Main.screenPosition;
			sourceRectangle2 = null;
			spriteBatch.Draw(texture2D21, vector2, sourceRectangle2, color44, projectile.rotation, texture2D21.Frame(1, 1, 0, 0).Top(), projectile.scale, SpriteEffects.None, 0f);
			return false;
		}

		public override void CutTiles()
		{
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Vector2 unit = projectile.velocity;
			Utils.PlotTileLine(projectile.Center, projectile.Center + unit * projectile.localAI[1], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (projHitbox.Intersects(targetHitbox))
			{
				return true;
			}
			float num6 = 0f;
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], 22f * projectile.scale, ref num6))
			{
				return true;
			}
			return false;
		}

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
		{
			target.Calamity().lastProjectileHit = projectile;
		}
	}
}
