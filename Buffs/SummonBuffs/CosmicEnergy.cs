using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.SummonBuffs
{
	public class CosmicEnergy : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Cosmic Energy");
			Description.SetDefault("The cosmic energy will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.Calamity();
			if (player.ownedProjectileCounts[mod.ProjectileType("CosmicEnergy")] > 0)
			{
				modPlayer.cEnergy = true;
			}
			if (!modPlayer.cEnergy)
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
