using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatBuffs
{
    public class ChaosCandle : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Chaos Candle");
			Description.SetDefault("Spawn rates around the candle are boosted!");
			Main.buffNoTimeDisplay[Type] = true;
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().chaosCandle = true;
		}
	}
}
