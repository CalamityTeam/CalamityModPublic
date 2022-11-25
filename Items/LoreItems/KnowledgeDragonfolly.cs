﻿using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeBumblebirb")]
    public class KnowledgeDragonfolly : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Dragonfolly");
            Tooltip.SetDefault("A failure of twisted scientific ambition; it appears our faulted arrogance over life has shown once more in the results.\n" +
                "Originally intended to be a clone of the Jungle Dragon, these were left to roam about the jungle, attacking anything in their path.");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Purple;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DragonfollyTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
