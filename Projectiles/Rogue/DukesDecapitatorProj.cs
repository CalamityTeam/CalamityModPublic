using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Rogue
{
    public class DukesDecapitatorProj : ModProjectile
    {
		bool stealthBubbles = false;
		float rotationAmount = 1.5f;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Decapitator");
		}

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 64;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 20;
			projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
		}

		public override void AI()
        {
            CalamityPlayer modPlayer = Main.player[Main.myPlayer].Calamity();
			if(projectile.velocity.X != 0 || projectile.velocity.Y != 0){
				projectile.velocity.X *= 0.99f;
				projectile.velocity.Y *= 0.99f;
			}
			projectile.ai[0] += 1f;
			if(projectile.ai[0] == 1f && modPlayer.StealthStrikeAvailable())
			{
				stealthBubbles = true;
                projectile.Calamity().stealthStrike = true;
			}
			if (projectile.ai[0] == 5f)
				projectile.tileCollide = true;

        	if ((projectile.ai[0] % 15f) == 0f && rotationAmount > 0)
        	{
				rotationAmount -= 0.05f;
				if(stealthBubbles == true && projectile.owner == Main.myPlayer)
        		{
					float velocityX = Main.rand.NextFloat(-0.8f, 0.8f);
					float velocityY = Main.rand.NextFloat(-0.8f, -0.8f);
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, velocityX, velocityY, ModContent.ProjectileType<DukesDecapitatorBubble>(), (int)((double)projectile.damage * 0.8), projectile.knockBack, projectile.owner, 0f, 0f);
				}
			}
			if(rotationAmount <= 0f)
				projectile.Kill();

			projectile.rotation += rotationAmount;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			projectile.velocity.X = 0f;
			projectile.velocity.Y = 0f;
			rotationAmount -= 0.05f;
		}

		public override void Kill(int timeLeft)
        {
			for (int i = 0; i < 20; i++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 49, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, new Color(255, 255, 255), 0.75f);
			}
        }
    }
}
