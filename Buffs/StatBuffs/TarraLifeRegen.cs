using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatBuffs
{
    public class TarraLifeRegen : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Tarra Life");
			Description.SetDefault("Rapid healing");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.Calamity().tRegen = true;
		}
	}
}
