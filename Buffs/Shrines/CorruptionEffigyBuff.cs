using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.Shrines
{
    public class CorruptionEffigyBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Corruption Effigy");
			Description.SetDefault("The corruption empowers you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().corrEffigy = true;
		}
	}
}
