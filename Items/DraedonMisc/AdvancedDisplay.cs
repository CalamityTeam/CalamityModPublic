using CalamityMod.Items.Materials;
using CalamityMod.Tiles.DraedonSummoner;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.TileEntities;
using CalamityMod.CustomRecipes;
using System.Collections.Generic;
using System;

namespace CalamityMod.Items.DraedonMisc
{
    public class AdvancedDisplay : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Advanced Display");
            Tooltip.SetDefault("Can be placed on the Codebreaker\n" +
                "Allows you to decrypt the Underworld schematic");
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Yellow;
            Item.useTime = Item.useAnimation = 15;
        }

        public override bool? UseItem(Player player) => true;

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 3);

        public override bool ConsumeItem(Player player)
        {
            Point placeTileCoords = Main.MouseWorld.ToTileCoordinates();
            Tile tile = CalamityUtils.ParanoidTileRetrieval(placeTileCoords.X, placeTileCoords.Y);
            float checkDistance = ((Player.tileRangeX + Player.tileRangeY) / 2f + player.blockRange) * 16f;

            if (Main.myPlayer == player.whoAmI && player.WithinRange(Main.MouseWorld, checkDistance) && tile.HasTile && tile.TileType == ModContent.TileType<CodebreakerTile>())
            {
                TECodebreaker codebreakerTileEntity = CalamityUtils.FindTileEntity<TECodebreaker>(placeTileCoords.X, placeTileCoords.Y, CodebreakerTile.Width, CodebreakerTile.Height, CodebreakerTile.SheetSquare);
                if (codebreakerTileEntity is null || codebreakerTileEntity.ContainsAdvancedDisplay)
                    return false;

                codebreakerTileEntity.ContainsAdvancedDisplay = true;
                codebreakerTileEntity.SyncConstituents((short)Main.myPlayer);
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(10).
                AddIngredient<LifeAlloy>(3).
                AddIngredient(ItemID.Glass, 20).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(3, out Predicate<Recipe> condition), condition).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
