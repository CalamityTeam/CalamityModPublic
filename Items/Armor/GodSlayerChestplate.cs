using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class GodSlayerChestplate : ModItem
    {
        public const int DashIFrames = 6;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Chestplate");
            Tooltip.SetDefault("+60 max life\n" +
                       "Enemies take damage when they hit you\n" +
                       "11% increased damage and 6% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.defense = 41;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.thorns += 0.5f;
            player.statLifeMax2 += 60;
            player.GetDamage<GenericDamageClass>() += 0.11f;
            modPlayer.AllCritBoost(6);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CosmiliteBar>(23).
                AddIngredient<AscendantSpiritEssence>(3).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
