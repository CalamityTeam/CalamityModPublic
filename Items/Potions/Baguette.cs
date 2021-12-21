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
            Tooltip.SetDefault("{$CommonItemTooltip.MinorStats}\n" +
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
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item2;
            item.consumable = true;

            item.value = Item.sellPrice(silver: 1);
            item.rare = ItemRarityID.Blue;
            item.Calamity().donorItem = true;

            item.buffType = ModContent.BuffType<BaguetteBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(300f);
        }

        public override void OnConsumeItem(Player player)
        {
            //5 minutes for both
            player.AddBuff(ModContent.BuffType<BaguetteBuff>(), CalamityUtils.SecondsToFrames(300f));
            player.AddBuff(BuffID.WellFed, CalamityUtils.SecondsToFrames(300f));
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
