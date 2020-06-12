using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class ScarfMeleeBoost : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Scarf Boost");
            Description.SetDefault("10% increased damage, 5% increased crit chance, and 5% increased melee speed");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().sMeleeBoost = true;
        }
    }
}
