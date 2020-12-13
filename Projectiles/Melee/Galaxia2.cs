using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Potions;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class Galaxia2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/Galaxia";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 50;
            projectile.penetrate = 2;
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 1;
        }

        public override void AI()
        {
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    Main.PlaySound(SoundID.Item9, projectile.position);
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
            if (Main.rand.NextBool(8))
            {
                Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy((double)projectile.velocity.ToRotation(), default);
                int num59 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.2f);
                Main.dust[num59].velocity = value3 * 0.66f;
                Main.dust[num59].noGravity = true;
                Main.dust[num59].position = projectile.Center + value3 * 12f;
            }
            if (Main.rand.NextBool(24))
            {
                int num60 = Gore.NewGore(projectile.Center, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), 16, 1f);
                Main.gore[num60].velocity *= 0.66f;
                Main.gore[num60].velocity += projectile.velocity * 0.3f;
            }
            if (projectile.ai[1] == 1f)
            {
                projectile.light = 0.9f;
                if (Main.rand.NextBool(5))
                {
                    int num59 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.2f);
                    Main.dust[num59].noGravity = true;
                }
                if (Main.rand.NextBool(10))
                {
                    Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
                }
            }

			CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 1600f, 35f, 20f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.Calamity();
			bool astral = modPlayer.ZoneAstral;
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
            if (astral)
			{
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 1200);
                player.AddBuff(ModContent.BuffType<GravityNormalizerBuff>(), 600);
                int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<AstralStar>(), projectile.damage, projectile.knockBack, projectile.owner);
				if (proj.WithinBounds(Main.maxProjectiles))
					Main.projectile[proj].Calamity().forceMelee = true;
			}
			else if (jungle)
            {
                target.AddBuff(ModContent.BuffType<Plague>(), 1200);
                player.AddBuff(BuffID.Thorns, 600);
                int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.Leaf, projectile.damage, projectile.knockBack, projectile.owner);
				if (proj.WithinBounds(Main.maxProjectiles))
					Main.projectile[proj].Calamity().forceMelee = true;
            }
            else if (snow)
            {
                target.AddBuff(ModContent.BuffType<GlacialState>(), 1200);
                player.AddBuff(BuffID.Warmth, 600);
                Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.IceBolt, projectile.damage, projectile.knockBack, projectile.owner);
            }
            else if (beach)
            {
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 1200);
                player.AddBuff(BuffID.Wet, 600);
                int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, ProjectileID.FlaironBubble, projectile.damage, projectile.knockBack, projectile.owner);
				if (proj.WithinBounds(Main.maxProjectiles))
					Main.projectile[proj].Calamity().forceMelee = true;
            }
            else if (corrupt)
            {
                player.AddBuff(BuffID.Wrath, 600);
                int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.CursedFlameFriendly, projectile.damage, projectile.knockBack, projectile.owner);
				if (proj.WithinBounds(Main.maxProjectiles))
				{
					Main.projectile[proj].Calamity().forceMelee = true;
					Main.projectile[proj].penetrate = 1;
				}
            }
            else if (crimson)
            {
                player.AddBuff(BuffID.Rage, 600);
                int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.GoldenShowerFriendly, projectile.damage, projectile.knockBack, projectile.owner);
				if (proj.WithinBounds(Main.maxProjectiles))
				{
					Main.projectile[proj].Calamity().forceMelee = true;
					Main.projectile[proj].penetrate = 1;
				}
            }
            else if (dungeon)
            {
                target.AddBuff(BuffID.Frostburn, 1200);
                player.AddBuff(BuffID.Dangersense, 600);
                int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.WaterBolt, projectile.damage, projectile.knockBack, projectile.owner);
				if (proj.WithinBounds(Main.maxProjectiles))
				{
					Main.projectile[proj].Calamity().forceMelee = true;
					Main.projectile[proj].penetrate = 1;
				}
            }
            else if (desert)
            {
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 1200);
                player.AddBuff(BuffID.Endurance, 600);
                int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.BlackBolt, projectile.damage, projectile.knockBack, projectile.owner);
				if (proj.WithinBounds(Main.maxProjectiles))
					Main.projectile[proj].Calamity().forceMelee = true;
            }
            else if (glow)
            {
                target.AddBuff(ModContent.BuffType<TemporalSadness>(), 1200);
                player.AddBuff(BuffID.Spelunker, 600);
                int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.Mushroom, projectile.damage, projectile.knockBack, projectile.owner);
				if (proj.WithinBounds(Main.maxProjectiles))
					Main.projectile[proj].Calamity().forceMelee = true;
            }
            else if (hell)
            {
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 1200);
                player.AddBuff(BuffID.Inferno, 600);
                int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.BallofFire, projectile.damage, projectile.knockBack, projectile.owner);
				if (proj.WithinBounds(Main.maxProjectiles))
					Main.projectile[proj].Calamity().forceMelee = true;
            }
            else if (holy)
            {
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 1200);
                player.AddBuff(BuffID.Heartreach, 600);
                int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.RainbowCrystalExplosion, projectile.damage, projectile.knockBack, projectile.owner);
				if (proj.WithinBounds(Main.maxProjectiles))
				{
					Main.projectile[proj].Calamity().forceMelee = true;
					Main.projectile[proj].usesLocalNPCImmunity = true;
					Main.projectile[proj].localNPCHitCooldown = -1;
				}
            }
            else if (nebula)
            {
                player.AddBuff(BuffID.MagicPower, 600);
                int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.NebulaBlaze1, projectile.damage, projectile.knockBack, projectile.owner);
				if (proj.WithinBounds(Main.maxProjectiles))
					Main.projectile[proj].Calamity().forceMelee = true;
            }
            else if (stardust)
            {
                player.AddBuff(BuffID.Summoning, 600);
                int ball = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.StardustCellMinionShot, projectile.damage, projectile.knockBack, projectile.owner);
				if (ball.WithinBounds(Main.maxProjectiles))
				{
					Main.projectile[ball].Calamity().forceMelee = true;
					Main.projectile[ball].penetrate = 1;
				}
            }
            else if (solar)
            {
                player.AddBuff(BuffID.Titan, 600);
                Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.SolarWhipSwordExplosion, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            }
            else if (vortex)
            {
                player.AddBuff(BuffID.AmmoReservation, 600);
                int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.VortexBeaterRocket, projectile.damage, projectile.knockBack, projectile.owner);
				if (proj.WithinBounds(Main.maxProjectiles))
				{
					Main.projectile[proj].Calamity().forceMelee = true;
					Main.projectile[proj].usesLocalNPCImmunity = true;
					Main.projectile[proj].localNPCHitCooldown = -1;
				}
            }
            else
            {
                target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 1200);
                player.AddBuff(BuffID.DryadsWard, 600);
                int ball = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.TerrarianBeam, projectile.damage, projectile.knockBack, projectile.owner);
				if (ball.WithinBounds(Main.maxProjectiles))
					Main.projectile[ball].penetrate = 1;
            }
        }
    }
}
