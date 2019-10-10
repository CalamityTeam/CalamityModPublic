using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.SummonBuffs
{
	public class IceClasper : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Ice Clasper");
			Description.SetDefault("The ice clasper will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.Calamity();
			if (player.ownedProjectileCounts[mod.ProjectileType("IceClasper")] > 0)
			{
				modPlayer.iClasper = true;
			}
			if (!modPlayer.iClasper)
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
