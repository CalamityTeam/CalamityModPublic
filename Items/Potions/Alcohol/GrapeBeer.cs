using CalamityMod.Buffs.Alcohol;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class GrapeBeer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grape Beer");
            Tooltip.SetDefault(@"Restores 100 mana
This crap is abhorrent but you might like it
Reduces defense by 2 and movement speed by 5%");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = 1;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.healLife = 100;
            item.healMana = 100;
            item.consumable = true;
            item.potion = true;
            item.value = Item.buyPrice(0, 0, 65, 0);
        }

        public override bool CanUseItem(Player player)
        {
			item.healLife = player.Calamity().bloodPactBoost ? 150 : 100;
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
            player.AddBuff(ModContent.BuffType<GrapeBeerBuff>(), 3600);

			// fixes hardcoded potion sickness duration from quick heal (see CalamityPlayerMiscEffects.cs)
			player.Calamity().potionTimer = 2;
			return true;
        }
    }
}
