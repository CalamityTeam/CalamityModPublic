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
    public class Hiveling : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hiveling");
            if (!Main.dedServ)
                glowmask = mod.GetTexture("NPCs/AstralBiomeNPCs/HivelingGlow");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 50;
            npc.height = 40;
            npc.aiStyle = -1;
            npc.damage = 50;
            npc.defense = 8;
            npc.lifeMax = 220;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyDeath");
            npc.knockBackResist = 0.4f;
            npc.noGravity = true;
            npc.value = 100f;
        }

        public override void AI()
        {
            if (npc.ai[1] == 0f)
            {
                npc.velocity *= 0.97f;

                npc.TargetClosest(false);
                if (Main.player[npc.target].dead)
                {
                    npc.TargetClosest(false);
                }
                Player targ = Main.player[npc.target];

                if (Collision.CanHit(npc.position, npc.width, npc.height, targ.position, targ.width, targ.height) || Vector2.Distance(npc.Center, targ.MountedCenter) < 320f)
                {
                    npc.ai[1] = 1f;
                }
            }
            else
            {
                CalamityGlobalNPC.DoFlyingAI(npc, 3f, 0.05f, 200f);
            }
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
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 30, frameHeight, mod.DustType("AstralOrange"), new Rectangle(16, 8, 6, 6), Vector2.Zero, 0.3f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            //draw glowmask
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition + new Vector2(0, 12), npc.frame, Color.White * 0.6f, npc.rotation, new Vector2(15, 10), 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit" + Main.rand.Next(3)), npc.Center);

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : mod.DustType("AstralEnemy"), 1f, 3, 20);

            //if dead do gores
            if (npc.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    Gore.NewGore(npc.Center, npc.velocity * 0.3f, mod.GetGoreSlot("Gores/Hadarian/HadarianGore" + i));
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }
    }
}
