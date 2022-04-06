using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class ScornEater : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scorn Eater");
            Main.npcFrameCount[NPC.type] = 7;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 3f;
            NPC.aiStyle = -1;
            NPC.damage = 90;
            NPC.width = 160;
            NPC.height = 160;
            NPC.defense = 38;
            NPC.DR_NERD(0.05f);
            NPC.lifeMax = 9000;
            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.lavaImmune = true;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.DeathSound = Mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/ScornDeath");
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ScornEaterBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            if ((Main.player[NPC.target].position.Y > NPC.position.Y + (float)NPC.height && NPC.velocity.Y > 0f) || (Main.player[NPC.target].position.Y < NPC.position.Y + (float)NPC.height && NPC.velocity.Y < 0f))
                NPC.noTileCollide = true;
            else
                NPC.noTileCollide = false;

            if (NPC.velocity.Y == 0f)
            {
                NPC.ai[2] += 1f;
                int num321 = 20;
                if (NPC.ai[1] == 0f)
                {
                    num321 = 12;
                }
                if (CalamityWorld.death)
                {
                    num321 /= 4;
                }
                if (NPC.ai[2] < (float)num321)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.9f;
                    return;
                }
                NPC.ai[2] = 0f;
                if (NPC.direction == 0)
                {
                    NPC.direction = -1;
                }
                NPC.spriteDirection = NPC.direction;
                NPC.ai[1] += 1f;
                NPC.ai[3] += 1f;
                if (NPC.ai[3] >= 4f)
                {
                    NPC.ai[3] = 0f;
                    NPC.noTileCollide = true;
                    if (NPC.ai[1] == 2f)
                    {
                        NPC.velocity.X = (float)NPC.direction * 15f;

                        if (Main.player[NPC.target].position.Y < NPC.position.Y + (float)NPC.height)
                            NPC.velocity.Y = -12f;
                        else
                            NPC.velocity.Y = 12f;

                        NPC.ai[1] = 0f;
                    }
                    else
                    {
                        NPC.velocity.X = (float)NPC.direction * 21f;

                        if (Main.player[NPC.target].position.Y < NPC.position.Y + (float)NPC.height)
                            NPC.velocity.Y = -6f;
                        else
                            NPC.velocity.Y = 12f;
                    }
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/ScornJump"), (int)NPC.Center.X, (int)NPC.Center.Y);
                }
                NPC.netUpdate = true;
            }
            else
            {
                if (NPC.direction == 1 && NPC.velocity.X < 1f)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.1f;
                    return;
                }
                if (NPC.direction == -1 && NPC.velocity.X > -1f)
                {
                    NPC.velocity.X = NPC.velocity.X - 0.1f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1.0;
            if (Math.Abs(NPC.velocity.X) <= 1f)
            {
                if (NPC.frameCounter > 9.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 0;
                }
            }
            else
            {
                if (NPC.frameCounter > 9.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y < frameHeight * 5)
                {
                    NPC.frame.Y = frameHeight * 5;
                }
                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = frameHeight * 5;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !NPC.downedMoonlord)
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
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 7;
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/ScornHurt"), NPC.Center);
            }
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/ScornEater"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/ScornEater2"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/ScornEater3"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/ScornEater4"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/ScornEater5"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/ScornEater6"), 1f);
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(NPC, ModContent.ItemType<UnholyEssence>(), 2, 4);
        }
    }
}
