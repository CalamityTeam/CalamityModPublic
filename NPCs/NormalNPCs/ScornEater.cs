using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
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
            npc.Calamity().canBreakPlayerDefense = true;
            npc.npcSlots = 3f;
            npc.aiStyle = -1;
            npc.damage = 90;
            npc.width = 160;
            npc.height = 160;
            npc.defense = 38;
            npc.DR_NERD(0.05f);
            npc.lifeMax = 9000;
            npc.knockBackResist = 0f;
            aiType = -1;
            npc.lavaImmune = true;
            npc.value = Item.buyPrice(0, 0, 50, 0);
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/ScornDeath");
            banner = npc.type;
            bannerItem = ModContent.ItemType<ScornEaterBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            npc.TargetClosest(true);
            if ((Main.player[npc.target].position.Y > npc.position.Y + (float)npc.height && npc.velocity.Y > 0f) || (Main.player[npc.target].position.Y < npc.position.Y + (float)npc.height && npc.velocity.Y < 0f))
                npc.noTileCollide = true;
            else
                npc.noTileCollide = false;

            if (npc.velocity.Y == 0f)
            {
                npc.ai[2] += 1f;
                int num321 = 20;
                if (npc.ai[1] == 0f)
                {
                    num321 = 12;
                }
                if (CalamityWorld.death)
                {
                    num321 /= 4;
                }
                if (npc.ai[2] < (float)num321)
                {
                    npc.velocity.X = npc.velocity.X * 0.9f;
                    return;
                }
                npc.ai[2] = 0f;
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
                    npc.noTileCollide = true;
                    if (npc.ai[1] == 2f)
                    {
                        npc.velocity.X = (float)npc.direction * 15f;

                        if (Main.player[npc.target].position.Y < npc.position.Y + (float)npc.height)
                            npc.velocity.Y = -12f;
                        else
                            npc.velocity.Y = 12f;

                        npc.ai[1] = 0f;
                    }
                    else
                    {
                        npc.velocity.X = (float)npc.direction * 21f;

                        if (Main.player[npc.target].position.Y < npc.position.Y + (float)npc.height)
                            npc.velocity.Y = -6f;
                        else
                            npc.velocity.Y = 12f;
                    }
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ScornJump"), (int)npc.Center.X, (int)npc.Center.Y);
                }
                npc.netUpdate = true;
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

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<HolyFlames>(), 180, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.soundDelay == 0)
            {
                npc.soundDelay = 7;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/ScornHurt"), npc.Center);
            }
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
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
            DropHelper.DropItem(npc, ModContent.ItemType<UnholyEssence>(), 2, 4);
        }
    }
}
