using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class Afflicted : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Afflicted");
            Description.SetDefault("Empowered by otherworldly spirits");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().afflicted = true;
        }
    }
}
