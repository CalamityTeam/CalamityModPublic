using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.NPCs.AstralBiomeNPCs
{
    public class AstralachneaGround : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astralachnea");

            Main.npcFrameCount[npc.type] = 5;

            if (!Main.dedServ)
				glowmask = mod.GetTexture("NPCs/AstralBiomeNPCs/AstralachneaGroundGlow");

            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            npc.width = 70;
            npc.height = 34;
            npc.aiStyle = 3;
            npc.damage = 90;
            npc.defense = 30;
            npc.lifeMax = 750;
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit");
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyDeath");
            npc.knockBackResist = 0.28f;
            npc.value = 900f;
            npc.buffImmune[20] = true;
            npc.buffImmune[31] = false;
            npc.timeLeft = NPC.activeTime * 2;

            animationType = NPCID.WallCreeper;
        }

        public override void AI()
        {
            npc.TargetClosest();
            if (Main.netMode != 1 && npc.velocity.Y == 0f)
            {
                int x = (int)npc.Center.X / 16;
                int y = (int)npc.Center.Y / 16;
                bool transform = false;
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (Main.tile[i, j] != null && Main.tile[i, j].wall > 0)
                        {
                            transform = true;
                        }
                    }
                }
                if (transform)
                {
                    npc.Transform(mod.NPCType("AstralachneaWall"));
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            //DO DUST
            int frame = npc.frame.Y / frameHeight;
            Rectangle rect = new Rectangle(62, 4, 14, 6);
            switch (frame)
            {
                case 1:
                    rect = new Rectangle(64, 6, 12, 6);
                    break;
                case 2:
                    rect = new Rectangle(58, 8, 22, 6);
                    break;
                case 3:
                    rect = new Rectangle(54, 8, 26, 8);
                    break;
                case 4:
                    rect = new Rectangle(58, 6, 20, 8);
                    break;
            }
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 80, frameHeight, mod.DustType("AstralOrange"), rect, Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : mod.DustType("AstralEnemy"), 1f, 4, 22);

            //if dead do gores
            if (npc.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    Gore.NewGore(npc.Center, npc.velocity * 0.3f, mod.GetGoreSlot("Gores/Astralachnea/AstralachneaGore" + i));
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 origin = new Vector2(40f, 21f);
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition, npc.frame, Color.White * 0.6f, npc.rotation, origin, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.GetModPlayer<CalamityPlayer>().ZoneAstral && spawnInfo.player.ZoneRockLayerHeight)
            {
                return 0.17f;
            }
            return 0f;
        }
    }
}
