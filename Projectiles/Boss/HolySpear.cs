using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class HolySpear : ModProjectile
    {
		Vector2 velocity = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Spear");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.alpha = 100;
            projectile.timeLeft = 900;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
			writer.WriteVector2(velocity);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
			velocity = reader.ReadVector2();
		}

        public override void AI()
        {
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
				Main.PlayTrackedSound(SoundID.DD2_BetsyFireballShot, projectile.Center);

				if (projectile.ai[0] == 1f)
					velocity = projectile.velocity;
			}

			if (projectile.ai[0] == 0f)
			{
				projectile.ai[1] += 1f;

				float slowGateValue = 90f;
				float fastGateValue = 30f;
				float minVelocity = 3f;
				float maxVelocity = 12f;
				float deceleration = 0.95f;
				float acceleration = 1.2f;

				if (projectile.ai[1] <= slowGateValue)
				{
					if (projectile.velocity.Length() > minVelocity)
						projectile.velocity *= deceleration;
				}
				else if (projectile.ai[1] < slowGateValue + fastGateValue)
				{
					if (projectile.velocity.Length() < maxVelocity)
						projectile.velocity *= acceleration;
				}
				else
					projectile.ai[1] = 0f;
			}
			else
			{
				float frequency = 0.1f;
				float amplitude = 2f;

				projectile.ai[1] += frequency;

				float wavyVelocity = (float)Math.Sin(projectile.ai[1]);

				projectile.velocity = velocity + new Vector2(wavyVelocity, wavyVelocity).RotatedBy(MathHelper.ToRadians(velocity.ToRotation())) * amplitude;
			}

			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft > 883)
            {
                projectile.localAI[1] += 5f;
                byte b2 = (byte)(((int)projectile.localAI[1]) * 3);
                byte a2 = (byte)(projectile.alpha * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(projectile.alpha * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            return new Color(255, 255, 255, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
