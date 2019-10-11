using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.Permafrost
{
    public class Popo : ModBuff
	{
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Popo");
            Description.SetDefault("You are a snowman now!");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.snowmanPrevious)
            {
                modPlayer.snowmanPower = true;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
	}
}
