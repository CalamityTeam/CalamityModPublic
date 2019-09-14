using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.SummonBuffs
{
	public class FungalClump : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Fungal Clump");
			Description.SetDefault("The fungal clump will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetCalamityPlayer();
			if (player.ownedProjectileCounts[mod.ProjectileType("FungalClump")] > 0)
			{
				modPlayer.fClump = true;
			}
			if (!modPlayer.fClump)
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
