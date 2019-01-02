using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class Margarita : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Margarita");
			Tooltip.SetDefault(@"Makes you immune to most debuffs
Reduces defense by 6 and life regen by 3
One of the best drinks ever created, enjoy it while it lasts
Restores 200 life and mana");
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
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = mod.BuffType("Margarita");
            item.buffTime = 10800; //3 minutes
            item.value = Item.buyPrice(0, 23, 30, 0);
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
            player.statLife += 200;
            player.statMana += 200;
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
                player.HealEffect(200, true);
                player.ManaEffect(200);
            }
            player.AddBuff(mod.BuffType("Margarita"), 10800);
            player.AddBuff(BuffID.PotionSickness, (player.pStone ? 2700 : 3600));
        }
    }
}
