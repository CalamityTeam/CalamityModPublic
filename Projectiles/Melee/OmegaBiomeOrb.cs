using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class OmegaBiomeOrb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Orb");
		}

		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.friendly = true;
			projectile.alpha = 255;
			projectile.penetrate = 1;
			projectile.timeLeft = 240;
			projectile.melee = true;
		}

		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			bool jungle = player.ZoneJungle;
			bool snow = player.ZoneSnow;
			bool beach = player.ZoneBeach;
			bool corrupt = player.ZoneCorrupt;
			bool crimson = player.ZoneCrimson;
			bool dungeon = player.ZoneDungeon;
			bool desert = player.ZoneDesert;
			bool glow = player.ZoneGlowshroom;
			bool hell = player.ZoneUnderworldHeight;
			bool sky = player.ZoneSkyHeight;
			bool holy = player.ZoneHoly;
			bool nebula = player.ZoneTowerNebula;
			bool stardust = player.ZoneTowerStardust;
			bool solar = player.ZoneTowerSolar;
			bool vortex = player.ZoneTowerVortex;
			int dustType = 0;
			if (jungle)
			{
				dustType = 39;
			}
			else if (snow)
			{
				dustType = 51;
			}
			else if (beach)
			{
				dustType = 33;
			}
			else if (corrupt)
			{
				dustType = 14;
			}
			else if (crimson)
			{
				dustType = 5;
			}
			else if (dungeon)
			{
				dustType = 29;
			}
			else if (desert)
			{
				dustType = 32;
			}
			else if (glow)
			{
				dustType = 56;
			}
			else if (hell)
			{
				dustType = 6;
			}
			else if (sky)
			{
				dustType = 213;
			}
			else if (holy)
			{
				dustType = 57;
			}
			else if (nebula)
			{
				dustType = 242;
			}
			else if (stardust)
			{
				dustType = 206;
			}
			else if (solar)
			{
				dustType = 244;
			}
			else if (vortex)
			{
				dustType = 107;
			}
			else
			{
				dustType = 3;
			}
			for (int num457 = 0; num457 < 5; num457++)
			{
				int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 1.2f);
				Main.dust[num458].noGravity = true;
				Main.dust[num458].velocity *= 0.5f;
				Main.dust[num458].velocity += projectile.velocity * 0.1f;
			}
			float num472 = projectile.Center.X;
			float num473 = projectile.Center.Y;
			float num474 = 400f;
			bool flag17 = false;
			for (int num475 = 0; num475 < 200; num475++)
			{
				if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
				{
					float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
					float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
					float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
					if (num478 < num474)
					{
						num474 = num478;
						num472 = num476;
						num473 = num477;
						flag17 = true;
					}
				}
			}
			if (flag17)
			{
				float num483 = 15f;
				Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num484 = num472 - vector35.X;
				float num485 = num473 - vector35.Y;
				float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
				num486 = num483 / num486;
				num484 *= num486;
				num485 *= num486;
				projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
				projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Player player = Main.player[projectile.owner];
			bool jungle = player.ZoneJungle;
			bool snow = player.ZoneSnow;
			bool beach = player.ZoneBeach;
			bool dungeon = player.ZoneDungeon;
			bool desert = player.ZoneDesert;
			bool glow = player.ZoneGlowshroom;
			bool hell = player.ZoneUnderworldHeight;
			bool holy = player.ZoneHoly;
			bool bloodMoon = Main.bloodMoon;
			bool snowMoon = Main.snowMoon;
			bool pumpkinMoon = Main.pumpkinMoon;
			if (bloodMoon)
			{
				player.AddBuff(BuffID.Battle, 600);
			}
			if (snowMoon)
			{
				player.AddBuff(BuffID.RapidHealing, 600);
			}
			if (pumpkinMoon)
			{
				player.AddBuff(BuffID.WellFed, 600);
			}
			if (jungle)
			{
				target.AddBuff(mod.BuffType("Plague"), 600);
			}
			else if (snow)
			{
				target.AddBuff(mod.BuffType("GlacialState"), 600);
			}
			else if (beach)
			{
				target.AddBuff(mod.BuffType("CrushDepth"), 600);
			}
			else if (dungeon)
			{
				target.AddBuff(BuffID.Frostburn, 600);
			}
			else if (desert)
			{
				target.AddBuff(mod.BuffType("HolyLight"), 600);
			}
			else if (glow)
			{
				target.AddBuff(mod.BuffType("TemporalSadness"), 600);
			}
			else if (hell)
			{
				target.AddBuff(mod.BuffType("BrimstoneFlames"), 600);
			}
			else if (holy)
			{
				target.AddBuff(mod.BuffType("HolyLight"), 600);
			}
			else
			{
				target.AddBuff(mod.BuffType("ArmorCrunch"), 600);
			}
		}
	}
}
