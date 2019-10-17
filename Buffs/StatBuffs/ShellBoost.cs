using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class ShellBoost : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Shell Speed Boost");
            Description.SetDefault("Speed is boosted");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().shellBoost = true;
        }
    }
}
