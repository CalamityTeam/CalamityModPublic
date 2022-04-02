using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class SuperDummyNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Dummy");
        }

        public override void SetDefaults()
        {
            npc.width = 18;
            npc.height = 48;
            npc.damage = 0;
            npc.defense = 0;
            npc.lifeMax = 9999999;
            npc.HitSound = SoundID.NPCHit15;
            npc.DeathSound = SoundID.NPCDeath2;
            npc.value = 0f;
            npc.knockBackResist = 0f;
            npc.netAlways = true;
            npc.aiStyle = 0;
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            npc.lifeRegen += 2000000;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CheckDead()
        {
            if (npc.lifeRegen < 0)
            {
                npc.life = npc.lifeMax;
                return false;
            }
            return true;
        }
    }
}
