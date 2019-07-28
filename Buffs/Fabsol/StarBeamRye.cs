using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Fabsol
{
    public class StarBeamRye : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Star Beam Rye");
			Description.SetDefault("Max mana and magic damage increased. Defense, mana usage, and life regen reduced");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).starBeamRye = true;
		}
	}
}
