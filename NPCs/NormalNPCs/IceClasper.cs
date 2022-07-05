using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class IceClasper : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Clasper");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 3f;
            NPC.noGravity = true;
            NPC.damage = 32;
            NPC.width = 40;
            NPC.height = 40;
            NPC.defense = 12;
            NPC.lifeMax = 600;
            NPC.knockBackResist = 0.35f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 25, 0);
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.rarity = 2;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<IceClasperBanner>();
            NPC.coldDamage = true;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = false;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundSnow,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("An enemy which knows no bounds in hunting its prey. In blizzards where visibility is low, they have been known to glaciate and capture travelers. What happens to the victims is unknown.")
            });
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge;
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead)
            {
                NPC.TargetClosest(true);
            }
            float num = revenge ? 7f : 6f;
            float num2 = revenge ? 0.07f : 0.06f;
            if (CalamityWorld.death)
            {
                num *= 1.5f;
                num2 *= 1.5f;
            }
            Vector2 vector = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float num4 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float num5 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            num4 = (float)((int)(num4 / 8f) * 8);
            num5 = (float)((int)(num5 / 8f) * 8);
            vector.X = (float)((int)(vector.X / 8f) * 8);
            vector.Y = (float)((int)(vector.Y / 8f) * 8);
            num4 -= vector.X;
            num5 -= vector.Y;
            float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
            float num7 = num6;
            bool flag = false;
            if (num6 > 600f)
            {
                flag = true;
            }
            if (num6 == 0f)
            {
                num4 = NPC.velocity.X;
                num5 = NPC.velocity.Y;
            }
            else
            {
                num6 = num / num6;
                num4 *= num6;
                num5 *= num6;
            }
            if (num7 > 100f)
            {
                NPC.ai[0] += 1f;
                if (NPC.ai[0] > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.023f;
                }
                else
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.023f;
                }
                if (NPC.ai[0] < -100f || NPC.ai[0] > 100f)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.023f;
                }
                else
                {
                    NPC.velocity.X = NPC.velocity.X - 0.023f;
                }
                if (NPC.ai[0] > 200f)
                {
                    NPC.ai[0] = -200f;
                }
            }
            if (Main.player[NPC.target].dead)
            {
                num4 = (float)NPC.direction * num / 2f;
                num5 = -num / 2f;
            }
            if (NPC.velocity.X < num4)
            {
                NPC.velocity.X = NPC.velocity.X + num2;
            }
            else if (NPC.velocity.X > num4)
            {
                NPC.velocity.X = NPC.velocity.X - num2;
            }
            if (NPC.velocity.Y < num5)
            {
                NPC.velocity.Y = NPC.velocity.Y + num2;
            }
            else if (NPC.velocity.Y > num5)
            {
                NPC.velocity.Y = NPC.velocity.Y - num2;
            }
            NPC.localAI[0] += 1f;
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] >= 150f)
            {
                NPC.localAI[0] = 0f;
                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    int num9 = ProjectileID.FrostBlastHostile;
                    int beam = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector.X, vector.Y, num4, num5, num9, 45, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[beam].timeLeft = 300;
                }
            }
            int num10 = (int)NPC.position.X + NPC.width / 2;
            int num11 = (int)NPC.position.Y + NPC.height / 2;
            num10 /= 16;
            num11 /= 16;
            if (!WorldGen.SolidTile(num10, num11))
            {
                Lighting.AddLight((int)((NPC.position.X + (float)(NPC.width / 2)) / 16f), (int)((NPC.position.Y + (float)(NPC.height / 2)) / 16f), 0f, 0.6f, 0.75f);
            }
            Vector2 vector92 = new Vector2(NPC.Center.X, NPC.Center.Y);
            float num740 = Main.player[NPC.target].Center.X - vector92.X;
            float num741 = Main.player[NPC.target].Center.Y - vector92.Y;
            NPC.rotation = (float)Math.Atan2((double)num741, (double)num740) - 1.57f;
            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
            {
                if (NPC.ai[2] > 0f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.ai[2] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[2] == 0f)
            {
                NPC.ai[1] += 1f;
            }
            if (NPC.ai[1] >= 150f)
            {
                NPC.ai[2] = 1f;
                NPC.ai[1] = 0f;
                NPC.netUpdate = true;
            }
            if (NPC.ai[2] == 0f)
            {
                NPC.alpha = 0;
                NPC.noTileCollide = false;
            }
            else
            {
                NPC.wet = false;
                NPC.alpha = 200;
                NPC.noTileCollide = true;
            }
            float num12 = 0.7f;
            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.oldVelocity.X * -num12;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                {
                    NPC.velocity.X = 2f;
                }
                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                {
                    NPC.velocity.X = -2f;
                }
            }
            if (NPC.collideY)
            {
                NPC.netUpdate = true;
                NPC.velocity.Y = NPC.oldVelocity.Y * -num12;
                if (NPC.velocity.Y > 0f && (double)NPC.velocity.Y < 1.5)
                {
                    NPC.velocity.Y = 2f;
                }
                if (NPC.velocity.Y < 0f && (double)NPC.velocity.Y > -1.5)
                {
                    NPC.velocity.Y = -2f;
                }
            }
            if (flag)
            {
                if ((NPC.velocity.X > 0f && num4 > 0f) || (NPC.velocity.X < 0f && num4 < 0f))
                {
                    if (Math.Abs(NPC.velocity.X) < 12f)
                    {
                        NPC.velocity.X = NPC.velocity.X * 1.05f;
                    }
                }
                else
                {
                    NPC.velocity.X = NPC.velocity.X * 0.9f;
                }
            }
            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
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
            return spawnInfo.Player.ZoneSnow &&
                !spawnInfo.Player.PillarZone() &&
                !spawnInfo.Player.ZoneDungeon &&
                !spawnInfo.Player.InSunkenSea() &&
                Main.hardMode && !spawnInfo.PlayerInTown && !spawnInfo.Player.ZoneOldOneArmy && !Main.snowMoon && !Main.pumpkinMoon ? 0.007f : 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Frostburn, 180, true);
            player.AddBuff(BuffID.Chilled, 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 92, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 92, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<EssenceofEleum>());
            npcLoot.Add(ModContent.ItemType<FrostBarrier>(), 5);
            npcLoot.Add(ModContent.ItemType<AncientIceChunk>(), 3);
        }
    }
}
