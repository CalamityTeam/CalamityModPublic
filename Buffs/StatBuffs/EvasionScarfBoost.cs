using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class EvasionScarfBoost : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Evasion Scarf Boost");
            Description.SetDefault("10% increased damage, 10% increased crit chance, and 15% increased melee speed");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().eScarfBoost = true;
        }
    }
}
