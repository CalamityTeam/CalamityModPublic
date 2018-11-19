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
    public class FusionFeeder : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fusion Feeder");
            if (!Main.dedServ)
				glowmask = mod.GetTexture("NPCs/AstralBiomeNPCs/FusionFeederGlow");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.width = 120;
            npc.height = 24;
            npc.damage = 64;
            npc.aiStyle = 103;
            npc.lifeMax = 600;
            npc.defense = 22;
            npc.value = 900f;
            npc.knockBackResist = 0.7f;
            npc.behindTiles = true;
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit");
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyDeath");

            animationType = NPCID.SandShark;
        }

        public override void FindFrame(int frameHeight)
        {
            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 134, frameHeight, mod.DustType("AstralOrange"), new Rectangle(46, 4, 60, 6), Vector2.Zero, 0.55f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : mod.DustType("AstralEnemy"), 1f, 4, 25);

            //if dead do gores
            if (npc.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    float rand = Main.rand.NextFloat(-0.18f, 0.18f);
                    Gore.NewGore(npc.position + new Vector2(Main.rand.NextFloat(0f, npc.width), Main.rand.NextFloat(0f, npc.height)), npc.velocity * rand, mod.GetGoreSlot("Gores/FusionFeeder/FusionFeederGore" + i));
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 offset = new Vector2(0f, 10f);
            Vector2 origin = new Vector2(67f, 23f);

            //draw shark
            spriteBatch.Draw(Main.npcTexture[npc.type], npc.Center - Main.screenPosition + offset, npc.frame, drawColor, npc.rotation, origin, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            //draw glowmask
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition + offset, npc.frame, Color.White * 0.6f, npc.rotation, origin, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.GetModPlayer<CalamityPlayer>().ZoneAstral && spawnInfo.player.ZoneDesert)
            {
                return 0.14f;
            }
            return 0f;
        }
    }
}
