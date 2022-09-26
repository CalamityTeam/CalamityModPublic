using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class HolyInferno : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Inferno");
            Description.SetDefault("You've gone too far from the Profaned Goddess!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().hInferno = true;
        }
    }
}
