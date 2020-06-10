using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
                "Temporary immunity to lava, greatly reduces lava burn damage, and 25% increased damage while in lava\n" +
				"Provides heat protection in Death Mode");
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

        public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().calamityRing;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ObsidianRose);
            recipe.AddIngredient(ModContent.ItemType<Gehenna>());
            recipe.AddIngredient(ModContent.ItemType<CalamityRing>());
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>());
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
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
                if (Main.rand.NextBool(10))
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
						float x = player.position.X + (float)Main.rand.Next(-400, 400);
						float y = player.position.Y - (float)Main.rand.Next(500, 800);
						Vector2 source = new Vector2(x, y);
						Vector2 velocity = player.Center - source;
						velocity.X += (float)Main.rand.Next(-100, 101);
						float speed = 22f;
						float targetDist = velocity.Length();
						targetDist = speed / targetDist;
						velocity.X *= targetDist;
						velocity.Y *= targetDist;
						int fire = Projectile.NewProjectile(source, velocity, ModContent.ProjectileType<StandingFire>(), (int)(30 * player.AverageDamage()), 5f, player.whoAmI, 0f, 0f);
						Main.projectile[fire].ai[1] = player.position.Y;
						Main.projectile[fire].usesLocalNPCImmunity = true;
						Main.projectile[fire].localNPCHitCooldown = 60;
						Main.projectile[fire].usesIDStaticNPCImmunity = false;
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
                        int projSpeed = 25;
                        float spawnX = Main.rand.Next(1000) - 500 + player.Center.X;
                        float spawnY = -1000 + player.Center.Y;
                        Vector2 baseSpawn = new Vector2(spawnX, spawnY);
                        Vector2 baseVelocity = player.Center - baseSpawn;
                        baseVelocity.Normalize();
                        baseVelocity *= projSpeed;
                        for (int i = 0; i < FireProjectiles; i++)
                        {
                            Vector2 spawn = baseSpawn;
                            spawn.X += i * 30 - (FireProjectiles * 15);
                            Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-FireAngleSpread / 2 + (FireAngleSpread * i / (float)FireProjectiles)));
                            velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
                            Projectile.NewProjectile(spawn, velocity, ModContent.ProjectileType<BrimstoneHellfireballFriendly2>(), (int)(70 * player.AverageDamage()), 5f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
        }
    }
}
