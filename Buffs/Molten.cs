using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class Molten : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Molten");
			Description.SetDefault("Resistant to cold effects");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).molten = true;
		}
	}
}
