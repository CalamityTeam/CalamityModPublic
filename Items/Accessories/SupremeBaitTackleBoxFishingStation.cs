using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SupremeBaitTackleBoxFishingStation : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Supreme Bait Tackle Box Fishing Station");
            Tooltip.SetDefault("The ultimate fishing accessory\n" +
                "Increases fishing skill by 80\n" +
                "Fishing line will never break and decreases chance of bait consumption\n" +
                "Increases chance to catch crates\n" +
                "Allows fishing in lava\n" +
                "Sonar potion effect");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
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
