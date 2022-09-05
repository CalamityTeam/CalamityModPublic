using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DynamicPursuer : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Dynamic Pursuer");
            Tooltip.SetDefault("A weapon that, as it flies, processes calculations and fires electricity\n" +
                               "Releases a flying disk that fires electricity at nearby enemies\n" +
                               "Stealth strikes allow the disk to ricochet multiple times and unleash an electric explosion, then fire inaccurate lasers while returning");
        }
        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.damage = 3100;
            Item.DamageType = RogueDamageClass.Instance;

            Item.width = 30;
            Item.height = 34;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = false;
            Item.knockBack = 3f;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();

            Item.noUseGraphic = true;

            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<DynamicPursuerProjectile>();
            Item.shootSpeed = 25f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 320f; // Tesla Cannon = 250f
            modItem.ChargePerUse = 0.5f; // Tesla Cannon = 0.9f
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 initialVelocity = velocity;
            int totalDamage = damage;
            if (player.Calamity().StealthStrikeAvailable())
            {
                initialVelocity *= 1.2f;
                totalDamage = (int)(damage * 0.15f);
            }
            int proj = Projectile.NewProjectile(source, position, initialVelocity, type, totalDamage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Eradicator>().
                AddIngredient<TrackingDisk>().
                AddIngredient<AuricBar>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
