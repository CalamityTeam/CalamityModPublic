using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class Omniscience : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omniscience");
            Description.SetDefault("You can see everything");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().omniscience = true;
        }
    }
}
