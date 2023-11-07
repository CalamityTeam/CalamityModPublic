using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Signus
{
    public class CosmicMine : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.width = 30;
            NPC.height = 30;
            NPC.lifeMax = 4800;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit53;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.signus < 0 || !Main.npc[CalamityGlobalNPC.signus].active)
            {
                NPC.life = 0;
                NPC.checkDead();
                NPC.netUpdate = true;
                return;
            }

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.7f, 0.2f, 1.1f);

            NPC.rotation = NPC.velocity.X * 0.04f;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            Vector2 vector = player.Center - NPC.Center;
            NPC.ai[2] = vector.Length();

            NPC.damage = 0;
            NPC.ai[3] += 1f;
            if (NPC.ai[2] < 90f || NPC.ai[3] >= 300f || NPC.Calamity().newAI[0] > 0f)
            {
                NPC.Calamity().newAI[0] += 1f;
                NPC.velocity *= 0.98f;
                NPC.dontTakeDamage = false;
                NPC.scale = MathHelper.Lerp(1f, 3f, NPC.Calamity().newAI[0] / 45f);

                if (NPC.Calamity().newAI[0] >= 45f)
                {
                    CheckDead();
                    NPC.life = 0;
                }

                return;
            }

            if (NPC.ai[1] == 0f)
            {
                NPC.scale -= 0.02f;
                NPC.alpha += 30;
                if (NPC.alpha >= 250)
                {
                    NPC.alpha = 255;
                    NPC.ai[1] = 1f;
                }
            }
            else if (NPC.ai[1] == 1f)
            {
                NPC.scale += 0.02f;
                NPC.alpha -= 30;
                if (NPC.alpha <= 0)
                {
                    NPC.alpha = 0;
                    NPC.ai[1] = 0f;
                }
            }

            float mineSpeed = death ? 16f : 14f;
            Vector2 vector167 = new Vector2(NPC.Center.X + (NPC.direction * 20), NPC.Center.Y + 6f);
            float playerXDist = player.position.X + player.width * 0.5f - vector167.X;
            float playerYDist = player.Center.Y - vector167.Y;
            float playerDistance = (float)Math.Sqrt(playerXDist * playerXDist + playerYDist * playerYDist);
            float velocityMult = mineSpeed / playerDistance;
            playerXDist *= velocityMult;
            playerYDist *= velocityMult;
            NPC.ai[0] -= 1f;
            if (playerDistance < 200f || NPC.ai[0] > 0f)
            {
                if (playerDistance < 200f)
                {
                    NPC.ai[0] = 20f;
                }
                if (NPC.velocity.X < 0f)
                {
                    NPC.direction = -1;
                }
                else
                {
                    NPC.direction = 1;
                }
                return;
            }

            NPC.velocity.X = (NPC.velocity.X * 50f + playerXDist) / 51f;
            NPC.velocity.Y = (NPC.velocity.Y * 50f + playerYDist) / 51f;
            if (playerDistance < 350f)
            {
                NPC.velocity.X = (NPC.velocity.X * 10f + playerXDist) / 11f;
                NPC.velocity.Y = (NPC.velocity.Y * 10f + playerYDist) / 11f;
            }
            if (playerDistance < 300f)
            {
                NPC.velocity.X = (NPC.velocity.X * 7f + playerXDist) / 8f;
                NPC.velocity.Y = (NPC.velocity.Y * 7f + playerYDist) / 8f;
            }
        }

        public override Color? GetAlpha(Color drawColor) => new Color(200, Main.DiscoG, 255, 0);

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.Calamity().newAI[0] >= 45f)
                return false;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / 2);
            int afterimageAmt = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckDead()
        {
            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
            NPC.position.X = NPC.position.X + (NPC.width / 2);
            NPC.position.Y = NPC.position.Y + (NPC.height / 2);
            NPC.damage = NPC.defDamage;
            NPC.width = NPC.height = 256;
            NPC.position.X = NPC.position.X - (NPC.width / 2);
            NPC.position.Y = NPC.position.Y - (NPC.height / 2);
            for (int i = 0; i < 10; i++)
            {
                int cosmiliteDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                Main.dust[cosmiliteDust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[cosmiliteDust].scale = 0.5f;
                    Main.dust[cosmiliteDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
                Main.dust[cosmiliteDust].noGravity = true;
            }
            for (int j = 0; j < 20; j++)
            {
                int cosmiliteDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                Main.dust[cosmiliteDust2].noGravity = true;
                Main.dust[cosmiliteDust2].velocity *= 5f;
                cosmiliteDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                Main.dust[cosmiliteDust2].velocity *= 2f;
                Main.dust[cosmiliteDust2].noGravity = true;
            }
            return true;
        }
    }
}
