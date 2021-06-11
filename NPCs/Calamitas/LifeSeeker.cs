using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Calamitas
{
    public class LifeSeeker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Seeker");
            Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
            npc.damage = 30;
            npc.width = 44;
            npc.height = 30;
            npc.defense = 8;
            npc.lifeMax = 200;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 3000;
            }
            npc.aiStyle = 5;
            aiType = NPCID.Probe;
            npc.knockBackResist = BossRushEvent.BossRushActive ? 0f : 0.25f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
        }

		public override bool PreAI()
		{
			// Setting this in SetDefaults will disable expert mode scaling, so put it here instead
			npc.damage = 0;

			return true;
		}

		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(texture.Width / 2), (float)(texture.Height / Main.npcFrameCount[npc.type] / 2));
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
					vector41 -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture = ModContent.GetTexture("CalamityMod/NPCs/Calamitas/LifeSeekerGlow");
			Color color37 = Color.Lerp(Color.White, Color.Red, 0.5f);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 *= (float)(num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/LifeSeeker"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/LifeSeeker2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/LifeSeeker3"), 1f);
            }
        }
    }
}
