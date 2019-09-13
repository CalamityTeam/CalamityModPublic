using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.Fabsol
{
    public class PurpleDefenseCandle : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Resilience");
			Description.SetDefault("Neither rain nor wind can snuff its undying flame");
			Main.buffNoTimeDisplay[Type] = true;
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).purpleCandle = true;
		}
	}
}
