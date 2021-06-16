using CalamityMod.Dusts;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    public class SCalWormBody : ModNPC
    {
		private bool setAlpha = false;
        public NPC AheadSegment => Main.npc[(int)npc.ai[1]];
        public NPC HeadSegment => Main.npc[(int)npc.ai[2]];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sepulcher");
        }

        public override void SetDefaults()
        {
            npc.damage = 0; //70
            npc.npcSlots = 5f;
            npc.width = npc.height = 48;
            npc.defense = 0;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = 0.999999f;
            global.unbreakableDR = true;
			npc.lifeMax = CalamityWorld.revenge ? 345000 : 300000;
			npc.aiStyle = -1; //new
            aiType = -1; //new
            npc.knockBackResist = 0f;
            npc.scale = 1.2f;
            if (Main.expertMode)
            {
                npc.scale = 1.35f;
            }
            npc.alpha = 255;
            npc.chaseable = false;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
            npc.dontCountMe = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[3]);
			writer.Write(setAlpha);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[3] = reader.ReadSingle();
			setAlpha = reader.ReadBoolean();
		}

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
			if (npc.ai[2] > 0f)
			{
				npc.realLife = (int)npc.ai[2];
			}

            bool shouldDie = false;
            if (npc.ai[1] <= 0f)
                shouldDie = true;
            else if (AheadSegment.life <= 0 || !AheadSegment.active || npc.life <= 0)
                shouldDie = true;

            if (shouldDie)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.checkDead();
            }

            if (AheadSegment.alpha < 128 && !setAlpha)
            {
                if (npc.alpha != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust fire = Dust.NewDustDirect(npc.position, npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
                        fire.noGravity = true;
                        fire.noLight = true;
                    }
                }
                npc.alpha -= 42;
                if (npc.alpha <= 0)
                {
                    setAlpha = true;
                    npc.alpha = 0;
                }
            }
            else
                npc.alpha = HeadSegment.alpha;

            if (Main.npc.IndexInRange((int)npc.ai[1]))
            {
                Vector2 offsetToAheadSegment = AheadSegment.Center - npc.Center;
                npc.rotation = offsetToAheadSegment.ToRotation() + MathHelper.PiOver2;
                npc.velocity = Vector2.Zero;
                npc.Center = AheadSegment.Center - offsetToAheadSegment.SafeNormalize(Vector2.UnitY) * 52f;
                npc.spriteDirection = (offsetToAheadSegment.X > 0f).ToDirectionInt();
            }
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => projectile.type != x))
			{
				if (projectile.penetrate == -1 && !projectile.minion)
				{
					projectile.penetrate = 1;
				}
				else if (projectile.penetrate >= 1)
				{
					projectile.penetrate = 1;
				}
			}
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = npc.localAI[3] / 2f % 2f == 0f ? ModContent.GetTexture("CalamityMod/NPCs/SupremeCalamitas/SCalWormBodyAlt") : Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / 2));

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 50;
                npc.height = 50;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 5; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
