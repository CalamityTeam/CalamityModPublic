using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class EmpyreanRage : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Empyrean Rage");
            Description.SetDefault("Rage of the cosmos");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().xRage = true;
        }
    }
}
