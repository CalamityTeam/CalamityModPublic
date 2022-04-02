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
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(gold: 45);
            item.defense = 44;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpookyLeggings);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddIngredient(ItemID.SoulofFright, 10);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 2);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
