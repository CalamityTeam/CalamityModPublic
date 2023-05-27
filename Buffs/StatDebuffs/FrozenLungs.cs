using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class FrozenLungs : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().iCantBreathe = true;
        }
    }
}
