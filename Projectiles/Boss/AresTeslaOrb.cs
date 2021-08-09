using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AresTeslaOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tesla Orb");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
			projectile.width = 32;
            projectile.height = 32;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
			projectile.Opacity = 0f;
			cooldownSlot = 1;
			projectile.timeLeft = 480;
			projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
		}

		public override void AI()
        {
			if (projectile.timeLeft < 15)
				projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 15f, 0f, 1f);
			else
				projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - 477) / 3f), 0f, 1f);

			Lighting.AddLight(projectile.Center, 0.1f * projectile.Opacity, 0.25f * projectile.Opacity, 0.25f * projectile.Opacity);

			projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;

				float speed1 = 1.8f;
				float speed2 = 2.8f;
				float angleRandom = 0.35f;

				for (int num53 = 0; num53 < 40; num53++)
				{
					float dustSpeed = Main.rand.NextFloat(speed1, speed2);
					Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
					dustVel = dustVel.RotatedBy(-angleRandom);
					dustVel = dustVel.RotatedByRandom(2f * angleRandom);
					int randomDustType = Main.rand.Next(2) == 0 ? 206 : 229;
					float scale = randomDustType == 206 ? 1.5f : 1f;

					int num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 2.5f * scale);
					Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;
					Main.dust[num54].noGravity = true;

					Dust dust = Main.dust[num54];
					dust.velocity *= 3f;
					dust = Main.dust[num54];

					num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 1.5f * scale);
					Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;

					dust = Main.dust[num54];
					dust.velocity *= 2f;

					Main.dust[num54].noGravity = true;
					Main.dust[num54].fadeIn = 1f;
					Main.dust[num54].color = Color.Cyan * 0.5f;

					dust = Main.dust[num54];
				}
				for (int num55 = 0; num55 < 20; num55++)
				{
					float dustSpeed = Main.rand.NextFloat(speed1, speed2);
					Vector2 dustVel = new Vector2(dustSpeed, 0f).RotatedBy(projectile.velocity.ToRotation());
					dustVel = dustVel.RotatedBy(-angleRandom);
					dustVel = dustVel.RotatedByRandom(2f * angleRandom);
					int randomDustType = Main.rand.Next(2) == 0 ? 206 : 229;
					float scale = randomDustType == 206 ? 1.5f : 1f;

					int num56 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 3f * scale);
					Main.dust[num56].position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(projectile.velocity.ToRotation()) * projectile.width / 3f;
					Main.dust[num56].noGravity = true;

					Dust dust = Main.dust[num56];
					dust.velocity *= 0.5f;
					dust = Main.dust[num56];
				}
			}
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity = oldVelocity;

			if (projectile.timeLeft > 15)
				projectile.timeLeft = 15;

			return false;
		}

		public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			if (projectile.Opacity != 1f)
				return;

			target.AddBuff(BuffID.Electrified, 240);
		}

		public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 * projectile.Opacity, 255 * projectile.Opacity, 255 * projectile.Opacity);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
		{
			target.Calamity().lastProjectileHit = projectile;
		}
	}
}
