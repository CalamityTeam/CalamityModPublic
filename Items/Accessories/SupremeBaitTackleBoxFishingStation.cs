using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SupremeBaitTackleBoxFishingStation : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 52;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.fishingSkill += 80;
            player.accFishingLine = true;
            player.accTackleBox = true;
            player.accLavaFishing = true;
            player.Calamity().fishingStation = true;
            player.sonarPotion = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LavaproofTackleBag).
                AddIngredient(ItemID.SonarPotion, 5).
                AddIngredient(ItemID.MasterBait, 5).
                AddIngredient<MolluskHusk>(5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
