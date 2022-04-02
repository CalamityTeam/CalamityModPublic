using CalamityMod.Buffs.Pets;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
	public class MiniHiveMind : ModProjectile
	{
		private int reelBackCooldown = 0;
		private int charging = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mini Hive Mind");
			Main.projFrames[projectile.type] = 16;
			Main.projPet[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.netImportant = true;
			projectile.width = 38;
			projectile.height = 44;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.timeLeft *= 5;
			projectile.tileCollide = false;
		}

		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.Calamity();

			if (!player.active)
			{
				projectile.active = false;
				return;
			}

			//Delete the projectile if the player doesnt have the buff
			if (!player.HasBuff(ModContent.BuffType<HiveMindPet>()))
			{
				projectile.Kill();
			}

			if (player.dead)
			{
				modPlayer.hiveMindPet = false;
			}
			if (modPlayer.hiveMindPet)
			{
				projectile.timeLeft = 2;
			}

			if (charging <= 0)
				projectile.FloatingPetAI(true, 0.05f);

			Vector2 playerVec = player.Center - projectile.Center;
			float playerDist = playerVec.Length();
			if (reelBackCooldown > 0)
				reelBackCooldown--;
			if (charging > 0)
				charging--;
			if (reelBackCooldown <= 0 && Main.rand.NextBool(500) && playerDist < 100f && charging <= 0)
			{
				if (Main.myPlayer == projectile.owner)
				{
					//Do a backwards charge away from the player
					reelBackCooldown = 300;
					playerVec.Normalize();
					projectile.velocity = playerVec * 8f;
					charging = 50;
					Main.PlaySound(SoundID.ForceRoar, (int)projectile.Center.X, (int)projectile.Center.Y, -1, 0.5f, 0f);
					projectile.netUpdate = true;
				}
			}
			if (charging < 22 && charging > 0)
				projectile.alpha += 12;
			if (charging == 1)
			{
				float xOffset = Main.rand.NextFloat(400f, 600f) * (Main.rand.NextBool() ? -1f : 1f);
				float yOffset = Main.rand.NextFloat(400f, 600f) * (Main.rand.NextBool() ? -1f : 1f);
				Vector2 teleportPos = new Vector2(player.Center.X + xOffset, player.Center.Y + yOffset);
                projectile.Center = teleportPos;
				projectile.alpha = 255;
                projectile.netUpdate = true;
			}
			if (projectile.alpha > 0 && charging <= 0)
				projectile.alpha -= 12;
			if (projectile.alpha < 0)
				projectile.alpha = 0;
			if (projectile.alpha > 255)
				projectile.alpha = 255;
			if (charging > 0)
			{
				projectile.rotation.AngleTowards(0f, 0.1f);
			}

			//Animation
			if (projectile.frameCounter++ % 6 == 0)
			{
				projectile.frame++;
			}
			if (projectile.frame >= Main.projFrames[projectile.type])
			{
				projectile.frame = 0;
			}
		}
	}
}
