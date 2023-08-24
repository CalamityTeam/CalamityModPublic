using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class ArmorCrunch : ModBuff
    {
        public static int DefenseReduction = 15;
        public static float MultiplicativeDamageReductionPlayer = 0.33f;
        //20% dr reduction
        public static float MultiplicativeDamageReductionEnemy = 0.80f;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().aCrunch < npc.buffTime[buffIndex])
                npc.Calamity().aCrunch = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().aCrunch = true;
        }
    }
}
