using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.Potions;
using CalamityMod.World;
using System.Collections.Generic;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class Bloodfin : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodfin");
            Tooltip.SetDefault(@"The wonders of angiogenesis
Grants a buff that boosts life regen for 10 seconds
The life regen boost is stronger if below 75% health");
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
			item.buffType = ModContent.BuffType<BloodfinBoost>();
			item.buffTime = 600;
        }

		public override bool CanUseItem(Player player) => player.potionDelay <= 0 && player.Calamity().potionTimer <= 0;

        public override bool CanUseItem(Player player)
        {
            if (player.Calamity().bloodPactBoost)
            {
                item.healLife = 360;
            }
            else
            {
                item.healLife = 240;
            }
            return base.CanUseItem(player);
        }

		public override bool UseItem(Player player)
		{
			if (PlayerInput.Triggers.JustPressed.QuickBuff)
			{
				int healAmt = CalamityWorld.ironHeart ? 0 : item.healLife;
				player.statLife += healAmt;
				if (player.statLife > player.statLifeMax2)
				{
					player.statLife = player.statLifeMax2;
				}
				if (Main.myPlayer == player.whoAmI)
				{
					if (!CalamityWorld.ironHeart)
						player.HealEffect(healAmt, true);
				}
			}
            player.AddBuff(ModContent.BuffType<BloodfinBoost>(), item.buffTime);

			// fixes hardcoded potion sickness duration from quick heal (see CalamityPlayerMiscEffects.cs)
			player.Calamity().potionTimer = 2;
			return true;
        }
    }
}
