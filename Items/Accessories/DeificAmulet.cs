using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DeificAmulet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deific Amulet");
			Tooltip.SetDefault("Taking damage makes you move very fast for a short time\n" +
				"Causes stars to fall and gives increased immune time when damaged\n" +
				"Increases armor penetration by 10 and reduces the cooldown of healing potions\n" +
				"Provides light underwater and provides a small amount of light in the abyss");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.rare = 5;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dAmulet = true;
            modPlayer.jellyfishNecklace = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CharmofMyths);
            recipe.AddIngredient(ItemID.JellyfishNecklace);
            recipe.AddIngredient(ItemID.PanicNecklace);
            recipe.AddIngredient(ItemID.SharkToothNecklace);
            recipe.AddIngredient(ItemID.StarVeil);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 25);
            recipe.AddIngredient(ItemID.MeteoriteBar, 25);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
