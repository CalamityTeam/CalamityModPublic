using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class LivingDew : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Dew");
            Tooltip.SetDefault("5% increased damage reduction, +5 defense, and increased life regen while in the Jungle\n" +
            "Immunity to Poison");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.ZoneJungle)
            {
                player.lifeRegen += 1;
                player.statDefense += 5;
                player.endurance += 0.05f;
            }
            player.buffImmune[BuffID.Poisoned] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bezoar);
            recipe.AddIngredient(ItemID.Vine, 2);
            recipe.AddIngredient(ModContent.ItemType<MurkyPaste>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
