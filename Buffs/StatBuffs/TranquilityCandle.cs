using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatBuffs
{
    public class TranquilityCandle : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Tranquility Candle");
			Description.SetDefault("Spawn rates around the candle are reduced!");
			Main.buffNoTimeDisplay[Type] = true;
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.Calamity().tranquilityCandle = true;
		}
	}
}
