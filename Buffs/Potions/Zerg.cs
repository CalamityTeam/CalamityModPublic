using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class Zerg : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Zerg");
            Description.SetDefault("Spawn rates are boosted");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().zerg = true;
        }
    }
}
