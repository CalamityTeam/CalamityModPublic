using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.SummonBuffs
{
	public class GuardianOffense : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Offensive Guardian");
			Description.SetDefault("The attacker will fight for you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("MiniGuardianAttack")] > 0)
			{
				modPlayer.gOffense = true;
			}
			if (!modPlayer.gOffense)
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
