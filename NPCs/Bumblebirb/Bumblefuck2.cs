using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Bumblebirb
{
    public class Bumblefuck2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draconic Swarmer");
            Main.npcFrameCount[npc.type] = 5;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override string Texture => "CalamityMod/NPCs/Bumblebirb/BumbleFolly";

        public override void SetDefaults()
        {
            npc.npcSlots = 1f;
            npc.aiStyle = -1;
            aiType = -1;
			npc.GetNPCDamage();
			npc.width = 120;
            npc.height = 80;
            npc.defense = 20;
            npc.LifeMaxNERB(9000, 11250, 5000); // Old HP - 12000, 15000
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
			npc.noTileCollide = true;
            npc.noGravity = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit51;
            npc.DeathSound = SoundID.NPCDeath46;
        }

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || !NPC.downedMoonlord || spawnInfo.player.Calamity().ZoneSunkenSea || NPC.AnyNPCs(npc.type))
			{
				return 0f;
			}
			return SpawnCondition.SurfaceJungle.Chance * 0.14f;
		}

		public override void AI()
        {
			npc.damage = CalamityPlayer.areThereAnyDamnBosses ? npc.defDamage : (int)(npc.defDamage * 0.8);

            Player player = Main.player[npc.target];
            Vector2 vector = npc.Center;

			bool increasedAggression = CalamityPlayer.areThereAnyDamnBosses;

			float rotationMult = 4f;
			float rotationAmt = 0.04f;

			if (Vector2.Distance(player.Center, vector) > 5600f)
            {
                if (npc.timeLeft > 5)
                {
                    npc.timeLeft = 5;
                }
            }

            npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt * 1.25f) / 10f;

            if (npc.ai[0] == 0f || npc.ai[0] == 1f)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type)
                    {
                        Vector2 value42 = Main.npc[i].Center - npc.Center;
                        if (value42.Length() < (npc.width + npc.height))
                        {
                            value42.Normalize();
                            value42 *= -0.1f;
                            npc.velocity += value42;
                            NPC nPC6 = Main.npc[i];
                            nPC6.velocity -= value42;
                        }
                    }
                }
            }

            if (npc.target < 0 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest(true);
                Vector2 vector240 = Main.player[npc.target].Center - npc.Center;
                if (Main.player[npc.target].dead || vector240.Length() > (increasedAggression ? 5600f : 2800f))
                {
                    npc.ai[0] = -1f;
                }
            }
            else
            {
                Vector2 vector241 = Main.player[npc.target].Center - npc.Center;
                if (npc.ai[0] > 1f && vector241.Length() > 3600f)
                {
                    npc.ai[0] = 1f;
                }
            }

            if (npc.ai[0] == -1f)
            {
                Vector2 value43 = new Vector2(0f, -8f);
                npc.velocity = (npc.velocity * 21f + value43) / 10f;
                npc.dontTakeDamage = true;
                return;
            }

            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(true);
                npc.spriteDirection = npc.direction;
                Vector2 value44 = Main.player[npc.target].Center - npc.Center;
                if (value44.Length() > 2800f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                }
                else if (value44.Length() > 400f)
                {
                    float scaleFactor20 = (increasedAggression ? 9f : 7f) + value44.Length() / 100f + npc.ai[1] / 15f;
                    float num1377 = 30f;
                    value44.Normalize();
                    value44 *= scaleFactor20;
                    npc.velocity = (npc.velocity * (num1377 - 1f) + value44) / num1377;
                }
                else if (npc.velocity.Length() > 2f)
                {
                    npc.velocity *= 0.95f;
                }
                else if (npc.velocity.Length() < 1f)
                {
                    npc.velocity *= 1.05f;
                }
                npc.ai[1] += 1f;
                if (npc.ai[1] >= (increasedAggression ? 90f : 105f))
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 2f;
                }
            }
            else
            {
                if (npc.ai[0] == 1f)
                {
                    if (npc.target < 0 || !Main.player[npc.target].active || Main.player[npc.target].dead)
                    {
                        npc.TargetClosest(true);
                    }
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;
                    Vector2 value45 = Main.player[npc.target].Center - npc.Center;
                    if (value45.Length() < 800f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }
                    npc.ai[2] += 0.0166666675f;
                    float scaleFactor21 = (increasedAggression ? 12f : 9f) + npc.ai[2] + value45.Length() / 150f;
                    float num1378 = 25f;
                    value45.Normalize();
                    value45 *= scaleFactor21;
                    npc.velocity = (npc.velocity * (num1378 - 1f) + value45) / num1378;
                    npc.netSpam = 5;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    }
                    return;
                }
                if (npc.ai[0] == 2f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.rotation = (npc.rotation * rotationMult * 0.75f + npc.velocity.X * rotationAmt * 1.25f) / 8f;
                    Vector2 vector242 = Main.player[npc.target].Center - npc.Center;
                    vector242.Y -= 8f;
                    float scaleFactor22 = increasedAggression ? 18f : 14f;
                    float num1379 = 8f;
                    vector242.Normalize();
                    vector242 *= scaleFactor22;
                    npc.velocity = (npc.velocity * (num1379 - 1f) + vector242) / num1379;
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 10f)
                    {
                        npc.velocity = vector242;
                        if (npc.velocity.X < 0f)
                        {
                            npc.direction = -1;
                        }
                        else
                        {
                            npc.direction = 1;
                        }
                        npc.ai[0] = 2.1f;
                        npc.ai[1] = 0f;
                    }
                }
                else if (npc.ai[0] == 2.1f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.velocity *= 1.01f;
                    npc.ai[1] += 1f;
                    int num1380 = 30;
                    if (npc.ai[1] > num1380)
                    {
                        if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                        {
                            npc.ai[0] = 0f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            return;
                        }
                        if (npc.ai[1] > (num1380 * 2))
                        {
                            npc.ai[0] = 1f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }
                    }
                }
            }
        }

		public override bool PreNPCLoot()
		{
			if (CalamityPlayer.areThereAnyDamnBosses)
				return false;

			if (!CalamityWorld.malice && !CalamityWorld.revenge)
			{
				int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
				if (Main.rand.Next(4) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}

			DropHelper.DropItemSpray(npc, ModContent.ItemType<EffulgentFeather>(), 2, 4);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
			npc.frameCounter += npc.ai[0] == 2.1f ? 1.5 : 1D;
			if (npc.frameCounter > 4D) //iban said the time between frames was 5 so using that as a base
			{
				npc.frameCounter = 0D;
				npc.frame.Y += frameHeight;
			}
			if (npc.frame.Y >= frameHeight * 4)
			{
				npc.frame.Y = 0;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
			Color color36 = Color.Gold;
			float amount9 = 0.5f;
			int num153 = npc.ai[0] == 2.1f ? 7 : 0;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}

		public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
