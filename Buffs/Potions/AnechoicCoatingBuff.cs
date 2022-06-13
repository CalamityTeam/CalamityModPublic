using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class AnechoicCoatingBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anechoic Coating");
            Description.SetDefault("Abyssal creatures' detection radii reduced");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().anechoicCoating = true;
        }
    }
}
