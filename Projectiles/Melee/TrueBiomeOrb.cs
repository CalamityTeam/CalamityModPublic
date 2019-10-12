using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class TrueBiomeOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
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
            int dustType;
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
            else
            {
                dustType = 3;
            }
            for (int num457 = 0; num457 < 10; num457++)
            {
                int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 1.2f);
                Main.dust[num458].noGravity = true;
                Main.dust[num458].velocity *= 0.5f;
                Main.dust[num458].velocity += projectile.velocity * 0.1f;
            }
            return;
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
            if (jungle)
            {
                target.AddBuff(mod.BuffType("Plague"), 360);
            }
            else if (snow)
            {
                target.AddBuff(mod.BuffType("GlacialState"), 360);
            }
            else if (beach)
            {
                target.AddBuff(mod.BuffType("CrushDepth"), 360);
            }
            else if (dungeon)
            {
                target.AddBuff(BuffID.Frostburn, 360);
            }
            else if (desert)
            {
                target.AddBuff(mod.BuffType("HolyLight"), 360);
            }
            else if (glow)
            {
                target.AddBuff(mod.BuffType("TemporalSadness"), 360);
            }
            else if (hell)
            {
                target.AddBuff(mod.BuffType("BrimstoneFlames"), 360);
            }
            else if (holy)
            {
                target.AddBuff(mod.BuffType("HolyLight"), 360);
            }
            else
            {
                target.AddBuff(mod.BuffType("ArmorCrunch"), 360);
            }
        }
    }
}
