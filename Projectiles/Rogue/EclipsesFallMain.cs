using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class EclipsesFallMain : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eclipse's Fall");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 138, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			SpawnSpears(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			SpawnSpears(target.Center);
        }

		private void SpawnSpears(Vector2 targetPos)
		{
            for (int n = 0; n < Main.rand.Next(3, 6); n++) //3 to 5 spears
            {
                float x = targetPos.X + Main.rand.Next(-400, 401);
                float y = targetPos.Y - Main.rand.Next(500, 801);
                Vector2 source = new Vector2(x, y);
				Vector2 velocity = targetPos - source;
                velocity.X += Main.rand.Next(-100, 101);
                float speed = 29f;
                float targetDist = velocity.Length();
                targetDist = speed / targetDist;
                velocity.X *= targetDist;
                velocity.Y *= targetDist;
                Projectile.NewProjectile(source, velocity, ModContent.ProjectileType<EclipsesSmol>(), (int)(projectile.damage * 0.08f * Main.rand.NextFloat(4f, 7f)), (int)(projectile.knockBack * 0.1f * Main.rand.NextFloat(7f, 10f)), projectile.owner, 0f, 0f);
            }
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
