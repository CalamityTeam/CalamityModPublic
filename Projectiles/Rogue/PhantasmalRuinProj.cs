using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class PhantasmalRuinProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/PhantasmalRuin";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantasmal Ruin");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 175, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, default(Color), 0.85f);
			if (projectile.timeLeft % 18 == 0)
			{
				if (projectile.owner == Main.myPlayer)
				{
					if (projectile.Calamity().stealthStrike)
					{
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, ModContent.ProjectileType<PhantasmalRuinGhost>(), (int)(projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
					}
					else
					{
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0f, Main.rand.NextFloat(-2,2), ModContent.ProjectileType<LostSoulFriendly>(), (int)(projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
					}
				}
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			float spread = 45f * 0.0174f;
			double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
			double deltaAngle = spread / 8f;
			double offsetAngle;
			int i;
			if (projectile.owner == Main.myPlayer)
			{
				if (Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<PhantasmalSoul>()] < 8)
				{
					for (i = 0; i < 8; i++)
					{
						float ai1 = Main.rand.NextFloat() + 0.5f;
						float randomSpeed = (float)Main.rand.Next(1, 7);
						float randomSpeed2 = (float)Main.rand.Next(1, 7);
						offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
						int num23 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f) + randomSpeed, ModContent.ProjectileType<PhantasmalSoul>(), (int)((double)projectile.damage * 0.05), 0f, projectile.owner, 1f, ai1);
						int num24 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f) + randomSpeed2, ModContent.ProjectileType<PhantasmalSoul>(), (int)((double)projectile.damage * 0.05), 0f, projectile.owner, 1f, ai1);
					}
				}
				else
				{
					damage = (int)(damage * 0.9);
				}
			}
		}

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			float spread = 45f * 0.0174f;
			double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
			double deltaAngle = spread / 8f;
			double offsetAngle;
			int i;
			if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<PhantasmalSoul>()] < 8)
			{
				for (i = 0; i < 8; i++)
				{
					float ai1 = Main.rand.NextFloat() + 0.5f;
					float randomSpeed = (float)Main.rand.Next(1, 7);
					float randomSpeed2 = (float)Main.rand.Next(1, 7);
					offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
					int num23 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f) + randomSpeed, ModContent.ProjectileType<PhantasmalSoul>(), (int)((double)projectile.damage * 0.05), 0f, projectile.owner, 1f, ai1);
					int num24 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f) + randomSpeed2, ModContent.ProjectileType<PhantasmalSoul>(), (int)((double)projectile.damage * 0.05), 0f, projectile.owner, 1f, ai1);
				}
			}
			else
			{
				damage = (int)(damage * 0.9);
			}
		}
    }
}
