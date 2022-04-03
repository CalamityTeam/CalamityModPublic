using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Signus
{
    public class SignusBomb : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Mine");
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.width = 30;
            NPC.height = 30;
            NPC.lifeMax = 4800;
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
                NPC.active = false;
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

            float num1372 = death ? 16f : 14f;
            Vector2 vector167 = new Vector2(NPC.Center.X + (NPC.direction * 20), NPC.Center.Y + 6f);
            float num1373 = player.position.X + player.width * 0.5f - vector167.X;
            float num1374 = player.Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt(num1373 * num1373 + num1374 * num1374);
            float num1376 = num1372 / num1375;
            num1373 *= num1376;
            num1374 *= num1376;
            NPC.ai[0] -= 1f;
            if (num1375 < 200f || NPC.ai[0] > 0f)
            {
                if (num1375 < 200f)
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

            NPC.velocity.X = (NPC.velocity.X * 50f + num1373) / 51f;
            NPC.velocity.Y = (NPC.velocity.Y * 50f + num1374) / 51f;
            if (num1375 < 350f)
            {
                NPC.velocity.X = (NPC.velocity.X * 10f + num1373) / 11f;
                NPC.velocity.Y = (NPC.velocity.Y * 10f + num1374) / 11f;
            }
            if (num1375 < 300f)
            {
                NPC.velocity.X = (NPC.velocity.X * 7f + num1373) / 8f;
                NPC.velocity.Y = (NPC.velocity.Y * 7f + num1374) / 8f;
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(200, Main.DiscoG, 255, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (NPC.Calamity().newAI[0] >= 45f)
                return false;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = Main.npcTexture[NPC.type];
            Vector2 vector11 = new Vector2(Main.npcTexture[NPC.type].Width / 2, Main.npcTexture[NPC.type].Height / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = lightColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - Main.screenPosition;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool PreNPCLoot() => false;

        public override bool CheckDead()
        {
            SoundEngine.PlaySound(SoundID.Item, (int)NPC.position.X, (int)NPC.position.Y, 14);
            NPC.position.X = NPC.position.X + (NPC.width / 2);
            NPC.position.Y = NPC.position.Y + (NPC.height / 2);
            NPC.damage = NPC.defDamage;
            NPC.width = NPC.height = 256;
            NPC.position.X = NPC.position.X - (NPC.width / 2);
            NPC.position.Y = NPC.position.Y - (NPC.height / 2);
            for (int num621 = 0; num621 < 10; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
                Main.dust[num622].noGravity = true;
            }
            for (int num623 = 0; num623 < 20; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
                Main.dust[num624].noGravity = true;
            }
            return true;
        }
    }
}
