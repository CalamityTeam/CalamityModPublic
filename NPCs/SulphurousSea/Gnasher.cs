using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SulphurousSea
{
    public class Gnasher : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gnasher");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.damage = 25;
            npc.width = 50;
            npc.height = 36;
            npc.defense = 30;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 35;
            npc.knockBackResist = 0.25f;
            npc.aiStyle = 3;
            aiType = 67;
            npc.value = Item.buyPrice(0, 0, 0, 60);
            npc.HitSound = SoundID.NPCHit50;
            npc.DeathSound = SoundID.NPCDeath54;
            banner = npc.type;
            bannerItem = ModContent.ItemType<GnasherBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            npc.spriteDirection = (npc.direction > 0) ? -1 : 1;
            float num79 = (Main.player[npc.target].Center - npc.Center).Length();
            num79 *= 0.0025f;
            if ((double)num79 > 1.5)
            {
                num79 = 1.5f;
            }
            float num78;
            if (Main.expertMode)
            {
                num78 = 2.5f - num79;
            }
            else
            {
                num78 = 2.25f - num79;
            }
            num78 *= (CalamityWorld.death ? 1.2f : 0.8f);
            if (npc.velocity.X < -num78 || npc.velocity.X > num78)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity *= 0.8f;
                }
            }
            else if (npc.velocity.X < num78 && npc.direction == 1)
            {
                npc.velocity.X = npc.velocity.X + 1f;
                if (npc.velocity.X > num78)
                {
                    npc.velocity.X = num78;
                }
            }
            else if (npc.velocity.X > -num78 && npc.direction == -1)
            {
                npc.velocity.X = npc.velocity.X - 1f;
                if (npc.velocity.X < -num78)
                {
                    npc.velocity.X = -num78;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe)
            {
                return 0f;
            }
            if (spawnInfo.player.Calamity().ZoneSulphur)
            {
                return 0.2f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ItemID.TurtleShell, Main.hardMode, 10, 1, 1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Gnasher"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Gnasher2"), 1f);
            }
        }
    }
}
