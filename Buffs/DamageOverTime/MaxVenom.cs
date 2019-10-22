using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class MaxVenom : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Vile Toxins");
            Description.SetDefault("The illness has spread"); //nope, not an AHiT reference, definitely not
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().maxVenom = true;
        }
    }
}
