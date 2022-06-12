using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class GraxBoost : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grax Boost");
            Description.SetDefault("Your defenses and muscles are strong");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().graxDefense = true;
        }
    }
}
