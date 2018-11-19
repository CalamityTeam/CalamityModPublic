using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstralBiomeNPCs
{
    public class SmallSightseer : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Small Sightseer");
            Main.npcFrameCount[npc.type] = 4;

            if (!Main.dedServ)
				glowmask = mod.GetTexture("NPCs/AstralBiomeNPCs/SmallSightseerGlow");
        }

        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 40;
            npc.damage = 58;
            npc.defense = 26;
            npc.lifeMax = 460;
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit");
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyDeath");
            npc.noGravity = true;
            npc.knockBackResist = 0.48f;
            npc.value = 600;
            npc.aiStyle = -1;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.05f + npc.velocity.Length() * 0.667f;
            if (npc.frameCounter >= 8)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y > npc.height * 2)
                {
                    npc.frame.Y = 0;
                }
            }

            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 80, frameHeight, mod.DustType("AstralOrange"), new Rectangle(16, 8, 6, 6), Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void AI()
        {
            CalamityGlobalNPC.DoFlyingAI(npc, 5.8f, 0.03f, 350f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : mod.DustType("AstralEnemy"), 1f, 4, 22);

            //if dead do gores
            if (npc.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    float rand = Main.rand.NextFloat(-0.18f, 0.18f);
                    Gore.NewGore(npc.position + new Vector2(Main.rand.NextFloat(0f, npc.width), Main.rand.NextFloat(0f, npc.height)), npc.velocity * rand, mod.GetGoreSlot("Gores/SmallSightseer/SmallSightseerGore" + i));
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition + new Vector2(0, 4f), new Rectangle(0, npc.frame.Y, 80, npc.frame.Height), Color.White * 0.75f, npc.rotation, new Vector2(40f, 20f), npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Tile tile = Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY];
            if (spawnInfo.player.GetModPlayer<CalamityPlayer>().ZoneAstral && (spawnInfo.player.ZoneOverworldHeight || spawnInfo.player.ZoneDirtLayerHeight))
            {
                return spawnInfo.player.ZoneDesert ? 0.16f : (spawnInfo.player.ZoneRockLayerHeight ? 0.08f :  0.2f);
            }
            return 0f;
        }
    }
}
