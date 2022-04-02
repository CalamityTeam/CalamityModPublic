using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class Hive : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hive");
            if (!Main.dedServ)
                glowmask = ModContent.GetTexture("CalamityMod/NPCs/Astral/HiveGlow");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.width = 38;
            npc.height = 62;
            npc.aiStyle = -1;
            npc.damage = 55;
            npc.defense = 15;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 470;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 15, 0);
            banner = npc.type;
            bannerItem = ModContent.ItemType<HiveBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 90;
                npc.defense = 25;
                npc.lifeMax = 700;
            }
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void AI()
        {
            npc.ai[0]++;
            if (npc.ai[0] > (CalamityWorld.death ? 120f : 180f))
            {
                if (Main.rand.NextBool(100) && NPC.CountNPCS(ModContent.NPCType<Hiveling>()) < 10)
                {
                    npc.ai[0] = 0;

                    //spawn hiveling, it's ai[0] is the hive npc index.
                    int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<Hiveling>(), 0, npc.whoAmI);
                    Main.npc[n].velocity.X = Main.rand.NextFloat(-0.4f, 0.4f);
                    Main.npc[n].velocity.Y = Main.rand.NextFloat(-0.5f, -0.05f);
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter > 10)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y > frameHeight * 4)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            //draw glowmask
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition, npc.frame, Color.White * 0.6f, npc.rotation, new Vector2(19, 30), 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.soundDelay == 0)
            {
                npc.soundDelay = 15;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit"), npc.Center);
                        break;
                    case 1:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit2"), npc.Center);
                        break;
                    case 2:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit3"), npc.Center);
                        break;
                }
            }

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 3, 20);

            //if dead do gores
            if (npc.life <= 0)
            {
                int type = ModContent.NPCType<Hiveling>();
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].type == type)
                    {
                        Main.npc[i].ai[0] = -1f;
                    }
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player) || NPC.AnyNPCs(npc.type))
            {
                return 0f;
            }
            else if (spawnInfo.player.InAstral(2))
            {
                return 0.17f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<Stardust>(), 2, 3);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Stardust>(), Main.expertMode);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<HivePod>(), CalamityWorld.downedAstrageldon, 7, 1, 1);
        }
    }
}
