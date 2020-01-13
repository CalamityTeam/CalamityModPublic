using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.Potions;

namespace CalamityMod.Items.Potions
{
    public class Baguette : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baguette");
            Tooltip.SetDefault("Minor improvements to all stats for 20 minutes\n" +
			"Boosts the effects of Red Wine\n" +
			"[c/FCE391:je suis Monte]");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.useTurn = true;
            item.maxStack = 30;
            item.useAnimation = 17;
            item.useTime = 17;
            item.rare = 1;
            item.useStyle = 2;
            item.UseSound = SoundID.Item2;
            item.consumable = true;
            item.value = Item.buyPrice(0, 0, 50, 0);
            item.buffType = ModContent.BuffType<BaguetteBuff>();
            item.buffTime = 18000;
            item.Calamity().postMoonLordRarity = 21;
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffID.WellFed, 72000);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Hay, 10);
            recipe.AddTile(TileID.Furnaces);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}