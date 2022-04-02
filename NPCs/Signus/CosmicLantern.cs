using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Signus
{
    public class CosmicLantern : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Lantern");
            Main.npcFrameCount[npc.type] = 4;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.GetNPCDamage();
            npc.width = 25;
            npc.height = 25;
            npc.defense = 50;
            npc.lifeMax = 25;
            npc.alpha = 255;
            npc.knockBackResist = 0.85f;
            npc.noGravity = true;
            npc.dontTakeDamage = true;
            npc.chaseable = false;
            npc.canGhostHeal = false;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit53;
            npc.DeathSound = SoundID.NPCDeath44;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.signus < 0 || !Main.npc[CalamityGlobalNPC.signus].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.3f, 0.3f, 0.3f);

            npc.alpha -= 3;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
                int num1262 = Dust.NewDust(npc.position, npc.width, npc.height, 204, 0f, 0f, 0, default, 0.25f);
                Main.dust[num1262].velocity *= 0.1f;
                Main.dust[num1262].noGravity = true;
            }

            bool revenge = CalamityWorld.revenge;
            float playerDistNormMult = revenge ? 24f : 22f;
            if (BossRushEvent.BossRushActive || CalamityWorld.malice)
                playerDistNormMult = 30f;
            CalamityAI.DungeonSpiritAI(npc, mod, playerDistNormMult, 0f, true);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = Main.npcTexture[npc.type];
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = lightColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = npc.GetAlpha(color38);
                    color38 *= (float)(num153 - num155) / 15f;
                    Vector2 vector41 = npc.oldPos[num155] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
                    vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = npc.Center - Main.screenPosition;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Signus/CosmicLanternGlow");
            Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 = npc.GetAlpha(color41);
                    color41 *= (float)(num153 - num163) / 15f;
                    Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
                    vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
                    vector44 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            return false;
        }

        public override bool PreNPCLoot() => false;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return npc.alpha == 0;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 204, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
