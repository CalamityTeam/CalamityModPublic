using CalamityMod.DataStructures;
using CalamityMod.NPCs;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class GalvanicCorrosion : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
        }
        // Purely to make it get electric color in tooltips
        public static DebuffData debuffData = new DebuffData()
        {
            ElectricDebuffScaling = 1
        };

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().galvanicCorrosion = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().galvanicCorrosion = true;
        }
    }
}
