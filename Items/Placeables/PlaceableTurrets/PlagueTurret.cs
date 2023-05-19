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
    public class PlagueTurret : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Ejects homing plague missiles towards nearby enemies\n" +
                "Cannot attack while a boss is alive"); */
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PlayerPlagueTurret>());

            Item.value = Item.buyPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 3);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(14).
                AddIngredient<DubiousPlating>(20).
                AddIngredient<Plagueplate>(10).
                AddIngredient<InfectedArmorPlating>(12).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(3, out Func<bool> condition), condition).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
