using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Yharon
{
    public class DetonatingFlare2 : ModNPC
    {
        float speed = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Detonating Flame");
            Main.npcFrameCount[npc.type] = 5;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 220;
            npc.width = 50;
            npc.height = 50;
            npc.defense = 75;
            npc.lifeMax = 13000;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit52;
            npc.DeathSound = SoundID.NPCDeath55;
            npc.alpha = 255;
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            npc.alpha -= 3;
            npc.TargetClosest(true);
            Vector2 vector98 = new Vector2(npc.Center.X, npc.Center.Y);
            float num790 = Main.player[npc.target].Center.X - vector98.X;
            float num791 = Main.player[npc.target].Center.Y - vector98.Y;
            float num792 = (float)Math.Sqrt((double)(num790 * num790 + num791 * num791));
            if (npc.localAI[3] == 0f)
            {
                switch (Main.rand.Next(6))
                {
                    case 0:
                        speed = 10f;
                        break;
                    case 1:
                        speed = 11.5f;
                        break;
                    case 2:
                        speed = 13f;
                        break;
                    case 3:
                        speed = 14.5f;
                        break;
                    case 4:
                        speed = 16f;
                        break;
                    case 5:
                        speed = 17.5f;
                        break;
                }
                npc.localAI[3] = 1f;
            }
            float num793 = speed + (revenge ? 1f : 0f);
            num792 = num793 / num792;
            num790 *= num792;
            num791 *= num792;
            npc.velocity.X = (npc.velocity.X * 100f + num790) / 101f;
            npc.velocity.Y = (npc.velocity.Y * 100f + num791) / 101f;
            npc.rotation = (float)Math.Atan2((double)num791, (double)num790) - 1.57f;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, Main.DiscoG, 53, 0);
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
			Color color36 = new Color(255, Main.DiscoG, 53, 0);
			float amount9 = 0.5f;
			int num153 = 10;

			for (int num155 = 1; num155 < num153; num155 += 2)
			{
				Color color38 = lightColor;
				color38 = Color.Lerp(color38, color36, amount9);
				color38 = npc.GetAlpha(color38);
				color38 *= (float)(num153 - num155) / 15f;
				Vector2 vector41 = npc.oldPos[num155] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
				vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
				vector41 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
				spriteBatch.Draw(texture2D15, vector41, new Rectangle?(npc.frame), color38, npc.rotation, vector11, npc.scale, SpriteEffects.None, 0f);
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, new Rectangle?(npc.frame), npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, SpriteEffects.None, 0f);

			return false;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }
    }
}
