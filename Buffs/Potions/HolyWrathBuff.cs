using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class HolyWrathBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Holy Wrath");
            Description.SetDefault("Increased damage and all attacks inflict holy fire");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().holyWrath = true;
        }
    }
}
