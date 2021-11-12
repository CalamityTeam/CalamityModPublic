using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Yharon
{
    public class DetonatingFlare2 : ModNPC
    {
        float randomSpeed = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Detonating Flame");
            Main.npcFrameCount[npc.type] = 5;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.aiStyle = -1;
            aiType = -1;
            npc.GetNPCDamage();
            npc.width = 50;
            npc.height = 50;
            npc.lifeMax = 3900;
            npc.defense = 26;
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
            npc.alpha -= 3;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive || CalamityWorld.malice;
            if (npc.localAI[3] == 0f)
            {
                switch (Main.rand.Next(6))
                {
                    case 0:
                        randomSpeed = 10f;
                        break;
                    case 1:
                        randomSpeed = 11.5f;
                        break;
                    case 2:
                        randomSpeed = 13f;
                        break;
                    case 3:
                        randomSpeed = 14.5f;
                        break;
                    case 4:
                        randomSpeed = 16f;
                        break;
                    case 5:
                        randomSpeed = 17.5f;
                        break;
                }
                npc.localAI[3] = 1f;
            }
            float speed = randomSpeed + (revenge ? 1f : 0f);
            CalamityAI.DungeonSpiritAI(npc, mod, speed, -MathHelper.PiOver2);
        }

		public override bool PreNPCLoot()
		{
			if (!CalamityWorld.malice && !CalamityWorld.revenge)
			{
				int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
				if (Main.rand.Next(8) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}

			return false;
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
                    spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, SpriteEffects.None, 0f);
                }
            }

            Vector2 vector43 = npc.Center - Main.screenPosition;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<LethalLavaBurn>(), 240, true);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }
    }
}
