using CalamityMod.Items.Materials;
using CalamityMod.Buffs.Potions;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ArkoftheCosmos : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ark of the Cosmos");
            Tooltip.SetDefault("Fires different homing projectiles based on what biome you're in\n" +
                "Upon hitting an enemy you are granted a buff based on what biome you're in\n" +
                "Projectiles also change based on moon events");
        }

        public override void SetDefaults()
        {
            item.width = 102;
            item.damage = 501;
            item.melee = true;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 9.5f;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.height = 102;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<EonBeam>();
            item.shootSpeed = 28f;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 15;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                    type = ModContent.ProjectileType<EonBeam>();
                    break;
                case 1:
                    type = ModContent.ProjectileType<EonBeamV2>();
                    break;
                case 2:
                    type = ModContent.ProjectileType<EonBeamV3>();
                    break;
                case 3:
                    type = ModContent.ProjectileType<EonBeamV4>();
                    break;
            }
            int projectile = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, Main.myPlayer);
            Main.projectile[projectile].timeLeft = 160;
            Main.projectile[projectile].tileCollide = false;
            float num72 = Main.rand.Next(22, 30);
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X + vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y + vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight + (float)Main.mouseY + vector2.Y;
            }
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num79 = 0f;
                num80 = num72;
            }
            else
            {
                num80 = num72 / num80;
            }

            int num107 = 4;
            for (int num108 = 0; num108 < num107; num108++)
            {
                vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)-(float)player.direction + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y);
                vector2.X = (vector2.X + player.Center.X) / 2f;
                vector2.Y -= (float)(100 * num108);
                num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
                num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
                num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
                num80 = num72 / num80;
                num78 *= num80;
                num79 *= num80;
                float speedX4 = num78 + (float)Main.rand.Next(-360, 361) * 0.02f;
                float speedY5 = num79 + (float)Main.rand.Next(-360, 361) * 0.02f;
                int projectileFire = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, ModContent.ProjectileType<Galaxia2>(), damage, knockBack, player.whoAmI, 0f, (float)Main.rand.Next(3));
                Main.projectile[projectileFire].timeLeft = 80;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FourSeasonsGalaxia>());
            recipe.AddIngredient(ModContent.ItemType<ArkoftheElements>());
			recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int num250 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 66, (float)(player.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
                Main.dust[num250].velocity *= 0.2f;
                Main.dust[num250].noGravity = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
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
                player.AddBuff(ModContent.BuffType<GravityNormalizerBuff>(), 600);
            }
            else if (jungle)
            {
                player.AddBuff(BuffID.Thorns, 600);
            }
            else if (snow)
            {
                player.AddBuff(BuffID.Warmth, 600);
            }
            else if (beach)
            {
                player.AddBuff(BuffID.Wet, 600);
            }
            else if (corrupt)
            {
                player.AddBuff(BuffID.Wrath, 600);
            }
            else if (crimson)
            {
                player.AddBuff(BuffID.Rage, 600);
            }
            else if (dungeon)
            {
                player.AddBuff(BuffID.Dangersense, 600);
            }
            else if (desert)
            {
                player.AddBuff(BuffID.Endurance, 600);
            }
            else if (glow)
            {
                player.AddBuff(BuffID.Spelunker, 600);
            }
            else if (hell)
            {
                player.AddBuff(BuffID.Inferno, 600);
            }
            else if (holy)
            {
                player.AddBuff(BuffID.Heartreach, 600);
            }
            else if (nebula)
            {
                player.AddBuff(BuffID.MagicPower, 600);
            }
            else if (stardust)
            {
                player.AddBuff(BuffID.Summoning, 600);
            }
            else if (solar)
            {
                player.AddBuff(BuffID.Titan, 600);
            }
            else if (vortex)
            {
                player.AddBuff(BuffID.AmmoReservation, 600);
            }
            else
            {
                player.AddBuff(BuffID.DryadsWard, 600);
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
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
                player.AddBuff(BuffID.Thorns, 600);
            }
            else if (snow)
            {
                player.AddBuff(BuffID.Warmth, 600);
            }
            else if (beach)
            {
                player.AddBuff(BuffID.Wet, 600);
            }
            else if (corrupt)
            {
                player.AddBuff(BuffID.Wrath, 600);
            }
            else if (crimson)
            {
                player.AddBuff(BuffID.Rage, 600);
            }
            else if (dungeon)
            {
                player.AddBuff(BuffID.Dangersense, 600);
            }
            else if (desert)
            {
                player.AddBuff(BuffID.Endurance, 600);
            }
            else if (glow)
            {
                player.AddBuff(BuffID.Spelunker, 600);
            }
            else if (hell)
            {
                player.AddBuff(BuffID.Inferno, 600);
            }
            else if (holy)
            {
                player.AddBuff(BuffID.Heartreach, 600);
            }
            else if (nebula)
            {
                player.AddBuff(BuffID.MagicPower, 600);
            }
            else if (stardust)
            {
                player.AddBuff(BuffID.Summoning, 600);
            }
            else if (solar)
            {
                player.AddBuff(BuffID.Titan, 600);
            }
            else if (vortex)
            {
                player.AddBuff(BuffID.AmmoReservation, 600);
            }
            else
            {
                player.AddBuff(BuffID.DryadsWard, 600);
            }
        }
    }
}
