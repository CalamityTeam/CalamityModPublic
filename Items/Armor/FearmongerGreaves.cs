using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class FearmongerGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fearmonger Greaves");
            Tooltip.SetDefault("6% increased damage\n" +
            "50% increased minion knockback\n" +
            "10% increased movement speed\n" +
            "Taking damage makes you move very fast for a short time");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(gold: 45);
            Item.defense = 44;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.06f;
            player.minionKB += 0.5f;
            player.moveSpeed += 0.1f;
            player.panic = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.SpookyLeggings).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10).AddIngredient(ItemID.SoulofFright, 10).AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 2).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
