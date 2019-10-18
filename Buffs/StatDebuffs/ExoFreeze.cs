using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class ExoFreeze : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Exo Freeze");
            Description.SetDefault("Cannot move");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().eFreeze = true;
        }
    }
}
