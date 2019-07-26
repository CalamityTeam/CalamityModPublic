using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class ElementalAxe : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Elemental Axe");
			Description.SetDefault("The elemental axe will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("ElementalAxe")] > 0)
			{
				modPlayer.eAxe = true;
			}
			if (!modPlayer.eAxe)
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
