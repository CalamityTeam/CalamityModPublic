using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class ExtremeGravity : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icarus' Folly");
            Description.SetDefault("Your wing time is reduced by 33%, infinite flight is disabled");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().eGravity = true;
        }
    }
}
