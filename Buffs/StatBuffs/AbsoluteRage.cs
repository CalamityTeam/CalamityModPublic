using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class AbsoluteRage : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Absolute Rage");
            Description.SetDefault("Anger hardens the heart. Boosts max life by 5%.");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().absoluteRage = true;
        }
    }
}
