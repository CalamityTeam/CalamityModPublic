using CalamityMod.Buffs.Potions;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class Bloodfin : ModItem
    {
		public static int BuffType = ModContent.BuffType<BloodfinBoost>();
		public static int BuffDuration = 600;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodfin");
            Tooltip.SetDefault(@"The wonders of angiogenesis
Grants a buff that boosts life regen for 10 seconds
The life regen boost is stronger if below 75% health
10 second duration");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.height = 36;
            item.maxStack = 999;
            item.useTurn = true;
            item.value = Item.sellPrice(gold: 5);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.healLife = 240;
            item.potion = true;
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffType, BuffDuration);
        }
    }
}
