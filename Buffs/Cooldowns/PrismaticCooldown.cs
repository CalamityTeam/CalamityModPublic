using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
	public class PrismaticCooldown : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Prismatic Cooldown");
			Description.SetDefault("Your laser attack is recharging");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.Calamity().prismaticCooldown = true;
		}
	}
}
