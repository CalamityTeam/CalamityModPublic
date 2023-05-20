using CalamityMod.Tiles.PlayerTurrets;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Plates;
using CalamityMod.CustomRecipes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;

namespace CalamityMod.Items.Placeables.PlaceableTurrets
{
    public class OnyxTurret : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Shoots a shotgun spread of bullets at nearby enemies\n" +
                "Cannot attack while a boss is alive"); */
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PlayerOnyxTurret>());

            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(14).
                AddIngredient<DubiousPlating>(20).
                AddIngredient<Onyxplate>(10).
                AddIngredient<BlightedGel>(50).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(1, out Func<bool> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
