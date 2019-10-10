using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.Fabsol
{
    public class GrapeBeer : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Grape Beer");
			Description.SetDefault("Defense and movement speed reduced");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.Calamity().grapeBeer = true;
		}
	}
}
