using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class TarragonCloak : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Tarragon Cloak");
            Description.SetDefault("Contact damage is being reduced");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.Calamity().tarragonCloak = true;
		}
	}
}
