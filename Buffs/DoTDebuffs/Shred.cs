using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DoTDebuffs
{
    public class Shred : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Shred");
            Description.SetDefault("Blood");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().pShred = true;
        }
    }
}
