using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.NPCs
{
    public class SignusBomb : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Mine");
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.width = 30;
            npc.height = 30;
            npc.lifeMax = 100;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit53;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            npc.rotation = npc.velocity.X * 0.04f;
            if (npc.ai[1] == 0f)
            {
                npc.scale -= 0.02f;
                npc.alpha += 30;
                if (npc.alpha >= 250)
                {
                    npc.alpha = 255;
                    npc.ai[1] = 1f;
                }
            }
            else if (npc.ai[1] == 1f)
            {
                npc.scale += 0.02f;
                npc.alpha -= 30;
                if (npc.alpha <= 0)
                {
                    npc.alpha = 0;
                    npc.ai[1] = 0f;
                }
            }
            Player player = Main.player[npc.target];
            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    if (npc.timeLeft > 10)
                    {
                        npc.timeLeft = 10;
                    }
                    return;
                }
            }
            else if (npc.timeLeft > 600)
            {
                npc.timeLeft = 600;
            }
            Vector2 vector = Main.player[npc.target].Center - npc.Center;
            if (vector.Length() < 90f || npc.ai[3] >= 300f)
            {
                npc.dontTakeDamage = false;
                CheckDead();
                npc.life = 0;
                return;
            }
            npc.ai[3] += 1f;
            npc.dontTakeDamage = npc.ai[3] >= 240f ? false : true;
            if (npc.ai[3] >= 180f)
            {
                npc.velocity.Y *= 0.985f;
                npc.velocity.X *= 0.985f;
                return;
            }
            npc.TargetClosest(true);
            float num1372 = 14f;
            Vector2 vector167 = new Vector2(npc.Center.X + (float)(npc.direction * 20), npc.Center.Y + 6f);
            float num1373 = player.position.X + (float)player.width * 0.5f - vector167.X;
            float num1374 = player.Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
            float num1376 = num1372 / num1375;
            num1373 *= num1376;
            num1374 *= num1376;
            npc.ai[0] -= 1f;
            if (num1375 < 200f || npc.ai[0] > 0f)
            {
                if (num1375 < 200f)
                {
                    npc.ai[0] = 20f;
                }
                if (npc.velocity.X < 0f)
                {
                    npc.direction = -1;
                }
                else
                {
                    npc.direction = 1;
                }
                return;
            }
            npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
            npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;
            if (num1375 < 350f)
            {
                npc.velocity.X = (npc.velocity.X * 10f + num1373) / 11f;
                npc.velocity.Y = (npc.velocity.Y * 10f + num1374) / 11f;
            }
            if (num1375 < 300f)
            {
                npc.velocity.X = (npc.velocity.X * 7f + num1373) / 8f;
                npc.velocity.Y = (npc.velocity.Y * 7f + num1374) / 8f;
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(200, Main.DiscoG, 255, 0);
        }

        public override bool CheckDead()
        {
            Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 14);
            npc.position.X = npc.position.X + (float)(npc.width / 2);
            npc.position.Y = npc.position.Y + (float)(npc.height / 2);
            npc.damage = 300;
            npc.width = npc.height = 256;
            npc.position.X = npc.position.X - (float)(npc.width / 2);
            npc.position.Y = npc.position.Y - (float)(npc.height / 2);
            for (int num621 = 0; num621 < 15; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
                Main.dust[num622].noGravity = true;
            }
            for (int num623 = 0; num623 < 30; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
                Main.dust[num624].noGravity = true;
            }
            return true;
        }
    }
}
