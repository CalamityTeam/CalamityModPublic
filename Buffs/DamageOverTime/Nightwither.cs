using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Nightwither : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Nightwither");
            Description.SetDefault("Incinerated by lunar rays");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().nightwither = true;
        }
    }
}
