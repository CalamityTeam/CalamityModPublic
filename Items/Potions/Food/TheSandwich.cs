using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Food
{
	public class TheSandwich : ModItem, ILocalizedModType
	{
		public new string LocalizationCategory => "Items.Potions";
        public static int alcoholCapBoost = 2;
        public static float statBoost = 0.05f; // The player starts with negative stat boosts, so it is a bit less than you'd expect
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(statBoost.ToPercent());
        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 5;
			Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
			ItemID.Sets.FoodParticleColors[Type] = new Color[3] {
				new Color(132, 171, 87),
				new Color(174, 91, 107),
				new Color(218, 176, 133)
			};
			ItemID.Sets.IsFood[Type] = true;
        }

		public override void SetDefaults()
		{
			Item.DefaultToFood(70, 48, ModContent.BuffType<Malnourished>(), CalamityUtils.MinutesToFrames(10));
			Item.value = Item.buyPrice(silver: 55); // Same price as Pad Thai; Sold by Shady Salesman
			Item.rare = ItemRarityID.Blue;
		}
        public override void UseAnimation(Player player)
        {
            player.ClearBuff(BuffID.WellFed);
            player.ClearBuff(BuffID.WellFed2);
            player.ClearBuff(BuffID.WellFed3);
        }
	}
}
