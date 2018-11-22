using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items
{
	public class IronHeart : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Iron Heart");
			Tooltip.SetDefault("Makes dying while a boss is alive permanently kill you.\n" +
                "This mode cannot be toggled on and off.\n" +
                "Using this while a boss is alive will permanently kill you.\n" +
                "Cannot be activated if any boss has been killed.");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
			item.expert = true;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.UseSound = SoundID.Item119;
			item.consumable = true;
		}

        public override bool CanUseItem(Player player)
        {
            if (CalamityWorld.downedBossAny)
            {
                return false;
            }
            return true;
        }

        public override bool UseItem(Player player)
		{
			for (int doom = 0; doom < 200; doom++)
			{
				if (Main.npc[doom].active && Main.npc[doom].boss)
				{
					player.KillMeForGood();
					Main.npc[doom].active = false;
                    Main.npc[doom].netUpdate = true;
                }
			}
			if (!CalamityWorld.ironHeart)
			{
				CalamityWorld.ironHeart = true;
                if (Main.netMode == 2)
                {
                    NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                }
            }
			return true;
		}
	}
}