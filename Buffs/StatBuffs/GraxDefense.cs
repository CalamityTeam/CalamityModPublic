using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatBuffs
{
    public class GraxDefense : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Grax Boost");
			Description.SetDefault("Your defenses and muscles are strong");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.Calamity().graxDefense = true;
		}
	}
}
