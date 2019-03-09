using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.Permafrost
{
	public class PopoNoseless : ModBuff
	{
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Noseless Popo");
            Description.SetDefault("Your nose has been stolen!");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>();
            if (modPlayer.snowmanPrevious)
            {
                modPlayer.snowmanPower = true;
                modPlayer.snowmanNoseless = true;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
	}
}