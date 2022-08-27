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
    public class DecryptionComputer : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Decryption Computer");
            Tooltip.SetDefault("Can be placed on the Codebreaker\n" +
                "Allows you to decrypt the Planetoid schematic\n" +
                "Doing so allows you to learn how to craft new things");
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 40;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Orange;
            Item.useTime = Item.useAnimation = 15;
        }

        public override bool? UseItem(Player player) => true;

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);

        public override bool ConsumeItem(Player player)
        {
            Point placeTileCoords = Main.MouseWorld.ToTileCoordinates();
            Tile tile = CalamityUtils.ParanoidTileRetrieval(placeTileCoords.X, placeTileCoords.Y);
            float checkDistance = ((Player.tileRangeX + Player.tileRangeY) / 2f + player.blockRange) * 16f;

            if (Main.myPlayer == player.whoAmI && player.WithinRange(Main.MouseWorld, checkDistance) && tile.HasTile && tile.TileType == ModContent.TileType<CodebreakerTile>())
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
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(18).
                AddIngredient<DubiousPlating>(10).
                AddIngredient(ItemID.Wire, 100).
                AddRecipeGroup("AnyCopperBar", 10).
                AddIngredient(ItemID.Glass, 15).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(1, out Predicate<Recipe> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
