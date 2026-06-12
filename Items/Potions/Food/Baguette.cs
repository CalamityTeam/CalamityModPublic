using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.Potions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Food
{
    public class Baguette : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";

        public static int RedWineHealValue = 200;
        public static int RedWineRegenLoss = 4;
        public static int RedWineRegenLossDuration = CalamityUtils.SecondsToFrames(15);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RedWineRegenLoss.ToRegenPerSecond());

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Type] = new Color[3] {
                new Color(231, 137, 159),
                new Color(179, 104, 56),
                new Color(108, 47, 16)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HasBuff<RedWineBuff>())
            {
                Item.healLife = RedWineHealValue;
                Item.potion = true;
            }
            else
            {
                Item.healLife = 0;
                Item.potion = false;
            }
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(52, 38, BuffID.WellFed, CalamityUtils.MinutesToFrames(5));
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.Calamity().donorItem = true;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Food;
        }

        public override void OnConsumeItem(Player player)
        {
            // 5 minutes for both
            player.AddBuff(BuffID.WellFed, Item.buffTime);
            if (player.HasBuff<RedWineBuff>())
                player.AddBuff(ModContent.BuffType<BaguetteBuff>(), RedWineRegenLossDuration);
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
