using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class AquaticHeartWaterSpeed : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ocean's Blessing");
            Description.SetDefault("15% increased max speed and acceleration");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().aquaticHeartWaterBuff = true;
        }
    }
}
