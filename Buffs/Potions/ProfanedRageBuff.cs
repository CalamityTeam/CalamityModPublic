using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class ProfanedRageBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Profaned Rage");
            Description.SetDefault("Increased critical strike chance");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().profanedRage = true;
        }
    }
}
