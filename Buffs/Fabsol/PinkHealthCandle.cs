using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.Fabsol
{
    public class PinkHealthCandle : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Vigor");
			Description.SetDefault("Its brilliant light suffuses those nearby with hope");
			Main.buffNoTimeDisplay[Type] = true;
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).pinkCandle = true;
		}
	}
}
