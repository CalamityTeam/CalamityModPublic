using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.TileEntities;
using CalamityMod.Tiles.DraedonSummoner;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CalamityMod.Items.DraedonMisc
{
    public class AuricQuantumCoolingCell : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.DraedonItems";
        public static readonly SoundStyle InstallSound = new("CalamityMod/Sounds/Custom/Codebreaker/AuricQuantumCoolingCellInstallNew");
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 44;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 15;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 5, true);

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(Item.Center, 1.2f * brightness, 0.4f * brightness, 0.8f);
        }

        public override bool? UseItem(Player player) => true;

        public override bool ConsumeItem(Player player)
        {
            Point placeTileCoords = Main.MouseWorld.ToTileCoordinates();
            Tile tile = CalamityUtils.ParanoidTileRetrieval(placeTileCoords.X, placeTileCoords.Y);
            float checkDistance = ((Player.tileRangeX + Player.tileRangeY) / 2f + player.blockRange) * 16f;

            if (Main.myPlayer == player.whoAmI && player.WithinRange(Main.MouseWorld, checkDistance) && tile.HasTile && tile.TileType == ModContent.TileType<CodebreakerTile>())
            {
                SoundEngine.PlaySound(InstallSound, Main.player[Main.myPlayer].Center);

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
            CreateRecipe().
                AddIngredient<AuricBar>(2).
                AddIngredient<MysteriousCircuitry>(8).
                AddIngredient<DubiousPlating>(8).
                AddIngredient<EndothermicEnergy>(40).
                AddIngredient<CoreofEleum>(6).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(5, out Func<bool> condition), condition).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
