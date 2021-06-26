using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    public class SCalWormBodyWeak : ModNPC
    {
		private bool setAlpha = false;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Heart");
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.npcSlots = 5f;
            npc.width = 20;
            npc.height = 20;
            npc.lifeMax = CalamityWorld.revenge ? 345000 : 300000;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = 0.999999f;
            global.unbreakableDR = true;
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
            npc.HitSound = SoundID.NPCHit13;
            npc.DeathSound = SoundID.NPCDeath13;
            npc.netAlways = true;
            npc.dontCountMe = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
			writer.Write(setAlpha);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
			setAlpha = reader.ReadBoolean();
		}

        public override void AI()
        {
			if (npc.ai[2] > 0f)
			{
				npc.realLife = (int)npc.ai[2];
			}

			bool flag = false;
			if (npc.ai[1] <= 0f)
			{
				flag = true;
			}
			else if (Main.npc[(int)npc.ai[1]].life <= 0 || npc.life <= 0)
			{
				flag = true;
			}
			if (flag)
			{
				npc.life = 0;
				npc.HitEffect(0, 10.0);
				npc.checkDead();
			}

			if (Main.npc[(int)npc.ai[1]].alpha < 128 && !setAlpha)
			{
				if (npc.alpha != 0)
				{
					for (int num934 = 0; num934 < 2; num934++)
					{
						int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
						Main.dust[num935].noGravity = true;
						Main.dust[num935].noLight = true;
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
			{
				npc.alpha = Main.npc[(int)npc.ai[2]].alpha;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				npc.localAI[0] += CalamityWorld.malice ? 2f : 1f;
				if (npc.localAI[0] >= 900f)
				{
					npc.localAI[0] = 0f;
					int type = ModContent.ProjectileType<BrimstoneBarrage>();
					int damage = npc.GetProjectileDamage(type);
					int totalProjectiles = 4;
					float radians = MathHelper.TwoPi / totalProjectiles;
					Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
					for (int k = 0; k < totalProjectiles; k++)
					{
						Vector2 velocity = spinningPoint.RotatedBy(radians * k);
						Projectile.NewProjectile(npc.Center, velocity, type, damage, 0f, Main.myPlayer);
					}
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SCalSounds/BrimstoneShoot"), npc.Center);
				}
			}

			Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
			float num191 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2);
			float num192 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2);
			num191 = (int)(num191 / 16f) * 16;
			num192 = (int)(num192 / 16f) * 16;
			vector18.X = (int)(vector18.X / 16f) * 16;
			vector18.Y = (int)(vector18.Y / 16f) * 16;
			num191 -= vector18.X;
			num192 -= vector18.Y;
			float num193 = (float)System.Math.Sqrt(num191 * num191 + num192 * num192);
			if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
			{
				try
				{
					vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
					num191 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
					num192 = Main.npc[(int)npc.ai[1]].position.Y + (Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
				}
				catch
				{
				}
				npc.rotation = (float)System.Math.Atan2(num192, num191) + 1.57f;
				num193 = (float)System.Math.Sqrt(num191 * num191 + num192 * num192);
				int num194 = npc.width;
				num193 = (num193 - num194) / num193;
				num191 *= num193;
				num192 *= num193;
				npc.velocity = Vector2.Zero;
				npc.position.X = npc.position.X + num191;
				npc.position.Y = npc.position.Y + num192;
				if (num191 < 0f)
				{
					npc.spriteDirection = -1;
				}
				else if (num191 > 0f)
				{
					npc.spriteDirection = 1;
				}
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
