using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class PlagueHive : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Hive");
            Tooltip.SetDefault("The power of bees and wasps will rival the Moon Lord himself\n" +
                   "All attacks inflict the Plague debuff\n" +
                   "Releases bees when damaged that inflict the Plague\n" +
                   "Projectiles spawn plague seekers on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 48;
            item.value = CalamityGlobalItem.Rarity9BuyPrice;
            item.rare = ItemRarityID.Cyan;
            item.accessory = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AlchemicalFlask>());
            recipe.AddIngredient(ItemID.HoneyComb);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.bee = true;
            modPlayer.uberBees = true;
            modPlayer.alchFlask = true;
        }
    }
}
