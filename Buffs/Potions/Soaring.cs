using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class Soaring : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soaring");
            Description.SetDefault("Increased wing flight time and speed\n" +
                "True melee hits restore wing flight time");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().soaring = true;
        }
    }
}
