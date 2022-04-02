using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class StatisCurse : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statis' Curse");
            Tooltip.SetDefault("Increases max minions by 3, does not stack with downgrades\n" +
                "10% increased minion damage\n" +
                "Increased minion knockback\n" +
                "Minions inflict holy flames and shadowflames on hit");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 4));
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.shadowMinions = true;
            modPlayer.holyMinions = true;
            player.minionKB += 2.75f;
            player.minionDamage += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StatisBlessing>());
            recipe.AddIngredient(ModContent.ItemType<TheFirstShadowflame>());
            recipe.AddIngredient(ItemID.FragmentStardust, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
