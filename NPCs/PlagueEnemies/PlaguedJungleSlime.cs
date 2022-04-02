using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PlagueEnemies
{
    public class PlaguedJungleSlime : ModNPC
    {
        public float spikeTimer = 60f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pestilent Slime");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 1;
            npc.damage = 55;
            npc.width = 40;
            npc.height = 30;
            npc.defense = 12;
            npc.lifeMax = 350;
            npc.knockBackResist = 0f;
            animationType = NPCID.CorruptSlime;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.alpha = 60;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<PestilentSlimeBanner>();
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void AI()
        {
            if (spikeTimer > 0f)
            {
                spikeTimer -= 1f;
            }
            if (!npc.wet && !Main.player[npc.target].npcTypeNoAggro[npc.type])
            {
                Vector2 vector3 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num14 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector3.X;
                float num15 = Main.player[npc.target].position.Y - vector3.Y;
                float num16 = (float)Math.Sqrt((double)(num14 * num14 + num15 * num15));
                if (Main.expertMode && num16 < 120f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
                {
                    npc.ai[0] = -40f;
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.9f;
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient && spikeTimer == 0f)
                    {
                        Main.PlaySound(SoundID.Item42, npc.position);
                        for (int n = 0; n < 5; n++)
                        {
                            Vector2 vector4 = new Vector2((float)(n - 2), -4f);
                            vector4.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                            vector4.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                            vector4.Normalize();
                            vector4 *= 4f + (float)Main.rand.Next(-50, 51) * 0.01f;
                            Projectile.NewProjectile(vector3.X, vector3.Y, vector4.X, vector4.Y, ModContent.ProjectileType<PlagueStingerGoliathV2>(), 25, 0f, Main.myPlayer, 0f, 0f);
                            spikeTimer = 30f;
                        }
                    }
                }
                else if (num16 < 360f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
                {
                    npc.ai[0] = -40f;
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.9f;
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient && spikeTimer == 0f)
                    {
                        Main.PlaySound(SoundID.Item42, npc.position);
                        num15 = Main.player[npc.target].position.Y - vector3.Y - (float)Main.rand.Next(0, 200);
                        num16 = (float)Math.Sqrt((double)(num14 * num14 + num15 * num15));
                        num16 = 6.5f / num16;
                        num14 *= num16;
                        num15 *= num16;
                        spikeTimer = 50f;
                        Projectile.NewProjectile(vector3.X, vector3.Y, num14, num15, ModContent.ProjectileType<PlagueStingerGoliathV2>(), 22, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !NPC.downedGolemBoss || spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.HardmodeJungle.Chance * 0.09f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Plague, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/PlagueSounds/PlagueBoom" + Main.rand.Next(1, 5)), npc.Center);
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Plague, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ItemID.Stinger, Main.expertMode ? 0.5f : 0.25f);
            DropHelper.DropItem(npc, ModContent.ItemType<PlagueCellCluster>(), 1, 2);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Plague>(), 180, true);
        }
    }
}
