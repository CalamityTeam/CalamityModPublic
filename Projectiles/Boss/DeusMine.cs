using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class DeusMine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Mine");
        }

        public override void SetDefaults()
        {
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.alpha = 100;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 900;
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
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 33);
            }

			if (projectile.timeLeft < 85)
				projectile.damage = 0;

			if (projectile.timeLeft < 815)
				return;

			float velocity = 0.1f;
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				if (Main.projectile[i].active)
				{
					if (i != projectile.whoAmI && Main.projectile[i].type == projectile.type)
					{
						if (Vector2.Distance(projectile.Center, Main.projectile[i].Center) < 48f)
						{
							if (projectile.position.X < Main.projectile[i].position.X)
								projectile.velocity.X -= velocity;
							else
								projectile.velocity.X += velocity;

							if (projectile.position.Y < Main.projectile[i].position.Y)
								projectile.velocity.Y -= velocity;
							else
								projectile.velocity.Y += velocity;
						}
						else
							projectile.velocity = Vector2.Zero;
					}
				}
			}
		}

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 12f, targetHitbox);

		public override bool CanHitPlayer(Player target) => projectile.timeLeft <= 815 && projectile.timeLeft >= 85;

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft > 815)
            {
                projectile.localAI[1] += 1f;
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

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, (int)projectile.position.X, (int)projectile.position.Y);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 96;
            projectile.position.X = projectile.position.X - (projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (projectile.height / 2);
            for (int num621 = 0; num621 < 10; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default, 1.2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 20; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.7f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 1.5f;
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
            }
            projectile.Damage();
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			if (projectile.timeLeft > 815 || projectile.timeLeft < 85)
				return;

			target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }
    }
}
