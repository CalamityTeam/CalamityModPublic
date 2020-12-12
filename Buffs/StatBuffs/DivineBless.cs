using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class DivineBless : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Kami Injection");
            Description.SetDefault("+5% damage, increased health regen, attacks inflict Banishing Fire");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().divineBless = true;
        }
    }
}
