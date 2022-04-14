using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class DarkSunRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Dark Sun Ring");
            Tooltip.SetDefault("Contains the power of the dark sun\n" +
                "12% increase to damage and melee speed\n" +
                "+1 life regen, 15% increased pick speed and +2 max minions\n" +
                "Increased minion knockback\n" +
                "During the day the player has +3 life regen\n" +
                "During the night the player has +15 defense\n" +
                "Both of these bonuses are granted during an eclipse");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 7));
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 60;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.defense = 10;
            Item.lifeRegen = 1;
            Item.accessory = true;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.darkSunRing = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UeliaceBar>(10).
                AddIngredient<DarksunFragment>(20).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
