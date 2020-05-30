using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class BurntOut : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Burnt Out");
            Description.SetDefault("You've been burnt out and your guardians are less effective");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().bOut = true;
        }
    }
}
