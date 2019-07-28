using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class PenumbraBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Penumbra");
			Description.SetDefault("Rogue stealth builds during nighttime and eclipse while moving");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).penumbra = true;
		}
	}
}
