using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Accessories
{
    public class VoidofExtinction : ModItem
    {
        public const int FireProjectiles = 2;
        public const float FireAngleSpread = 120;
        public int FireCountdown = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void of Extinction");
            Tooltip.SetDefault("No longer cursed\n" +
                "Drops brimstone fireballs from the sky occasionally\n" +
                "15% increase to all damage\n" +
                "Brimstone fire rains down while invincibility is active\n" +
				"Temporary immunity to lava, greatly reduces lava burn damage, and 25% increased damage while in lava");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.expert = true;
			item.rare = 9;
			item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            if (modPlayer.calamityRing)
            {
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ObsidianRose);
            recipe.AddIngredient(null, "Gehenna");
            recipe.AddIngredient(null, "CalamityRing");
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.calamityRing = true;
			player.lavaRose = true;
			player.lavaMax += 240;
			player.allDamage += 0.15f;
			if (player.lavaWet)
            {
				player.allDamage += 0.25f;
			}
            if (player.immune)
            {
                if (Main.rand.Next(10) == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int l = 0; l < 1; l++)
                        {
                            float x = player.position.X + (float)Main.rand.Next(-400, 400);
                            float y = player.position.Y - (float)Main.rand.Next(500, 800);
                            Vector2 vector = new Vector2(x, y);
                            float num15 = player.position.X + (float)(player.width / 2) - vector.X;
                            float num16 = player.position.Y + (float)(player.height / 2) - vector.Y;
                            num15 += (float)Main.rand.Next(-100, 101);
                            int num17 = 22;
                            float num18 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
                            num18 = (float)num17 / num18;
                            num15 *= num18;
                            num16 *= num18;
                            int num19 = Projectile.NewProjectile(x, y, num15, num16, mod.ProjectileType("StandingFire"), 40, 5f, player.whoAmI, 0f, 0f);
                            Main.projectile[num19].ai[1] = player.position.Y;
                        }
                    }
                }
            }
            if (FireCountdown == 0)
            {
                FireCountdown = 600;
            }
            if (FireCountdown > 0)
            {
                FireCountdown--;
                if (FireCountdown == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int speed2 = 25;
                        float spawnX = Main.rand.Next(1000) - 500 + player.Center.X;
                        float spawnY = -1000 + player.Center.Y;
                        Vector2 baseSpawn = new Vector2(spawnX, spawnY);
                        Vector2 baseVelocity = player.Center - baseSpawn;
                        baseVelocity.Normalize();
                        baseVelocity = baseVelocity * speed2;
                        for (int i = 0; i < FireProjectiles; i++)
                        {
                            Vector2 spawn = baseSpawn;
                            spawn.X = spawn.X + i * 30 - (FireProjectiles * 15);
                            Vector2 velocity = baseVelocity;
                            velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-FireAngleSpread / 2 + (FireAngleSpread * i / (float)FireProjectiles)));
                            velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
                            int projectile = Projectile.NewProjectile(spawn.X, spawn.Y, velocity.X, velocity.Y, mod.ProjectileType("BrimstoneHellfireballFriendly2"), 70, 5f, Main.myPlayer, 0f, 0f);
                            Main.projectile[projectile].tileCollide = false;
                            Main.projectile[projectile].timeLeft = 50;
                        }
                    }
                }
            }
        }
    }
}
