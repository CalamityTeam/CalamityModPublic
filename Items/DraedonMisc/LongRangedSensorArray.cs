using CalamityMod.Items.Materials;
using CalamityMod.Tiles.DraedonSummoner;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.TileEntities;
using CalamityMod.CustomRecipes;
using System.Collections.Generic;

namespace CalamityMod.Items.DraedonMisc
{
    public class LongRangedSensorArray : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Long Ranged Sensor Array");
            Tooltip.SetDefault("Can be placed on the Codebreaker");
        }

        public override void SetDefaults()
        {
            item.width = 52;
            item.height = 52;
            item.maxStack = 999;
            item.consumable = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Pink;
            item.useTime = item.useAnimation = 15;
        }

        public override bool UseItem(Player player) => true;

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 2);

        public override bool ConsumeItem(Player player)
        {
            Point placeTileCoords = Main.MouseWorld.ToTileCoordinates();
            Tile tile = CalamityUtils.ParanoidTileRetrieval(placeTileCoords.X, placeTileCoords.Y);
            float checkDistance = ((Player.tileRangeX + Player.tileRangeY) / 2f + player.blockRange) * 16f;

            if (Main.myPlayer == player.whoAmI && player.WithinRange(Main.MouseWorld, checkDistance) && tile.active() && tile.type == ModContent.TileType<CodebreakerTile>())
            {
                TECodebreaker codebreakerTileEntity = CalamityUtils.FindTileEntity<TECodebreaker>(placeTileCoords.X, placeTileCoords.Y, CodebreakerTile.Width, CodebreakerTile.Height, CodebreakerTile.SheetSquare);
                if (codebreakerTileEntity is null || codebreakerTileEntity.ContainsSensorArray)
                    return false;

                codebreakerTileEntity.ContainsSensorArray = true;
                codebreakerTileEntity.SyncConstituents((short)Main.myPlayer);
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 2);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 10);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 10);
            recipe.AddRecipeGroup("AnyMythrilBar", 10);
            recipe.AddIngredient(ItemID.Wire, 50);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
