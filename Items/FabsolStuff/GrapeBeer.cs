using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class GrapeBeer : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Grape Beer");
			Tooltip.SetDefault(@"Reduces life regen by 1, defense by 2, and movement speed by 5%
This crap is abhorrent but you might like it
Restores 100 life and mana");
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
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = mod.BuffType("GrapeBeer");
            item.buffTime = 3600; //3600 = 1 minute
            item.value = Item.buyPrice(0, 0, 65, 0);
		}

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override bool ConsumeItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override void OnConsumeItem(Player player)
        {
            player.statLife += 100;
            player.statMana += 100;
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
                player.HealEffect(100, true);
                player.ManaEffect(100);
            }
            player.AddBuff(mod.BuffType("GrapeBeer"), 3600);
            player.AddBuff(BuffID.PotionSickness, (player.pStone ? 2700 : 3600));
        }
    }
}
