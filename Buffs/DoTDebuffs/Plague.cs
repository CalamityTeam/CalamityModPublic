using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DoTDebuffs
{
    public class Plague : ModBuff
    {
        public static int DefenseReduction = 4;

        public override void SetDefaults()
        {
            DisplayName.SetDefault("Plague");
            Description.SetDefault("Rotting from the inside");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().pFlames = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().pFlames = true;
        }
    }
}
