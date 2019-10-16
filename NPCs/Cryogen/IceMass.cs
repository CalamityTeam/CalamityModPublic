using CalamityMod.World;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.NPCs
{
    public class IceMass : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aurora Spirit");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 86;
            npc.damage = 40;
            npc.width = 40;
            npc.height = 24;
            npc.defense = 8;
            npc.alpha = 100;
            npc.lifeMax = 220;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 30000;
            }
            npc.knockBackResist = 0f;
            animationType = 472;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath15;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Frostburn, 90, true);
            player.AddBuff(BuffID.Chilled, 60, true);
        }

        public override void AI()
        {
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.01f, 0.35f, 0.35f);
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CryoSpirit"), 1f);
            }
        }
    }
}
