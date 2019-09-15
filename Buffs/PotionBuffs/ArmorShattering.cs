using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.PotionBuffs
{
    public class ArmorShattering : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Armor Shattering");
			Description.SetDefault("Melee and rogue attacks break enemy armor");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().armorShattering = true;
		}
	}
}
