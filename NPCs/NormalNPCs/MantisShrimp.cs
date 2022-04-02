using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class MantisShrimp : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mantis Shrimp");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.damage = 200;
            npc.width = 40;
            npc.height = 24;
            npc.defense = 10;
            npc.DR_NERD(0.1f);
            npc.lifeMax = 30;
            npc.aiStyle = 3;
            aiType = NPCID.Crab;
            npc.value = Item.buyPrice(0, 0, 1, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<MantisShrimpBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = true;
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
                num78 = 3f - num79;
            }
            else
            {
                num78 = 2.5f - num79;
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

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(player.Center, Vector2.Zero, ProjectileID.SolarWhipSwordExplosion, 0, 0f, Main.myPlayer);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !Main.hardMode || spawnInfo.player.Calamity().ZoneSulphur)
                return 0f;

            return SpawnCondition.OceanMonster.Chance * 0.2f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<MantisClaws>(), NPC.downedPlantBoss, 0.2f);
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
            }
        }
    }
}
