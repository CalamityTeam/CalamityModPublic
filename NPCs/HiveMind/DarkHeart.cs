using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.HiveMind
{
    public class DarkHeart : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Heart");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.width = 32;
            NPC.height = 32;
            NPC.defense = 2;

            NPC.lifeMax = 150;
            if (BossRushEvent.BossRushActive)
                NPC.lifeMax = 1800;
            if (Main.getGoodWorld)
                NPC.lifeMax *= 4;

            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = BossRushEvent.BossRushActive ? 0f : 0.4f;
            NPC.noGravity = true;
            NPC.canGhostHeal = false;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath21;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<HiveMind>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCorruption,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("It pulses with stagnant, stinking waters of corruption, and leaks as it flies overhead. Wherever those drops land, any organic matter nearby slowly corrodes.")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            NPC.TargetClosest();
            float num1164 = revenge ? 4.5f : 4f;
            float num1165 = revenge ? 0.8f : 0.75f;
            if (BossRushEvent.BossRushActive)
            {
                num1164 *= 2f;
                num1165 *= 2f;
            }

            Vector2 vector133 = new Vector2(NPC.Center.X, NPC.Center.Y);
            float num1166 = Main.player[NPC.target].Center.X - vector133.X;
            float num1167 = Main.player[NPC.target].Center.Y - vector133.Y - 400f;
            float num1168 = (float)Math.Sqrt(num1166 * num1166 + num1167 * num1167);
            if (num1168 < 20f)
            {
                num1166 = NPC.velocity.X;
                num1167 = NPC.velocity.Y;
            }
            else
            {
                num1168 = num1164 / num1168;
                num1166 *= num1168;
                num1167 *= num1168;
            }
            if (NPC.velocity.X < num1166)
            {
                NPC.velocity.X = NPC.velocity.X + num1165;
                if (NPC.velocity.X < 0f && num1166 > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X + num1165 * 2f;
                }
            }
            else if (NPC.velocity.X > num1166)
            {
                NPC.velocity.X = NPC.velocity.X - num1165;
                if (NPC.velocity.X > 0f && num1166 < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X - num1165 * 2f;
                }
            }
            if (NPC.velocity.Y < num1167)
            {
                NPC.velocity.Y = NPC.velocity.Y + num1165;
                if (NPC.velocity.Y < 0f && num1167 > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + num1165 * 2f;
                }
            }
            else if (NPC.velocity.Y > num1167)
            {
                NPC.velocity.Y = NPC.velocity.Y - num1165;
                if (NPC.velocity.Y > 0f && num1167 < 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - num1165 * 2f;
                }
            }
            if (NPC.position.X + NPC.width > Main.player[NPC.target].position.X && NPC.position.X < Main.player[NPC.target].position.X + Main.player[NPC.target].width && NPC.position.Y + NPC.height < Main.player[NPC.target].position.Y && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.ai[0] += 1f;
                if (NPC.ai[0] >= (Main.getGoodWorld ? 12f : 24f))
                {
                    NPC.ai[0] = 0f;
                    int num1169 = (int)(NPC.position.X + 10f + Main.rand.Next(NPC.width - 20));
                    int num1170 = (int)(NPC.position.Y + NPC.height + 4f);
                    int type = ModContent.ProjectileType<ShaderainHostile>();
                    int damage = NPC.GetProjectileDamage(type);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), num1169, num1170, 0f, 4f, type, damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(4) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 14, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 14, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
