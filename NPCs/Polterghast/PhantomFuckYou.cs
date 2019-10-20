using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Boss;

namespace CalamityMod.NPCs
{
    public class PhantomFuckYou : ModNPC
    {
        private bool start = true;
        private int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.width = 30;
            npc.height = 30;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.damage = 0;
            npc.lifeMax = 1500;
            npc.dontTakeDamage = true;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(start);
            writer.Write(timer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            start = reader.ReadBoolean();
            timer = reader.ReadInt32();
        }

        public override bool PreAI()
        {
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;

            if (start)
            {
                start = false;

                for (int num621 = 0; num621 < 5; num621++)
                    Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 2f);

                npc.ai[1] = npc.ai[0];
            }

            if (CalamityGlobalNPC.ghostBoss < 0 || !Main.npc[CalamityGlobalNPC.ghostBoss].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return false;
            }

            npc.TargetClosest(true);

            Vector2 direction = Main.player[npc.target].Center - npc.Center;
            direction.Normalize();
            direction *= 9f;
            npc.rotation = direction.ToRotation();

            timer++;
            if (CalamityWorld.death || CalamityWorld.bossRushActive)
                timer++;

            if (timer >= 150)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = expertMode ? 62 : 75;
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, direction.X * 0.05f, direction.Y * 0.05f, ModContent.ProjectileType<PhantomMine>(), damage, 1f, npc.target);
                }

                timer = 0;
            }

            Player player = Main.player[npc.target];
            double deg = (double)npc.ai[1];
            double rad = deg * (Math.PI / 180);
            double dist = 500;
            npc.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - npc.width / 2;
            npc.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - npc.height / 2;
            npc.ai[1] += 0.5f; //1f
            return false;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(200, 200, 200, 0);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.velocity != Vector2.Zero)
            {
                Texture2D texture = Main.npcTexture[npc.type];
                Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                for (int i = 1; i < npc.oldPos.Length; ++i)
                {
                    Vector2 vector2_2 = npc.oldPos[i];
                    Color color2 = Color.White * npc.Opacity;
                    color2.R = (byte)(0.5 * (double)color2.R * (double)(10 - i) / 20.0);
                    color2.G = (byte)(0.5 * (double)color2.G * (double)(10 - i) / 20.0);
                    color2.B = (byte)(0.5 * (double)color2.B * (double)(10 - i) / 20.0);
                    color2.A = (byte)(0.5 * (double)color2.A * (double)(10 - i) / 20.0);
                    Main.spriteBatch.Draw(Main.npcTexture[npc.type], new Vector2(npc.oldPos[i].X - Main.screenPosition.X + (npc.width / 2),
                        npc.oldPos[i].Y - Main.screenPosition.Y + npc.height / 2), new Rectangle?(npc.frame), color2, npc.oldRot[i], origin, npc.scale, SpriteEffects.None, 0.0f);
                }
            }
            return true;
        }
    }
}
