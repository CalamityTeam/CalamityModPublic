using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class EclipsesStealth : ModProjectile
	{
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/EclipsesFall";

		private bool changedTimeLeft = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eclipse's Stealth");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 25;
			projectile.height = 25;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.timeLeft = 300;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = -1;
			projectile.Calamity().rogue = true;
		}

		public override void AI()
		{
			if (projectile.timeLeft % 5 == 0) //congrats Pinkie... every 5 ticks
			{
				if (Main.rand.NextBool(2) && Main.myPlayer == projectile.owner)
				{
					float dmgKBMult = Main.rand.NextFloat(0.4f, 0.6f);
					int spearAmt = Main.rand.Next(1, 3); //1 to 2 spears
					for (int n = 0; n < spearAmt; n++)
					{
						CalamityUtils.ProjectileRain(projectile.Center, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<EclipsesSmol>(), (int)(projectile.damage * dmgKBMult), projectile.knockBack * dmgKBMult, projectile.owner);
					}
				}
			}

			//Behavior when not sticking to anything
			if (projectile.ai[0] == 0f)
			{
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
				if (Main.rand.NextBool(8)) //dust
				{
					Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 138, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
				}
			}

			//Ensures that a spear will last 10 seconds after it hits something
			if (projectile.ai[0] == 1f && !changedTimeLeft)
			{
				projectile.timeLeft = 600;
				changedTimeLeft = true;
			}

			//Sticky Behaviour
			projectile.StickyProjAI(10);
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			projectile.ModifyHitNPCSticky(1, true);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
			{
				targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
			}
			return null;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}

		public override bool? CanHitNPC(NPC target)
		{
			if (projectile.ai[0] == 1f)
			{
				return false;
			}
			return null;
		}

		public override bool CanHitPvp(Player target) => projectile.ai[0] != 1f;
	}
}
