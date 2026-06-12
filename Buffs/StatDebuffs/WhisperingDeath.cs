using CalamityMod.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class WhisperingDeath : ModBuff
    {
        public static float PlayerDamageReduction = 0.2f;
        public static float EnemyDamageReduction = 0.1f;

        // Whispering Death does not deal DoT, but is classified as a sickness debuff.
        public static DebuffData debuffData = new DebuffData()
        {
            SicknessDebuffScaling = 1
        };
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().whisperingDeath = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().whisperingDeath = true;
        }
    }
}
