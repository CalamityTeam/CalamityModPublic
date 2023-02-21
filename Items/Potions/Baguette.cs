using CalamityMod.Buffs.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 38;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item2;
            Item.consumable = true;

            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.Calamity().donorItem = true;

            Item.buffType = ModContent.BuffType<BaguetteBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(300f);
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Food;
		}

        public override void OnConsumeItem(Player player)
        {
            //5 minutes for both
            player.AddBuff(ModContent.BuffType<BaguetteBuff>(), CalamityUtils.SecondsToFrames(300f));
            player.AddBuff(BuffID.WellFed, CalamityUtils.SecondsToFrames(300f));
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Hay, 20).
                AddTile(TileID.Furnaces).
                Register();
        }
    }
}
