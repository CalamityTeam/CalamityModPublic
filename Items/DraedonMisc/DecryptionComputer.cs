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
    public class DecryptionComputer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Decryption Computer");
            Tooltip.SetDefault("Can be placed on the Codebreaker\n" +
                "Allows you to decrypt advanced schematics\n" +
                "Doing so allows you to learn how to craft new things");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 40;
            item.maxStack = 999;
            item.consumable = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Orange;
            item.useTime = item.useAnimation = 15;
        }

        public override bool UseItem(Player player) => true;

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);

        public override bool ConsumeItem(Player player)
        {
            Point placeTileCoords = Main.MouseWorld.ToTileCoordinates();
            Tile tile = CalamityUtils.ParanoidTileRetrieval(placeTileCoords.X, placeTileCoords.Y);
            float checkDistance = ((Player.tileRangeX + Player.tileRangeY) / 2f + player.blockRange) * 16f;

            if (Main.myPlayer == player.whoAmI && player.WithinRange(Main.MouseWorld, checkDistance) && tile.active() && tile.type == ModContent.TileType<CodebreakerTile>())
            {
                TECodebreaker codebreakerTileEntity = CalamityUtils.FindTileEntity<TECodebreaker>(placeTileCoords.X, placeTileCoords.Y, CodebreakerTile.Width, CodebreakerTile.Height, CodebreakerTile.SheetSquare);
                if (codebreakerTileEntity is null || codebreakerTileEntity.ContainsDecryptionComputer)
                    return false;

                codebreakerTileEntity.ContainsDecryptionComputer = true;
                codebreakerTileEntity.SyncConstituents((short)Main.myPlayer);
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 1);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 18);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 10);
            // You do not need the Decryption Computer for the Tier 1 (Sunken Sea) schematic.
            // As such, its recipe can contain anything that is pre-Mechs (where Tier 2 is).
            // Wires are thus fair game to include without causing any issues.
            recipe.AddIngredient(ItemID.Wire, 100);
            recipe.AddRecipeGroup("AnyCopperBar", 10);
            recipe.AddIngredient(ItemID.Glass, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
