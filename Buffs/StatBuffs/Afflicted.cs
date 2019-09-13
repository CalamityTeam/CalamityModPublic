using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatBuffs
{
    public class Afflicted : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Afflicted");
			Description.SetDefault("Empowered by otherworldly spirits");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).afflicted = true;
		}
	}
}
