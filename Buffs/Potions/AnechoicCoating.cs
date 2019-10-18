using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class AnechoicCoating : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Anechoic Coating");
            Description.SetDefault("Abyssal creature's detection radius reduced");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().anechoicCoating = true;
        }
    }
}
