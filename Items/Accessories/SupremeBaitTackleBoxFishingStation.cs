using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class SupremeBaitTackleBoxFishingStation : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Supreme Bait Tackle Box Fishing Station");
            Tooltip.SetDefault("The ultimate fishing accessory\n" +
                "Increases fishing skill by 80\n" +
                "Fishing line will never break and decreases chance of bait consumption\n" +
                "Increases chance to catch crates\n" +
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
                AddIngredient(ItemID.AnglerHat).
                AddIngredient(ItemID.AnglerVest).
                AddIngredient(ItemID.AnglerPants).
                AddIngredient(ItemID.LavaproofTackleBag).
                AddIngredient(ItemID.FishingPotion, 5).
                AddIngredient(ItemID.CratePotion, 5).
                AddIngredient(ItemID.SonarPotion, 5).
                AddIngredient(ItemID.MasterBait, 5).
                AddIngredient(ItemID.SoulofLight, 5).
                AddIngredient(ItemID.SoulofNight, 5).
                AddIngredient<SeaPrism>(10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
