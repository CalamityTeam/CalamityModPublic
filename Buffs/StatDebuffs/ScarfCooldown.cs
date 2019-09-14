using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class ScarfCooldown : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Scarf Cooldown");
			Description.SetDefault("Your dodge is recharging");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().scarfCooldown = true;
		}
	}
}
