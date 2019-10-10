using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.Fabsol
{
    public class Whiskey : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Whiskey");
			Description.SetDefault("Damage, critical strike chance, and knockback boosted, defense reduced");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.Calamity().whiskey = true;
		}
	}
}
