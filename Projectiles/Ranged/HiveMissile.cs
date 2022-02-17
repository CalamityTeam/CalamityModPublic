using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class HiveMissile : ModProjectile
    {
        public static Item FalseLauncher = null;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bomb");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.scale = 0.75f;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 95;
            projectile.ranged = true;
        }

        private static void DefineFalseLauncher()
        {
            int rocketID = ItemID.RocketLauncher;
            FalseLauncher = new Item();
            FalseLauncher.SetDefaults(rocketID, true);
        }

        public override void AI()
        {
			if (Math.Abs(projectile.velocity.X) >= 8f || Math.Abs(projectile.velocity.Y) >= 8f)
			{
				for (int num246 = 0; num246 < 2; num246++)
				{
					float num247 = 0f;
					float num248 = 0f;
					if (num246 == 1)
					{
						num247 = projectile.velocity.X * 0.5f;
						num248 = projectile.velocity.Y * 0.5f;
					}
					int num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 6, 0f, 0f, 100, default, 1f);
					Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
					Main.dust[num249].velocity *= 0.2f;
					Main.dust[num249].noGravity = true;
					num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 31, 0f, 0f, 100, default, 0.5f);
					Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
					Main.dust[num249].velocity *= 0.05f;
				}
			}
			if (Math.Abs(projectile.velocity.X) < 15f && Math.Abs(projectile.velocity.Y) < 15f)
			{
				projectile.velocity *= 1.5f;
			}
			else if (Main.rand.NextBool(2))
			{
				int num252 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1f);
				Main.dust[num252].scale = 0.1f + (float)Main.rand.Next(5) * 0.1f;
				Main.dust[num252].fadeIn = 1.5f + (float)Main.rand.Next(5) * 0.1f;
				Main.dust[num252].noGravity = true;
				Main.dust[num252].position = projectile.Center + new Vector2(0f, (float)(-(float)projectile.height / 2)).RotatedBy((double)projectile.rotation, default) * 1.1f;
				Main.rand.Next(2);
				num252 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1f);
				Main.dust[num252].scale = 1f + (float)Main.rand.Next(5) * 0.1f;
				Main.dust[num252].noGravity = true;
				Main.dust[num252].position = projectile.Center + new Vector2(0f, (float)(-(float)projectile.height / 2 - 6)).RotatedBy((double)projectile.rotation, default) * 1.1f;
			}
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 80);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            if (projectile.owner == Main.myPlayer)
            {
                int num516 = 3;
                for (int num517 = 0; num517 < num516; num517++)
                {
                    if (num517 % 2 != 1 || Main.rand.NextBool(3))
                    {
                        Vector2 value20 = projectile.position;
                        Vector2 value21 = projectile.oldVelocity;
                        value21.Normalize();
                        value21 *= 8f;
                        float num518 = (float)Main.rand.Next(-35, 36) * 0.01f;
                        float num519 = (float)Main.rand.Next(-35, 36) * 0.01f;
                        value20 -= value21 * (float)num517;
                        num518 += projectile.oldVelocity.X / 6f;
                        num519 += projectile.oldVelocity.Y / 6f;
                        int bee = Projectile.NewProjectile(value20.X, value20.Y, num518, num519, Main.player[projectile.owner].beeType(), Main.player[projectile.owner].beeDamage(projectile.damage / 2), Main.player[projectile.owner].beeKB(0f), Main.myPlayer);
						if (bee.WithinBounds(Main.maxProjectiles))
						{
							Main.projectile[bee].penetrate = 2;
							Main.projectile[bee].Calamity().forceRanged = true;
						}
                    }
                }
            }
            Main.PlaySound(SoundID.Item14, projectile.position);
            for (int num621 = 0; num621 < 20; num621++)
            {
                int num622 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 90, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 30; num623++)
            {
                int num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 90, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 90, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
			CalamityUtils.ExplosionGores(projectile.Center, 3);

			// Construct a fake item to use with vanilla code for the sake of picking ammo.
			if (FalseLauncher is null)
				DefineFalseLauncher();
			Player player = Main.player[projectile.owner];
			int projID = ProjectileID.RocketI;
			float shootSpeed = 0f;
			bool canShoot = true;
			int damage = 0;
			float kb = 0f;
			player.PickAmmo(FalseLauncher, ref projID, ref shootSpeed, ref canShoot, ref damage, ref kb, true);
			int blastRadius = 0;
			if (projID == ProjectileID.RocketII)
				blastRadius = 5;
			else if (projID == ProjectileID.RocketIV)
				blastRadius = 8;

			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 14);

			if (projectile.owner == Main.myPlayer && blastRadius > 0)
			{
				CalamityUtils.ExplodeandDestroyTiles(projectile, blastRadius, true, new List<int>() { }, new List<int>() { });
			}
        }
    }
}
