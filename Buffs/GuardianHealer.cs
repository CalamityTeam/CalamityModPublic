using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class GuardianHealer : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Healer Guardian");
			Description.SetDefault("The guardian will heal you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("MiniGuardianHealer")] > 0)
			{
				modPlayer.gHealer = true;
			}
			if (!modPlayer.gHealer)
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