using System.Linq;
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
            BuffID.Sets.IsATagBuff[Type] = true;
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            var whipBuffs = new int[]
            {
                BuffID.BlandWhipEnemyDebuff, BuffID.FlameWhipEnemyDebuff, BuffID.BoneWhipNPCDebuff,
                BuffID.ScytheWhipEnemyDebuff, BuffID.CoolWhipNPCDebuff, BuffID.MaceWhipNPCDebuff,
                BuffID.RainbowWhipNPCDebuff, BuffID.SwordWhipNPCDebuff, BuffID.ThornWhipNPCDebuff
            };
            
            //kill whip stacking for psc purposes
            // 29SEP2023: Ozzatron: this won't kill stacking with other mod whips. need a generalized system for this
            for (int buff = 0; buff < NPC.maxBuffs; buff++)
            {
                int buffID = npc.buffType[buff];
                if(npc.buffTime[buff] > 0 && whipBuffs.Contains(buffID) && npc.buffType[buff] != Type)
                    npc.RequestBuffRemoval(npc.buffType[buff]);
            }
        }
    }
}
