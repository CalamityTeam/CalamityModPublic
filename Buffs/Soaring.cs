using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class Soaring : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Soaring");
			Description.SetDefault("Increased wing flight time and speed");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).soaring = true;
		}
	}
}
