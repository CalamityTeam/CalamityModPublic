using System;
using Microsoft.Xna.Framework;
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
			npc.width = 38; //324
			npc.height = 38; //216
			npc.defense = 40;
			npc.lifeMax = 20000;
			npc.knockBackResist = 0f;
			npc.aiStyle = -1;
            aiType = -1;
            animationType = 10;
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.netAlways = true;
			npc.dontCountMe = true;
			banner = mod.NPCType("ArmoredDiggerHead");
			bannerItem = mod.ItemType("ArmoredDiggerBanner");
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
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            npc.velocity.Length();
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
            if (Main.netMode != 1)
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
                        int num10 = 450;
                        vector.X += num6 * 5f;
                        vector.Y += num7 * 5f;
                        Projectile.NewProjectile(vector.X, vector.Y, num6, num7, num10, num9, 0f, Main.myPlayer, 0f, 0f);
                        npc.netUpdate = true;
                    }
                }
            }
            int num12 = (int)(npc.position.X / 16f) - 1;
            int num13 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
            int num14 = (int)(npc.position.Y / 16f) - 1;
            int num15 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
            if (num12 < 0)
            {
                num12 = 0;
            }
            if (num13 > Main.maxTilesX)
            {
                num13 = Main.maxTilesX;
            }
            if (num14 < 0)
            {
                num14 = 0;
            }
            if (num15 > Main.maxTilesY)
            {
                num15 = Main.maxTilesY;
            }
            bool flag2 = false;
            if (!flag2)
            {
                for (int k = num12; k < num13; k++)
                {
                    for (int l = num14; l < num15; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].nactive() && (Main.tileSolid[(int)Main.tile[k, l].type] || (Main.tileSolidTop[(int)Main.tile[k, l].type] && Main.tile[k, l].frameY == 0))) || Main.tile[k, l].liquid > 64))
                        {
                            Vector2 vector2;
                            vector2.X = (float)(k * 16);
                            vector2.Y = (float)(l * 16);
                            if (npc.position.X + (float)npc.width > vector2.X && npc.position.X < vector2.X + 16f && npc.position.Y + (float)npc.height > vector2.Y && npc.position.Y < vector2.Y + 16f)
                            {
                                flag2 = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!flag2)
            {
                npc.localAI[1] = 1f;
            }
            else
            {
                npc.localAI[1] = 0f;
            }
            float num17 = 16f;
            if (Main.player[npc.target].dead || (double)Main.player[npc.target].position.Y < Main.rockLayer)
            {
                flag2 = false;
                npc.velocity.Y = npc.velocity.Y + 1f;
                if ((double)npc.position.Y > (double)((Main.maxTilesY - 200) * 16))
                {
                    npc.velocity.Y = npc.velocity.Y + 1f;
                    num17 = 32f;
                }
                if ((double)npc.position.Y > (double)((Main.maxTilesY - 200) * 16))
                {
                    for (int a = 0; a < 200; a++)
                    {
                        if (Main.npc[a].type == mod.NPCType("ArmoredDiggerHead") || Main.npc[a].type == mod.NPCType("ArmoredDiggerBody") ||
                            Main.npc[a].type == mod.NPCType("ArmoredDiggerTail"))
                        {
                            Main.npc[a].active = false;
                        }
                    }
                }
            }
            float num18 = 0.1f;
            float num19 = 0.15f;
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
                }
                catch
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
                return;
            }
            if (!flag2)
            {
                npc.TargetClosest(true);
                npc.velocity.Y = npc.velocity.Y + 0.15f;
                if (npc.velocity.Y > num17)
                {
                    npc.velocity.Y = num17;
                }
                if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num17 * 0.4)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X - num18 * 1.1f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X + num18 * 1.1f;
                    }
                }
                else if (npc.velocity.Y == num17)
                {
                    if (npc.velocity.X < num20)
                    {
                        npc.velocity.X = npc.velocity.X + num18;
                    }
                    else if (npc.velocity.X > num20)
                    {
                        npc.velocity.X = npc.velocity.X - num18;
                    }
                }
                else if (npc.velocity.Y > 4f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X + num18 * 0.9f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X - num18 * 0.9f;
                    }
                }
            }
            else
            {
                if (npc.soundDelay == 0)
                {
                    float num24 = num22 / 40f;
                    if (num24 < 10f)
                    {
                        num24 = 10f;
                    }
                    if (num24 > 20f)
                    {
                        num24 = 20f;
                    }
                    npc.soundDelay = (int)num24;
                    Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);
                }
                num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
                float num25 = Math.Abs(num20);
                float num26 = Math.Abs(num21);
                float num27 = num17 / num22;
                num20 *= num27;
                num21 *= num27;
                if (((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f)) && ((npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f)))
                {
                    if (npc.velocity.X < num20)
                    {
                        npc.velocity.X = npc.velocity.X + num19;
                    }
                    else if (npc.velocity.X > num20)
                    {
                        npc.velocity.X = npc.velocity.X - num19;
                    }
                    if (npc.velocity.Y < num21)
                    {
                        npc.velocity.Y = npc.velocity.Y + num19;
                    }
                    else if (npc.velocity.Y > num21)
                    {
                        npc.velocity.Y = npc.velocity.Y - num19;
                    }
                }
                if ((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f) || (npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f))
                {
                    if (npc.velocity.X < num20)
                    {
                        npc.velocity.X = npc.velocity.X + num18;
                    }
                    else if (npc.velocity.X > num20)
                    {
                        npc.velocity.X = npc.velocity.X - num18;
                    }
                    if (npc.velocity.Y < num21)
                    {
                        npc.velocity.Y = npc.velocity.Y + num18;
                    }
                    else if (npc.velocity.Y > num21)
                    {
                        npc.velocity.Y = npc.velocity.Y - num18;
                    }
                    if ((double)Math.Abs(num21) < (double)num17 * 0.2 && ((npc.velocity.X > 0f && num20 < 0f) || (npc.velocity.X < 0f && num20 > 0f)))
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num18 * 2f;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y - num18 * 2f;
                        }
                    }
                    if ((double)Math.Abs(num20) < (double)num17 * 0.2 && ((npc.velocity.Y > 0f && num21 < 0f) || (npc.velocity.Y < 0f && num21 > 0f)))
                    {
                        if (npc.velocity.X > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num18 * 2f;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X - num18 * 2f;
                        }
                    }
                }
                else if (num25 > num26)
                {
                    if (npc.velocity.X < num20)
                    {
                        npc.velocity.X = npc.velocity.X + num18 * 1.1f;
                    }
                    else if (npc.velocity.X > num20)
                    {
                        npc.velocity.X = npc.velocity.X - num18 * 1.1f;
                    }
                    if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num17 * 0.5)
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num18;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y - num18;
                        }
                    }
                }
                else
                {
                    if (npc.velocity.Y < num21)
                    {
                        npc.velocity.Y = npc.velocity.Y + num18 * 1.1f;
                    }
                    else if (npc.velocity.Y > num21)
                    {
                        npc.velocity.Y = npc.velocity.Y - num18 * 1.1f;
                    }
                    if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num17 * 0.5)
                    {
                        if (npc.velocity.X > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num18;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X - num18;
                        }
                    }
                }
            }
            npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default(Color), 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default(Color), 1f);
                }
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Chilled, 180, true);
            player.AddBuff(BuffID.Electrified, 120, true);
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
