using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.PotionBuffs
{
    public class PhotosynthesisBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Photosynthesis");
			Description.SetDefault("Life regen boosted, more during daytime, and hearts heal more HP");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).photosynthesis = true;
		}
	}
}
