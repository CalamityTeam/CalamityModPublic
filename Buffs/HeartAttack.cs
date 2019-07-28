using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class HeartAttack : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Heart Attack");
			Description.SetDefault("You survived a heart attack.  Boosts max life.");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).hAttack = true;
		}
	}
}
