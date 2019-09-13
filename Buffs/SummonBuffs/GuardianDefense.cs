using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.SummonBuffs
{
	public class GuardianDefense : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Defensive Guardian");
			Description.SetDefault("The defender will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("MiniGuardianDefense")] > 0)
			{
				modPlayer.gDefense = true;
			}
			if (!modPlayer.gDefense)
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
