using Terraria.DataStructures;
using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class KingofConstellationsTenryu : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetDefaults()
        {
            Item.mana = 10;
            Item.damage = 213;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<BlackDragonHead>();
            Item.width = 44;
            Item.height = 62;
            Item.UseSound = NPCs.Yharon.Yharon.HitSound;
            Item.useAnimation = Item.useTime = 25;

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().donorItem = true;

            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Summon;
            Item.autoReuse = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.maxMinions >= 4;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 spawnPos = new Vector2(player.position.X + 200, player.position.Y - 200);
            Vector2 spawnPos2 = new Vector2(player.position.X - 200, player.position.Y - 200);
            SpawnDragon(ModContent.ProjectileType<WhiteDragonHead>(), ModContent.ProjectileType<WhiteDragonBody>(), ModContent.ProjectileType<WhiteDragonTail>(), spawnPos2, player, source, damage, knockback);
            SpawnDragon(ModContent.ProjectileType<BlackDragonHead>(), ModContent.ProjectileType<BlackDragonBody>(), ModContent.ProjectileType<BlackDragonTail>(), spawnPos, player, source, damage, knockback);
            return false;
        }

        public static void SpawnDragon(int head, int body, int tail, Vector2 spawnPos, Player player, EntitySource_ItemUse_WithAmmo source, int damage, float knockback)
        {
            bool Exists = false;
            int tailID = 0;
            foreach (var projectile in Main.projectile)
            {
                if (projectile.type == head && projectile.owner == player.whoAmI && projectile.active)
                {
                    Exists = true;
                }

                if (projectile.type == tail && projectile.owner == player.whoAmI && projectile.active)
                {
                    tailID = projectile.whoAmI;
                }
            }
            float foundSlotsCount = 0f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.minion && p.owner == player.whoAmI)
                {
                    foundSlotsCount += p.minionSlots;
                }
            }
            if (Exists)
            {
                if (!(foundSlotsCount + 0.5f > (float)player.maxMinions))
                {
                    //Projectile.NewProjectile(source, Main.projectile[tailID].Center, Vector2.Zero, body, damage, knockback, player.whoAmI);
                }
            }
            else
            {
                if (!(foundSlotsCount + 1f > (float)player.maxMinions))
                {
                    Projectile.NewProjectile(source, spawnPos, player.DirectionTo(Main.MouseWorld) * 3, head, damage, knockback, player.whoAmI);
                    Projectile.NewProjectile(source, spawnPos, Vector2.Zero * 3, tail, damage, knockback, player.whoAmI);
                    for (var i = 0; i < 20; i++)
                    {
                        Projectile.NewProjectile(source, spawnPos, Vector2.Zero * 3, body, damage, knockback, player.whoAmI);
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.StardustDragonStaff).
                AddIngredient(ItemID.LightShard).
                AddIngredient(ItemID.DarkShard).
                AddIngredient(ModContent.ItemType<ArmoredShell>(), 7).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

