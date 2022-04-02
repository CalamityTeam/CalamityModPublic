using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class ArmoredDiggerBody : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Armored Digger");
        }

        public override void SetDefaults()
        {
            npc.damage = 70;
            npc.width = 38;
            npc.height = 38;
            npc.defense = 20;
            npc.DR_NERD(0.2f);
            npc.lifeMax = 20000;
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
            npc.dontCountMe = true;
            banner = ModContent.NPCType<ArmoredDiggerHead>();
            bannerItem = ModContent.ItemType<ArmoredDiggerBanner>();
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (npc.ai[3] > 0f)
            {
                npc.realLife = (int)npc.ai[3];
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest(true);
            }
            bool flag = false;
            if (npc.ai[1] <= 0f)
            {
                flag = true;
            }
            else if (Main.npc[(int)npc.ai[1]].life <= 0)
            {
                flag = true;
            }
            if (flag)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.checkDead();
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] += (float)Main.rand.Next(4);
                if (npc.localAI[0] >= (float)Main.rand.Next(1800, 26000))
                {
                    npc.localAI[0] = 0f;
                    npc.TargetClosest(true);
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position,
                        Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float speed = 7f;
                        Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                        float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X + (float)Main.rand.Next(-20, 21);
                        float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-20, 21);
                        float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                        num8 = speed / num8;
                        num6 *= num8;
                        num7 *= num8;
                        int num9 = 30;
                        int num10 = ProjectileID.SaucerScrap;
                        vector.X += num6 * 5f;
                        vector.Y += num7 * 5f;
                        Projectile.NewProjectile(vector.X, vector.Y, num6, num7, num10, num9, 0f, Main.myPlayer, 0f, 0f);
                        npc.netUpdate = true;
                    }
                }
            }
            Vector2 vector3 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num20 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num21 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
            num20 = (float)((int)(num20 / 16f) * 16);
            num21 = (float)((int)(num21 / 16f) * 16);
            vector3.X = (float)((int)(vector3.X / 16f) * 16);
            vector3.Y = (float)((int)(vector3.Y / 16f) * 16);
            num20 -= vector3.X;
            num21 -= vector3.Y;
            float num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
            if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    vector3 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    num20 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector3.X;
                    num21 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector3.Y;
                } catch
                {
                }
                npc.rotation = (float)Math.Atan2((double)num21, (double)num20) + 1.57f;
                num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
                int num23 = (int)(44f * npc.scale);
                num22 = (num22 - (float)num23) / num22;
                num20 *= num22;
                num21 *= num22;
                npc.velocity = Vector2.Zero;
                npc.position.X = npc.position.X + num20;
                npc.position.Y = npc.position.Y + num21;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }
    }
}
