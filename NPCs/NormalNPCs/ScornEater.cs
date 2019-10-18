using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.NPCs
{
    public class ScornEater : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scorn Eater");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 3f;
            npc.aiStyle = -1;
            npc.damage = 90;
            npc.width = 160;
            npc.height = 160;
            npc.defense = 38;
            npc.Calamity().RevPlusDR(0.05f);
            npc.lifeMax = 16000;
            npc.knockBackResist = 0f;
            aiType = -1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.lavaImmune = true;
            npc.value = Item.buyPrice(0, 0, 50, 0);
            npc.HitSound = SoundID.NPCHit23;
            npc.DeathSound = SoundID.NPCDeath26;
            banner = npc.type;
            bannerItem = ModContent.ItemType<ScornEaterBanner>();
        }

        public override void AI()
        {
            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(true);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.velocity.X != 0f || npc.velocity.Y < 0f || (double)npc.velocity.Y > 0.9)
                    {
                        npc.ai[0] = 1f;
                        npc.netUpdate = true;
                        return;
                    }
                    npc.ai[0] = 1f;
                    npc.netUpdate = true;
                    return;
                }
            }
            else if (npc.velocity.Y == 0f)
            {
                npc.ai[2] += 1f;
                int num321 = 20;
                if (npc.ai[1] == 0f)
                {
                    num321 = 12;
                }
                if (npc.ai[2] < (float)num321)
                {
                    npc.velocity.X = npc.velocity.X * 0.9f;
                    return;
                }
                npc.ai[2] = 0f;
                npc.TargetClosest(true);
                if (npc.direction == 0)
                {
                    npc.direction = -1;
                }
                npc.spriteDirection = npc.direction;
                npc.ai[1] += 1f;
                npc.ai[3] += 1f;
                if (npc.ai[3] >= 4f)
                {
                    npc.ai[3] = 0f;
                    if (npc.ai[1] == 2f)
                    {
                        npc.velocity.X = (float)npc.direction * 15f;
                        npc.velocity.Y = -12f;
                        npc.ai[1] = 0f;
                    }
                    else
                    {
                        npc.velocity.X = (float)npc.direction * 21f;
                        npc.velocity.Y = -6f;
                    }
                }
                npc.netUpdate = true;
                return;
            }
            else
            {
                if (npc.direction == 1 && npc.velocity.X < 1f)
                {
                    npc.velocity.X = npc.velocity.X + 0.1f;
                    return;
                }
                if (npc.direction == -1 && npc.velocity.X > -1f)
                {
                    npc.velocity.X = npc.velocity.X - 0.1f;
                    return;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 1.0;
            if (Math.Abs(npc.velocity.X) <= 1f)
            {
                if (npc.frameCounter > 9.0)
                {
                    npc.frame.Y = npc.frame.Y + frameHeight;
                    npc.frameCounter = 0.0;
                }
                if (npc.frame.Y >= frameHeight * 5)
                {
                    npc.frame.Y = 0;
                }
            }
            else
            {
                if (npc.frameCounter > 9.0)
                {
                    npc.frame.Y = npc.frame.Y + frameHeight;
                    npc.frameCounter = 0.0;
                }
                if (npc.frame.Y < frameHeight * 5)
                {
                    npc.frame.Y = frameHeight * 5;
                }
                if (npc.frame.Y >= frameHeight * 7)
                {
                    npc.frame.Y = frameHeight * 5;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !NPC.downedMoonlord)
            {
                return 0f;
            }
            if (SpawnCondition.Underworld.Chance > 0f)
            {
                return SpawnCondition.Underworld.Chance / 4f;
            }
            return SpawnCondition.OverworldHallow.Chance / 4f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScornEater"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScornEater2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScornEater3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScornEater4"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScornEater5"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScornEater6"), 1f);
            }
        }

        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<UnholyEssence>(), Main.rand.Next(2, 5));
        }
    }
}
