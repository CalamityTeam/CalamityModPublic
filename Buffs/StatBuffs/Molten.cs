using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class Molten : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Molten");
            Description.SetDefault("Resistant to cold effects");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().molten = true;
        }
    }
}
