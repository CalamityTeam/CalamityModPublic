using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class EidolonWyrmBodyAlt : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eidolon Wyrm");
        }

        public override void SetDefaults()
        {
            NPC.damage = 100; //70
            NPC.width = 48; //36
            NPC.height = 58;
            NPC.defense = 0;
            NPC.lifeMax = 160000;
            NPC.aiStyle = -1; //new
            AIType = -1; //new
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
            NPC.chaseable = false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            NPC.damage = 0;

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Check if other segments are still alive, if not, die
            bool shouldDespawn = true;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<EidolonWyrmHead>())
                {
                    shouldDespawn = false;
                    break;
                }
            }
            if (!shouldDespawn)
            {
                if (NPC.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                NPC.active = false;
            }

            if (Main.npc[(int)NPC.ai[1]].alpha < 128)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            Vector2 vector18 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float num191 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float num192 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    num191 = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - vector18.Y;
                } catch
                {
                }
                NPC.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                int num194 = NPC.width;
                num193 = (num193 - (float)num194) / num193;
                num191 *= num193;
                num192 *= num193;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + num191;
                NPC.position.Y = NPC.position.Y + num192;

                if (num191 < 0f)
                    NPC.spriteDirection = -1;
                else if (num191 > 0f)
                    NPC.spriteDirection = 1;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
            Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/EidolonWyrmBodyAltGlow").Value.Width, (float)(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/EidolonWyrmBodyAltGlow").Value.Height / Main.npcFrameCount[NPC.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + NPC.gfxOffY);
            Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.LightYellow);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/EidolonWyrmBodyAltGlow").Value, vector,
                new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(NPC.Center, NPC.velocity, Mod.GetGoreSlot("Gores/Wyrm3"), 1f);
            }
        }
    }
}
