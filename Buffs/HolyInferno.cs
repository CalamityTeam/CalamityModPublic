using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class HolyInferno : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Holy Inferno");
			Description.SetDefault("You've gone too far from the Profaned Goddess!");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).hInferno = true;
		}
	}
}
