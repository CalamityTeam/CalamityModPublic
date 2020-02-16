using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ElementalGauntlet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Gauntlet");
            Tooltip.SetDefault("Melee attacks and projectiles inflict most debuffs\n" +
                "15% increased melee speed, damage, and 5% increased melee critical strike chance\n" +
                "Increased invincibility after taking damage\n" +
                "Temporary immunity to lava\n" +
                "Increased melee knockback\n" +
                "Melee attacks have a chance to instantly kill normal enemies\n" +
				"Provides heat and cold protection in Death Mode");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 38;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.eGauntlet = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FireGauntlet);
            recipe.AddIngredient(ModContent.ItemType<YharimsInsignia>());
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 20);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 20);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
