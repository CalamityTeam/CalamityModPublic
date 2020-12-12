using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Ranged;
namespace CalamityMod.Projectiles.Rogue
{
    public class Prismalline3 : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Prismalline";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismalline");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
			CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 300f, 25f, 20f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 154, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
			if (projectile.ai[0] == 1f)
			{
				int shardCount = Main.rand.Next(1,4);
				for (int s = 0; s < shardCount; s++)
				{
					Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
					int shard = Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<AquashardSplit>(), projectile.damage / 3, 0f, projectile.owner);
					if (shard.WithinBounds(Main.maxProjectiles))
					{
						Main.projectile[shard].Calamity().forceRogue = true;
						Main.projectile[shard].penetrate = 1;
					}
				}
			}
        }
    }
}
