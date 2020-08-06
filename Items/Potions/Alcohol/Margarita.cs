using CalamityMod.Buffs.Alcohol;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Margarita : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Margarita");
            Tooltip.SetDefault(@"Restores 200 mana
One of the best drinks ever created, enjoy it while it lasts
Reduces the duration of most debuffs
Reduces defense by 6 and life regen by 1");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = 5;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.potion = true;
            item.healLife = 200;
            item.healMana = 200;
            item.buffType = ModContent.BuffType<MargaritaBuff>();
            item.buffTime = 10800;
            item.value = Item.buyPrice(0, 23, 30, 0);
        }

        public override bool CanUseItem(Player player)
        {
			item.healLife = player.Calamity().bloodPactBoost ? 300 : 200;
            return player.potionDelay <= 0 && player.Calamity().potionTimer <= 0;
        }

		public override bool UseItem(Player player)
		{
			if (PlayerInput.Triggers.JustPressed.QuickBuff)
			{
				int healAmt = CalamityWorld.ironHeart ? 0 : item.healLife;
				player.statLife += healAmt;
				player.statMana += item.healMana;
				if (player.statLife > player.statLifeMax2)
				{
					player.statLife = player.statLifeMax2;
				}
				if (player.statMana > player.statManaMax2)
				{
					player.statMana = player.statManaMax2;
				}
				player.AddBuff(BuffID.ManaSickness, Player.manaSickTime, true);
				if (Main.myPlayer == player.whoAmI)
				{
					if (!CalamityWorld.ironHeart)
						player.HealEffect(healAmt, true);
					player.ManaEffect(item.healMana);
				}
			}
            player.AddBuff(ModContent.BuffType<MargaritaBuff>(), item.buffTime);

			// fixes hardcoded potion sickness duration from quick heal (see CalamityPlayerMiscEffects.cs)
			player.Calamity().potionTimer = 2;
			return true;
        }
    }
}
