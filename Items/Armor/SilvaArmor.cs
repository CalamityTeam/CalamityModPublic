using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class SilvaArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Armor");
            Tooltip.SetDefault("+80 max life\n" +
                       "12% increased damage and 8% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 24;
            item.value = Item.buyPrice(0, 72, 0, 0);
            item.defense = 44;
			item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 80;
            player.allDamage += 0.12f;
            player.Calamity().AllCritBoost(8);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<EffulgentFeather>(), 10);
            recipe.AddRecipeGroup("AnyGoldBar", 10);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 12);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 10);
            recipe.AddIngredient(ModContent.ItemType<LeadCore>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
