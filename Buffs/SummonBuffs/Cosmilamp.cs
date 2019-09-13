using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.SummonBuffs
{
	public class Cosmilamp : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Cosmilamp");
			Description.SetDefault("The cosmilamp will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("Cosmilamp")] > 0)
			{
				modPlayer.cLamp = true;
			}
			if (!modPlayer.cLamp)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
			else
			{
				player.buffTime[buffIndex] = 18000;
			}
		}
	}
}
