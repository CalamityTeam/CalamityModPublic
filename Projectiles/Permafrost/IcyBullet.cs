using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Permafrost
{
	public class IcyBullet : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 4;
			projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
            projectile.timeLeft = 600;
			projectile.friendly = true;
            projectile.ranged = true;
            projectile.coldDamage = true;
            projectile.penetrate = 3;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
		}
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Icy Bullet");
		}

		public override void AI()
		{
            if (Main.rand.Next(3) == 0)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 88, projectile.velocity.X, projectile.velocity.Y, 0, default(Color), 1f);
                Main.dust[index2].noGravity = true;
            }
        }
		
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
            target.AddBuff(BuffID.Frostburn, 300);
            target.AddBuff(mod.BuffType("GlacialState"), 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha);
        }

        public override void Kill(int timeLeft)
		{
            Main.PlaySound(SoundID.Item27, projectile.position);
            for (int index1 = 0; index1 < 5; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 88, 0f, 0f, 0, new Color(), 0.9f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
            }
            if (projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < 2; ++index)
                {
                    float SpeedX = -projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    float SpeedY = -projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    Projectile.NewProjectile(projectile.position.X + SpeedX, projectile.position.Y + SpeedY, SpeedX, SpeedY, ProjectileID.CrystalShard, projectile.damage / 3, 0f, projectile.owner);
                }
            }
        }
	}
}