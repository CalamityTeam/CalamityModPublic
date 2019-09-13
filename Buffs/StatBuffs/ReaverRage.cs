using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatBuffs
{
    public class ReaverRage : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Reaver Rage");
			Description.SetDefault("You are angry");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).rRage = true;
		}
	}
}
