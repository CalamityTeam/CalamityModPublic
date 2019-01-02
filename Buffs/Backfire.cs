using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class Backfire : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Backfire");
			Description.SetDefault("Damage, defense, and wing time drastically reduced");
			Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
        {
            player.meleeDamage -= 0.9f;
            player.magicDamage -= 0.9f;
            player.rangedDamage -= 0.9f;
            player.thrownDamage -= 0.9f;
            player.minionDamage -= 0.9f;
            player.statDefense -= 20;
            player.endurance -= 0.2f;
            player.wingTimeMax /= 2;
        }
	}
}
