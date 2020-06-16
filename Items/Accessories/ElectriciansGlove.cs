using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ElectriciansGlove : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electrician's Glove");
            Tooltip.SetDefault(@"Stealth strikes summon sparks on enemy hits
Stealth strikes also have +30 armor penetration, deal 10% more damage, and heal for 1 HP");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 40;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.accessory = true;
            item.rare = 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.electricianGlove = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FilthyGlove>());
            recipe.AddIngredient(ItemID.Wire, 100);
            recipe.AddIngredient(ItemID.HallowedBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodstainedGlove>());
            recipe.AddIngredient(ItemID.Wire, 100);
            recipe.AddIngredient(ItemID.HallowedBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
