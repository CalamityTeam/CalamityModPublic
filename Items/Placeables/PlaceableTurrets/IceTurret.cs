﻿using CalamityMod.Tiles.PlayerTurrets;
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
    public class IceTurret : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PlayerIceTurret>());

            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Pink;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 2);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(14).
                AddIngredient<DubiousPlating>(20).
                AddIngredient<Elumplate>(10).
                AddIngredient<EssenceofEleum>(12).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(2, out Func<bool> condition), condition).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
