using CalamityMod.Tiles.Abyss;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
	public class RubberMortarRoundProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Ammo/RubberMortarRound";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Round");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 300;
            projectile.ranged = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
			if (projectile.velocity.Length() >= 8f)
			{
				for (int d = 0; d < 2; d++)
				{
					float xOffset = 0f;
					float yOffset = 0f;
					if (d == 1)
					{
						xOffset = projectile.velocity.X * 0.5f;
						yOffset = projectile.velocity.Y * 0.5f;
					}
					int fire = Dust.NewDust(new Vector2(projectile.position.X + 3f + xOffset, projectile.position.Y + 3f + yOffset) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, DustID.Fire, 0f, 0f, 100, default, 1f);
					Main.dust[fire].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
					Main.dust[fire].velocity *= 0.2f;
					Main.dust[fire].noGravity = true;
					int smoke = Dust.NewDust(new Vector2(projectile.position.X + 3f + xOffset, projectile.position.Y + 3f + yOffset) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, DustID.Smoke, 0f, 0f, 100, default, 0.5f);
					Main.dust[smoke].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
					Main.dust[smoke].velocity *= 0.05f;
				}
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			int penetrateAmt = projectile.penetrate;
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 200);
            projectile.maxPenetrate = projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, projectile.Center);
			projectile.penetrate = penetrateAmt;

			SpawnDust();
			CalamityUtils.ExplosionGores(projectile.Center, 3);

			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 14);

            if (projectile.owner == Main.myPlayer)
            {
				DestroyTiles();
			}

            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
                projectile.active = false;
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                projectile.velocity *= 1.25f;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 200);
            projectile.maxPenetrate = projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
			projectile.knockBack *= 5f;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, projectile.Center);

			SpawnDust();
			CalamityUtils.ExplosionGores(projectile.Center, 3);

			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 14);

            if (projectile.owner == Main.myPlayer)
            {
				DestroyTiles();
			}
		}

		private void SpawnDust()
		{
            for (int d = 0; d < 40; d++)
            {
                int smoke = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Main.dust[smoke].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[smoke].scale = 0.5f;
                    Main.dust[smoke].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 70; d++)
            {
                int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 3f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 5f;
                fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 2f);
                Main.dust[fire].velocity *= 2f;
            }
		}

		private void DestroyTiles()
		{
			CalamityUtils.ExplodeandDestroyTiles(projectile, 5, false, new List<int>()
			{
				ModContent.TileType<AbyssGravel>(),
				ModContent.TileType<Voidstone>()
			},
			new List<int>()
			{
				ModContent.WallType<AbyssGravelWall>(),
				ModContent.WallType<VoidstoneWallUnsafe>()
			});
		}
    }
}
