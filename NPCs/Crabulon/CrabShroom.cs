using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Crabulon
{
	public class CrabShroom : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crab Shroom");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
			npc.GetNPCDamage();
			npc.width = 14;
            npc.height = 14;
            npc.lifeMax = 25;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 15000;
            }
            aiType = -1;
            npc.knockBackResist = 0.75f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
			npc.Calamity().VulnerableToHeat = true;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToSickness = true;
		}

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0f, 0.2f, 0.4f);
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			float xVelocityLimit = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 7.5f : 5f;
            float yVelocityLimit = revenge ? 1.25f : 1f;
            Player player = Main.player[npc.target];
            npc.velocity.Y += 0.02f;
            if (npc.velocity.Y > yVelocityLimit)
            {
                npc.velocity.Y = yVelocityLimit;
            }
            npc.TargetClosest();
            if (npc.position.X + npc.width < player.position.X)
            {
                if (npc.velocity.X < 0f)
                {
                    npc.velocity.X *= 0.98f;
                }
                npc.velocity.X += 0.1f;
            }
            else if (npc.position.X > player.position.X + player.width)
            {
                if (npc.velocity.X > 0f)
                {
                    npc.velocity.X *= 0.98f;
                }
                npc.velocity.X -= 0.1f;
            }
            if (npc.velocity.X > xVelocityLimit || npc.velocity.X < -xVelocityLimit)
            {
                npc.velocity.X *= 0.97f;
            }
            npc.rotation = npc.velocity.X * 0.1f;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Crabulon/CrabShroomGlow");
			Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool PreNPCLoot()
		{
			if (!CalamityWorld.revenge)
			{
				int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
				if (Main.rand.Next(8) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}

			return false;
		}

		public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 56, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 56, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
	}
}
