using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class StickyBol : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sticky Bol");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
			int Alpha = 175;
			Color newColor = new Color(0, 80, (int) byte.MaxValue, 100);
			if (Main.rand.NextBool(12))
			{
				Vector2 vector2 = projectile.velocity * (float) Main.rand.Next(6) / 6f;
				int num = 6;
				Dust.NewDust(projectile.position + Vector2.One * 6f, projectile.width - num * 2, projectile.height - num * 2, 4, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, Alpha, newColor, 1.2f);
			}
            //Sticky Behaviour
            projectile.StickyProjAI(10);
			if (projectile.ai[0] != 1f)
			{
				try
				{
					int num223 = (int)(projectile.position.X / 16f) - 1;
					int num224 = (int)((projectile.position.X + (float)projectile.width) / 16f) + 2;
					int num225 = (int)(projectile.position.Y / 16f) - 1;
					int num226 = (int)((projectile.position.Y + (float)projectile.height) / 16f) + 2;
					if (num223 < 0)
					{
						num223 = 0;
					}
					if (num224 > Main.maxTilesX)
					{
						num224 = Main.maxTilesX;
					}
					if (num225 < 0)
					{
						num225 = 0;
					}
					if (num226 > Main.maxTilesY)
					{
						num226 = Main.maxTilesY;
					}
					for (int num227 = num223; num227 < num224; num227++)
					{
						for (int num228 = num225; num228 < num226; num228++)
						{
							if (Main.tile[num227, num228] != null && Main.tile[num227, num228].nactive() && (Main.tileSolid[(int)Main.tile[num227, num228].type] || (Main.tileSolidTop[(int)Main.tile[num227, num228].type] && Main.tile[num227, num228].frameY == 0)))
							{
								Vector2 vector19;
								vector19.X = (float)(num227 * 16);
								vector19.Y = (float)(num228 * 16);
								if (projectile.position.X + (float)projectile.width - 4f > vector19.X && projectile.position.X + 4f < vector19.X + 16f && projectile.position.Y + (float)projectile.height - 4f > vector19.Y && projectile.position.Y + 4f < vector19.Y + 16f)
								{
									projectile.velocity.X = 0f;
									projectile.velocity.Y = -0.2f;
								}
							}
						}
					}
				} catch
				{
				}
				projectile.localAI[1] += 1f;
				if (projectile.localAI[1] > 10f)
				{
					projectile.localAI[1] = 10f;
					if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
					{
						projectile.velocity.X = projectile.velocity.X * 0.97f;
						if ((double)projectile.velocity.X > -0.01 && (double)projectile.velocity.X < 0.01)
						{
							projectile.velocity.X = 0f;
							projectile.netUpdate = true;
						}
					}
					projectile.velocity.Y = projectile.velocity.Y + 0.2f;
				}
				projectile.rotation += projectile.velocity.X * 0.1f;
			}
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.ModifyHitNPCSticky(5, true);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 120);
        }
    }
}
