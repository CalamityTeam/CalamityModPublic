using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.TileEntities;
using CalamityMod.Tiles.DraedonSummoner;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CalamityMod.Items.DraedonMisc
{
    public class AuricQuantumCoolingCell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Quantum Cooling Cell");
            Tooltip.SetDefault("Can be placed in the Codebreaker, completing it\n" +
                "The completion of the Codebreaker allows you to make contact with its original creator\n" +
                "Attempting to do so may have dire consequences");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 44;
            item.maxStack = 999;
            item.consumable = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Red;
            item.useTime = item.useAnimation = 15;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FirstOrDefault(x => x.Name == "Tooltip2" && x.mod == "Terraria").overrideColor = Color.DarkRed;
            CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 5, true);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(item.Center, 1.2f * brightness, 0.4f * brightness, 0.8f);
        }

        public override bool UseItem(Player player) => true;

        public override bool ConsumeItem(Player player)
        {
            Point placeTileCoords = Main.MouseWorld.ToTileCoordinates();
            Tile tile = CalamityUtils.ParanoidTileRetrieval(placeTileCoords.X, placeTileCoords.Y);
            float checkDistance = ((Player.tileRangeX + Player.tileRangeY) / 2f + player.blockRange) * 16f;

            if (Main.myPlayer == player.whoAmI && player.WithinRange(Main.MouseWorld, checkDistance) && tile.active() && tile.type == ModContent.TileType<CodebreakerTile>())
            {
                TECodebreaker codebreakerTileEntity = CalamityUtils.FindTileEntity<TECodebreaker>(placeTileCoords.X, placeTileCoords.Y, CodebreakerTile.Width, CodebreakerTile.Height, CodebreakerTile.SheetSquare);
                if (codebreakerTileEntity is null || codebreakerTileEntity.ContainsCoolingCell || codebreakerTileEntity.DecryptionCountdown > 0)
                    return false;

                codebreakerTileEntity.ContainsCoolingCell = true;
                codebreakerTileEntity.SyncConstituents((short)Main.myPlayer);
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            // Old worlds can craft the cell immediately for the sake of being able to easily fight Draedon in endgame worlds.
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 5, true);
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 2);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 8);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 8);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 40);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>(), 6);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
