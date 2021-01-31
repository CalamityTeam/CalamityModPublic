using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class DemonshadeBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demonshade Breastplate");
            Tooltip.SetDefault("20% increased melee speed, 15% increased damage and critical strike chance\n" +
                "Enemies take ungodly damage when they touch you\n" +
                "Increased max life and mana by 200\n" +
                "Standing still lets you absorb the shadows and boost your life regen");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.defense = 50;
            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
            item.Calamity().devItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.shadeRegen = true;
            player.thorns += 100f;
            player.statLifeMax2 += 200;
            player.statManaMax2 += 200;
            player.allDamage += 0.15f;
            modPlayer.AllCritBoost(15);
            player.meleeSpeed += 0.2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 15);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
