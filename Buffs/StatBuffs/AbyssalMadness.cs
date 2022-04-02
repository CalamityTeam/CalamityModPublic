using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class AbyssalMadness : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Abyssal Madness");
            Description.SetDefault("Increased damage, critical strike chance, and tentacle aggression/range");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().omegaBlueHentai = true;
        }
    }
}
