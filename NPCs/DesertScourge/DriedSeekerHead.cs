using CalamityMod.Events;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    public class DriedSeekerHead : ModNPC
    {
        bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dried Seeker");
        }

        public override void SetDefaults()
        {
            npc.damage = 14;
            npc.npcSlots = 5f;
            npc.width = 18;
            npc.height = 18;
            npc.lifeMax = 100;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 50000;
            }
            npc.aiStyle = 6;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;
        }

        public override void AI()
        {
            if (!TailSpawned)
            {
                int Previous = npc.whoAmI;
                for (int num36 = 0; num36 < 4; num36++)
                {
                    int lol;
                    if (num36 >= 0 && num36 < 3)
                    {
                        lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DriedSeekerBody>(), npc.whoAmI);
                    }
                    else
                    {
                        lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DriedSeekerTail>(), npc.whoAmI);
                    }
                    Main.npc[lol].realLife = npc.whoAmI;
                    Main.npc[lol].ai[2] = (float)npc.whoAmI;
                    Main.npc[lol].ai[1] = (float)Previous;
                    Main.npc[Previous].ai[0] = (float)lol;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                    Previous = lol;
                }
                TailSpawned = true;
            }
            if (Main.player[npc.target].dead)
            {
                npc.velocity.Y = npc.velocity.Y + 1f;
                if ((double)npc.position.Y > Main.worldSurface * 16.0)
                {
                    npc.velocity.Y = npc.velocity.Y + 1f;
                }
                if ((double)npc.position.Y > Main.rockLayer * 16.0)
                {
                    for (int num957 = 0; num957 < 200; num957++)
                    {
                        if (Main.npc[num957].aiStyle == npc.aiStyle)
                        {
                            Main.npc[num957].active = false;
                        }
                    }
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override bool PreNPCLoot()
        {
            return false;
        }
    }
}
