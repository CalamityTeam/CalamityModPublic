﻿
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.NPCs
{
    public class Aries : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aries");
            Main.npcFrameCount[npc.type] = 8;
            if (!Main.dedServ)
                glowmask = mod.GetTexture("NPCs/AstralBiomeNPCs/AriesGlow");
        }

        public override void SetDefaults()
        {
            npc.damage = 50;
            npc.width = 56;
            npc.height = 54;
            npc.aiStyle = 41;
            npc.defense = 14;
            npc.Calamity().RevPlusDR(0.15f);
            npc.lifeMax = 300;
            npc.knockBackResist = 0.6f;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            banner = npc.type;
            bannerItem = ModContent.ItemType<AriesBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 85;
                npc.defense = 24;
                npc.knockBackResist = 0.5f;
                npc.lifeMax = 450;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            CalamityGlobalNPC.SpawnDustOnNPC(npc, 66, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(44, 18, 12, 12));
            if (npc.velocity.Y == 0f)
            {
                npc.frame.Y = 0;
            }
            else if ((double)npc.velocity.Y < -1.5)
            {
                npc.frame.Y = frameHeight * 7;
            }
            else if ((double)npc.velocity.Y < 0)
            {
                npc.frame.Y = frameHeight * 4;
            }
            else if ((double)npc.velocity.Y > 1.5)
            {
                npc.frame.Y = frameHeight * 6;
            }
            else
            {
                npc.frame.Y = frameHeight * 5;
            }
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

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, ModContent.DustType<AstralOrange>(), 1f, 4, 24);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            //draw glowmask
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition, npc.frame, Color.White * 0.6f, npc.rotation, new Vector2(33, 31), 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneAstral && !spawnInfo.player.ZoneRockLayerHeight)
            {
                return 0.15f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<Stardust>(), Main.rand.Next(1, 3));
            }
            if (Main.expertMode)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<Stardust>());
            }
            if (CalamityWorld.downedAstrageldon && Main.rand.NextBool(7))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<StellarKnife>());
            }
        }
    }
}
