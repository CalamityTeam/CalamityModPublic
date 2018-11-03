using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class Galaxia : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Orb");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 50;
            projectile.penetrate = 3;
            projectile.timeLeft = 240;
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 3;
        }

        public override void AI()
        {
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.Next(5) == 0)
                {
                    Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 9);
                }
            }
            projectile.alpha -= 15;
            int num58 = 150;
            if (projectile.Center.Y >= projectile.ai[1])
            {
                num58 = 0;
            }
            if (projectile.alpha < num58)
            {
                projectile.alpha = num58;
            }
            projectile.localAI[0] += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
            if (Main.rand.Next(8) == 0)
            {
                Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy((double)projectile.velocity.ToRotation(), default(Vector2));
                int num59 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.2f);
                Main.dust[num59].noGravity = true;
                Main.dust[num59].velocity = value3 * 0.66f;
                Main.dust[num59].position = projectile.Center + value3 * 12f;
            }
            if (Main.rand.Next(24) == 0)
            {
                int num60 = Gore.NewGore(projectile.Center, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), 16, 1f);
                Main.gore[num60].velocity *= 0.66f;
                Main.gore[num60].velocity += projectile.velocity * 0.3f;
            }
            if (projectile.ai[1] == 1f)
            {
                projectile.light = 0.9f;
                if (Main.rand.Next(5) == 0)
                {
                    int num59 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.2f);
                    Main.dust[num59].noGravity = true;
                }
                if (Main.rand.Next(10) == 0)
                {
                    Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
                    return;
                }
            }
            float num472 = projectile.Center.X;
			float num473 = projectile.Center.Y;
			float num474 = 1600f;
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
				float num483 = 35f;
				Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num484 = num472 - vector35.X;
				float num485 = num473 - vector35.Y;
				float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
				num486 = num483 / num486;
				num484 *= num486;
				num485 *= num486;
				projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
				projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
				return;
			}
			return;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, projectile.alpha);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
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
        	bool holy = player.ZoneHoly;
        	bool nebula = player.ZoneTowerNebula;
        	bool stardust = player.ZoneTowerStardust;
        	bool solar = player.ZoneTowerSolar;
        	bool vortex = player.ZoneTowerVortex;
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
                target.AddBuff(mod.BuffType("Plague"), 1200);
                player.AddBuff(BuffID.Thorns, 600);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 206, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
            else if (snow)
            {
                target.AddBuff(mod.BuffType("GlacialState"), 1200);
                player.AddBuff(BuffID.Warmth, 600);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 118, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
            else if (beach)
            {
                target.AddBuff(mod.BuffType("CrushDepth"), 1200);
                player.AddBuff(BuffID.Wet, 600);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 405, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
            else if (corrupt)
            {
                player.AddBuff(BuffID.Wrath, 600);
                int ball = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 95, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                Main.projectile[ball].penetrate = 1;
            }
            else if (crimson)
            {
                player.AddBuff(BuffID.Rage, 600);
                int ball = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 280, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                Main.projectile[ball].penetrate = 1;
            }
            else if (dungeon)
            {
                target.AddBuff(BuffID.Frostburn, 1200);
                player.AddBuff(BuffID.Dangersense, 600);
                int ball = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 27, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                Main.projectile[ball].penetrate = 1;
            }
            else if (desert)
            {
                target.AddBuff(mod.BuffType("HolyLight"), 1200);
                player.AddBuff(BuffID.Endurance, 600);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 661, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
            else if (glow)
            {
                target.AddBuff(mod.BuffType("TemporalSadness"), 1200);
                player.AddBuff(BuffID.Spelunker, 600);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 131, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
            else if (hell)
            {
                target.AddBuff(mod.BuffType("BrimstoneFlames"), 1200);
                player.AddBuff(BuffID.Inferno, 600);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 15, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
            else if (holy)
            {
                target.AddBuff(mod.BuffType("HolyLight"), 1200);
                player.AddBuff(BuffID.Heartreach, 600);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 644, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
            else if (nebula)
            {
                player.AddBuff(BuffID.MagicPower, 600);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 634, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
            else if (stardust)
            {
                player.AddBuff(BuffID.Summoning, 600);
                int ball = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 614, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                Main.projectile[ball].penetrate = 1;
            }
            else if (solar)
            {
                player.AddBuff(BuffID.Titan, 600);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 612, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
            else if (vortex)
            {
                player.AddBuff(BuffID.AmmoReservation, 600);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 616, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
            else
            {
                target.AddBuff(mod.BuffType("ArmorCrunch"), 1200);
                player.AddBuff(BuffID.DryadsWard, 600);
                int ball = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, 604, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                Main.projectile[ball].penetrate = 1;
            }
        }
    }
}