using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class Vodka : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Vodka");
            Description.SetDefault("Damage and critical stike chance boosted, defense and life regen reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().vodka = true;
        }
    }
}
