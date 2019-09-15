using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.Fabsol
{
    public class Moonshine : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Moonshine");
			Description.SetDefault("Defense and damage reduction boosted, life regen reduced");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().moonshine = true;
		}
	}
}
