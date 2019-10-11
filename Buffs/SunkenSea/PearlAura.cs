using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.SunkenSea
{
    public class PearlAura : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Pearl Aura");
            Description.SetDefault("Slowed down");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().pearlAura = true;
        }
    }
}
