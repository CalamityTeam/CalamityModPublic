using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class Corroslime : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Corroslime");
			Description.SetDefault("The corroslime will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("Corroslime")] > 0)
			{
				modPlayer.cSlime = true;
			}
			if (!modPlayer.cSlime)
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