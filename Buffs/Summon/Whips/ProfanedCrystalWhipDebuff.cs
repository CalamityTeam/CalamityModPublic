using CalamityMod.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon.Whips
{
    public class ProfanedCrystalWhipDebuff : ModBuff
    {
        public override string Texture => "CalamityMod/Buffs/Summon/Whips/SentinalLash";

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            // var whipBuffs = new int[]
            // {
            //     BuffID.BlandWhipEnemyDebuff, BuffID.FlameWhipEnemyDebuff, BuffID.BoneWhipNPCDebuff,
            //     BuffID.ScytheWhipEnemyDebuff, BuffID.CoolWhipNPCDebuff, BuffID.MaceWhipNPCDebuff,
            //     BuffID.RainbowWhipNPCDebuff, BuffID.SwordWhipNPCDebuff, BuffID.ThornWhipNPCDebuff
            // };
            
            //kill whip stacking for psc purposes
            for (int buff = 0; buff < NPC.maxBuffs; buff++)
            {
                if(npc.buffTime[buff] > 0 && BuffID.Sets.IsAnNPCWhipDebuff[npc.buffType[buff]] && npc.buffType[buff] != Type)
                    npc.RequestBuffRemoval(npc.buffType[buff]);
            }
        }
    }
}
