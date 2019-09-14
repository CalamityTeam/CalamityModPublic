using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.Fabsol
{
    public class TequilaSunrise : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Tequila Sunrise");
			Description.SetDefault("Damage, critical strike chance, damage reduction, defense, and knockback boosted during daytime, life regen reduced");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().tequilaSunrise = true;
		}
	}
}
