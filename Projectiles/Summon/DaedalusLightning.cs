using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
namespace CalamityMod.Projectiles.Summon
{
	public class DaedalusLightning : ModProjectile
    {
		// TODO: Continue cleaning this shit out.

		// Also works somewhat as a seed.
		public ref float BaseTurnAngleRatio => ref projectile.ai[1];

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Lightning");
			ProjectileID.Sets.MinionShot[projectile.type] = true;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
			projectile.minion = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
			projectile.ignoreWater = true;
            projectile.tileCollide = false;
			projectile.extraUpdates = 20;
            projectile.timeLeft = 75 * projectile.extraUpdates;
			cooldownSlot = 1;
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
			projectile.frameCounter++;

			Lighting.AddLight(projectile.Center, Color.Pink.ToVector3());
			if (projectile.frameCounter >= projectile.extraUpdates * 2)
			{
				projectile.frameCounter = 0;
				float originalSpeed = projectile.velocity.Length();
				UnifiedRandom unifiedRandom = new UnifiedRandom((int)BaseTurnAngleRatio);
				int num862 = 0;
				Vector2 vector97 = -Vector2.UnitY;
				Vector2 vector98;
				do
				{
					BaseTurnAngleRatio = unifiedRandom.Next() % 100;
					vector98 = (BaseTurnAngleRatio / 100f * MathHelper.TwoPi).ToRotationVector2();
					if (vector98.Y > 0f)
					{
						vector98.Y *= -1f;
					}
					bool flag37 = false;
					if (vector98.Y > -0.02f)
					{
						flag37 = true;
					}
					if (vector98.X * (projectile.extraUpdates + 1) * 2f * originalSpeed + projectile.localAI[0] > 40f)
					{
						flag37 = true;
					}
					if (vector98.X * (projectile.extraUpdates + 1) * 2f * originalSpeed + projectile.localAI[0] < -40f)
					{
						flag37 = true;
					}
					if (!flag37)
					{
						vector97 = vector98;
					}
					num862++;
				}
				while (num862 < 100);

				projectile.velocity = Vector2.Zero;
				projectile.localAI[1] = 1f;

				if (projectile.velocity != Vector2.Zero)
				{
					projectile.localAI[0] += vector97.X * (projectile.extraUpdates + 1) * 2f * originalSpeed;
					projectile.velocity = vector97.RotatedBy(projectile.ai[0] + MathHelper.PiOver2) * originalSpeed;
					projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			Vector2 end = projectile.Center + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
			Texture2D lightningTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/RedLightningTexture");
			projectile.GetAlpha(lightColor);
			Vector2 scaleVector = new Vector2(projectile.scale) / 2f;
			for (int i = 0; i < 3; i++)
			{
				// c_1 and f_1 are two somewhat ambiguous fields that are used by various methods
				// in the DelegateMethods class. c_1 is used to represent the current color to be
				// drawn while f_1 represents the opacity.
				switch (i)
				{
					case 0:
						scaleVector = new Vector2(projectile.scale) * 0.6f;
						DelegateMethods.c_1 = Color.HotPink * 0.5f;
						break;
					case 1:
						scaleVector = new Vector2(projectile.scale) * 0.4f;
						DelegateMethods.c_1 = Color.Pink * 0.5f;
						break;
					case 2:
						scaleVector = new Vector2(projectile.scale) * 0.2f;
						DelegateMethods.c_1 = Color.White * 0.5f;
						break;
				}
				DelegateMethods.f_1 = 1f;

				// Ignore zero oldPos indices.
				// There are almost guaranteed to be currently unfilled instead of being
				// legitimate positions.
				for (int j = projectile.oldPos.Length - 1; j > 0; j--)
				{
					if (!(projectile.oldPos[j] == Vector2.Zero))
					{
						Vector2 start = projectile.oldPos[j] + projectile.Size * 0.5f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
						Vector2 end2 = projectile.oldPos[j - 1] + projectile.Size * 0.5f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
						Utils.DrawLaser(spriteBatch, lightningTexture, start, end2, scaleVector, DelegateMethods.LightningLaserDraw);
					}
				}
				if (projectile.oldPos[0] != Vector2.Zero)
				{
					DelegateMethods.f_1 = 1f;
					Vector2 start2 = projectile.oldPos[0] + projectile.Size * 0.5f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
					Utils.DrawLaser(Main.spriteBatch, lightningTexture, start2, end, scaleVector, DelegateMethods.LightningLaserDraw);
				}
			}
			return false;
        }
	}
}
