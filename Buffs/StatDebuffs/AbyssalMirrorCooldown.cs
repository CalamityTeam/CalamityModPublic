using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class AbyssalMirrorCooldown : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Abyssal Evade Cooldown");
			Description.SetDefault("Your Abyssal Mirror's dodge is recharging");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).abyssalMirrorCooldown = true;
		}
	}
}
