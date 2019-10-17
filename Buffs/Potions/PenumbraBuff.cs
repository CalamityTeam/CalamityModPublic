using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class PenumbraBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Penumbra");
            Description.SetDefault("Stealth regenerates at 10% speed while moving\n" +
                "The boost increases to 20% at night and 30% during an eclipse");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().penumbra = true;
        }
    }
}
