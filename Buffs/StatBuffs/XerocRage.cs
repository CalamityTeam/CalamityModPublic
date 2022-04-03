using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class XerocRage : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Empyrean Rage");
            Description.SetDefault("Rage of the cosmos");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().xRage = true;
        }
    }
}
