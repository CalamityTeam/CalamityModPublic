using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AquaticScourge
{
    public class AquaticParasite : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Parasite");
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.damage = Main.hardMode ? 35 : 15;
            NPC.width = 28;
            NPC.height = 28;
            NPC.defense = 5;
            NPC.LifeMaxNERB(Main.hardMode ? 200 : 30);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 0, 60);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AquaticParasiteBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            bool flag15 = false;
            if (NPC.wet && NPC.ai[1] == 1f)
            {
                flag15 = true;
            }
            if (NPC.direction == 0)
            {
                NPC.TargetClosest(true);
            }
            if (flag15)
            {
                return;
            }
            if (!NPC.wet)
            {
                NPC.rotation += NPC.velocity.X * 0.1f;
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.98f;
                    if ((double)NPC.velocity.X > -0.01 && (double)NPC.velocity.X < 0.01)
                    {
                        NPC.velocity.X = 0f;
                    }
                }
                NPC.velocity.Y = NPC.velocity.Y + 0.2f;
                if (NPC.velocity.Y > 10f)
                {
                    NPC.velocity.Y = 10f;
                }
                NPC.ai[0] = 1f;
                return;
            }
            if (NPC.collideX)
            {
                NPC.velocity.X = NPC.velocity.X * -1f;
                NPC.direction *= -1;
            }
            if (NPC.collideY)
            {
                if (NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y = Math.Abs(NPC.velocity.Y) * -1f;
                    NPC.directionY = -1;
                    NPC.ai[0] = -1f;
                }
                else if (NPC.velocity.Y < 0f)
                {
                    NPC.velocity.Y = Math.Abs(NPC.velocity.Y);
                    NPC.directionY = 1;
                    NPC.ai[0] = 1f;
                }
            }
            bool flag16 = false;
            if (!Main.player[NPC.target].dead)
            {
                flag16 = true;
            }
            if (flag16)
            {
                NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) + 1.57f;
                NPC.velocity *= 0.975f;
                float num263 = 2f;
                if (NPC.velocity.X > -num263 && NPC.velocity.X < num263 && NPC.velocity.Y > -num263 && NPC.velocity.Y < num263)
                {
                    NPC.TargetClosest(true);
                    float num264 = 12f;
                    Vector2 vector31 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    float num265 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - vector31.X;
                    float num266 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - vector31.Y;
                    float num267 = (float)Math.Sqrt((double)(num265 * num265 + num266 * num266));
                    num267 = num264 / num267;
                    num265 *= num267;
                    num266 *= num267;
                    NPC.velocity.X = num265;
                    NPC.velocity.Y = num266;
                }
            }
            else
            {
                NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.02f;
                NPC.rotation = NPC.velocity.X * 0.4f;
                if (NPC.velocity.X < -1f || NPC.velocity.X > 1f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.95f;
                }
                if (NPC.ai[0] == -1f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.01f;
                    if (NPC.velocity.Y < -1f)
                    {
                        NPC.ai[0] = 1f;
                    }
                }
                else
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.01f;
                    if (NPC.velocity.Y > 1f)
                    {
                        NPC.ai[0] = -1f;
                    }
                }
                int num268 = (int)(NPC.position.X + (float)(NPC.width / 2)) / 16;
                int num269 = (int)(NPC.position.Y + (float)(NPC.height / 2)) / 16;
                if (Main.tile[num268, num269 - 1].LiquidAmount > 128)
                {
                    if (Main.tile[num268, num269 + 1].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                    else if (Main.tile[num268, num269 + 2].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                }
                else
                {
                    NPC.ai[0] = 1f;
                }
                if ((double)NPC.velocity.Y > 1.2 || (double)NPC.velocity.Y < -1.2)
                {
                    NPC.velocity.Y = NPC.velocity.Y * 0.99f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe)
            {
                return 0f;
            }
            if (spawnInfo.Player.InSulphur() && spawnInfo.Water)
            {
                return 0.2f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Irradiated>(), 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.Center, NPC.velocity, Mod.Find<ModGore>("Gores/AquaticScourgeGores/AquaticParasite1").Type, 1f);
                    Gore.NewGore(NPC.Center, NPC.velocity, Mod.Find<ModGore>("Gores/AquaticScourgeGores/AquaticParasite2").Type, 1f);
                }
            }
        }
    }
}
