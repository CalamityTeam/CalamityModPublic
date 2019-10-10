using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.SummonBuffs
{
	public class BloodClot : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Blood Clot");
			Description.SetDefault("The blood clot will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.Calamity();
			if (player.ownedProjectileCounts[mod.ProjectileType("BloodClotMinion")] > 0)
			{
				modPlayer.bClot = true;
			}
			if (!modPlayer.bClot)
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
