using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class CosmicFreeze : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Freeze");
            Description.SetDefault("You feel as cold as the empty expanse of outer space");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().cFreeze = true;
        }
    }
}
