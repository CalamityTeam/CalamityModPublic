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
            npc.damage = 85;
            npc.width = 56;
            npc.height = 54;
            npc.aiStyle = 41;
            npc.defense = 24;
            npc.lifeMax = 450;
            npc.knockBackResist = 0.5f;
            npc.value = 380f;
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit");
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyDeath");
        }

        public override void FindFrame(int frameHeight)
        {
            CalamityGlobalNPC.SpawnDustOnNPC(npc, 66, frameHeight, mod.DustType("AstralOrange"), new Rectangle(44, 18, 12, 12));
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
            CalamityGlobalNPC.DoHitDust(npc, hitDirection, mod.DustType("AstralOrange"), 1f, 4, 24);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            //draw glowmask
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition, npc.frame, Color.White * 0.6f, npc.rotation, new Vector2(33, 31), 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.GetModPlayer<CalamityPlayer>().ZoneAstral && !spawnInfo.player.ZoneRockLayerHeight)
            {
                return 0.15f;
            }
            return 0f;
        }
    }
}
