using System;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Rogue
{
    public class SkyStabberProj : ModProjectile
    {
        private static int Lifetime = 1200;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SkyStabberProj");
        }

        public override void SetDefaults()
        {
            projectile.width = 15;
            projectile.height = 15;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = true;
            projectile.penetrate = 20;
            projectile.timeLeft = Lifetime;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;

            if (projectile.ai[0] >= 90f)
            {
                projectile.velocity.X *= 0.98f;
                projectile.velocity.Y *= 0.98f;
            }
			else
			{
				projectile.rotation += 0.3f * (float)projectile.direction;
			}

            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
			if (modPlayer.killSpikyBalls == true)
			{
				projectile.active = false;
				projectile.netUpdate = true;
			}
        }

        // Makes the projectile bounce infinitely, as it stops mid-air anyway.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X * 0.6f;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y * 0.6f;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (projectile.Calamity().stealthStrike == true)
			{
				for (int n = 0; n < 4; n++)
				{
					float x = target.Center.X + (float)Main.rand.Next(-400, 400);
					float y = target.Center.Y - (float)Main.rand.Next(500, 800);
					Vector2 vector = new Vector2(x, y);
					float num13 = target.Center.X + (float)(target.width / 2) - vector.X;
					float num14 = target.Center.Y + (float)(target.height / 2) - vector.Y;
					num13 += (float)Main.rand.Next(-100, 101);
					int num15 = 20;
					float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
					num16 = (float)num15 / num16;
					num13 *= num16;
					num14 *= num16;
					int num17 = Projectile.NewProjectile(x, y, num13, num14, ModContent.ProjectileType<StickyFeatherAero>(), (int)((double)projectile.damage * 0.25), 1f, projectile.owner, 0f, 0f);
					Main.projectile[num17].Calamity().forceRogue = true;
				}
			}
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			if (projectile.Calamity().stealthStrike == true)
			{
				for (int n = 0; n < 4; n++)
				{
					float x = target.Center.X + (float)Main.rand.Next(-400, 400);
					float y = target.Center.Y - (float)Main.rand.Next(500, 800);
					Vector2 vector = new Vector2(x, y);
					float num13 = target.Center.X + (float)(target.width / 2) - vector.X;
					float num14 = target.Center.Y + (float)(target.height / 2) - vector.Y;
					num13 += (float)Main.rand.Next(-100, 101);
					int num15 = 20;
					float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
					num16 = (float)num15 / num16;
					num13 *= num16;
					num14 *= num16;
					int num17 = Projectile.NewProjectile(x, y, num13, num14, ModContent.ProjectileType<StickyFeatherAero>(), (int)((double)projectile.damage * 0.25), 1f, projectile.owner, 0f, 0f);
					Main.projectile[num17].Calamity().forceRogue = true;
				}
			}
        }
    }
}
