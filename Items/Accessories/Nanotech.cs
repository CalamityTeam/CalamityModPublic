using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class Nanotech : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nanotech");
            Tooltip.SetDefault("Rogue projectiles create nanoblades as they travel\n" +
                "Rogue weapons have a chance to instantly kill normal enemies\n" +
                "10% increased rogue damage, 5% increased rogue crit chance, and 15% increased rogue velocity\n" +
                "Whenever you crit an enemy with a rogue weapon your rogue damage increases\n" +
                "This effect can stack up to 250 times\n" +
                "Max rogue damage boost is 25%");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.accessory = true;
            item.Calamity().postMoonLordRarity = 20;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.nanotech = true;
            modPlayer.raiderTalisman = true;
            player.Calamity().throwingDamage += 0.1f;
            player.Calamity().throwingCrit += 5;
            player.Calamity().throwingVelocity += 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<RogueEmblem>());
            recipe.AddIngredient(ModContent.ItemType<RaidersTalisman>());
            recipe.AddIngredient(ItemID.MartianConduitPlating, 250);
            recipe.AddIngredient(ItemID.Nanites, 500);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 20);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 20);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
