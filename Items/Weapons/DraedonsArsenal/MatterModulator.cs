using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class MatterModulator : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Matter Modulator");
            Tooltip.SetDefault("Using extra mass gained from collision with solid materials, it causes extra damage\n" +
            "Fires a burst of unstable matter that does significant damage after striking a tile\n" +
            "Before striking a tile, the matter pierces infinitely but deals little damage");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.width = 40;
            Item.height = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 84;
            Item.knockBack = 11f;
            Item.useTime = Item.useAnimation = 33;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = CommonCalamitySounds.PlasmaBoltSound;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();

            Item.shoot = ModContent.ProjectileType<UnstableMatter>();
            Item.shootSpeed = 7f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 85f;
            modItem.ChargePerUse = 0.075f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < Main.rand.Next(3, 5 + 1); i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.8f, 1.3f), type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 2);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(12).
                AddIngredient<DubiousPlating>(8).
                AddRecipeGroup("AnyMythrilBar", 10).
                AddIngredient(ItemID.SoulofFright, 20).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(2, out Predicate<Recipe> condition), condition).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
