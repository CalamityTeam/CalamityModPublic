using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.Umbrella
{
    public class MagicHammer : ModProjectile
    {
        public float Behavior = 0f;
		public float PivotPointX = 0f;
		public float PivotPointY = 0f;
		private int counter = 0;
		private const int projSize = 60;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hammer");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = projSize;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Behavior);
            writer.Write(PivotPointX);
            writer.Write(PivotPointY);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Behavior = reader.ReadSingle();
            PivotPointX = reader.ReadSingle();
            PivotPointY = reader.ReadSingle();
        }

        public override void AI()
        {
			Player player = Main.player[Projectile.owner];
            Projectile.alpha -= 50;
			if (player.Calamity().magicHat)
			{
				Projectile.timeLeft = 2;
			}
			Projectile.ai[0] -= MathHelper.ToRadians(4f);

            float homingRange = MagicHat.Range;
            int targetIndex = -1;
            //If targeting something, prioritize that enemy
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    //Calculate distance between target and the projectile to know if it's too far or not
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (targetIndex == -1 && targetDist < (homingRange + extraDist))
                    {
                        homingRange = targetDist;
                        targetIndex = npc.whoAmI;
                    }
                }
            }
            if (targetIndex == -1)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        //Calculate distance between target and the projectile to know if it's too far or not
                        float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                        if (targetIndex == -1 && targetDist < (homingRange + extraDist))
                        {
                            homingRange = targetDist;
                            targetIndex = npc.whoAmI;
                        }
                    }
                }
            }

			if (targetIndex == -1)
			{
				Behavior = 0f;
				IdleAI();
			}
			else
			{
				MoveToEnemy(targetIndex);
			}
        }

		private void IdleAI()
		{
			Player player = Main.player[Projectile.owner];

			const float outwardPosition = 180f;
			Vector2 returnPos = player.Center + Projectile.ai[0].ToRotationVector2() * outwardPosition;

			// Player distance calculations
			Vector2 playerVec = returnPos - Projectile.Center;
			float playerDist = playerVec.Length();

			float playerHomeSpeed = 40f;
			// Teleport to the player if abnormally far
			if (playerDist > 2000f)
			{
				Projectile.Center = returnPos;
				Projectile.netUpdate = true;
			}
			// If more than 60 pixels away, move toward the player
			if (playerDist > 60f)
			{
				playerVec.Normalize();
				playerVec *= playerHomeSpeed;
				Projectile.velocity = (Projectile.velocity * 10f + playerVec) / 11f;
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
			}
			else
			{
				Projectile.Center = returnPos;
				Projectile.rotation = Projectile.ai[0] + MathHelper.PiOver4;
			}
			if (Projectile.scale != 1f)
			{
				Projectile.scale = 1f;
				Projectile.ExpandHitboxBy((int)(projSize * Projectile.scale));
			}
		}

		private void MoveToEnemy(int targetIndex)
		{
			// Target distance calculations
			NPC npc = Main.npc[targetIndex];
			Vector2 targetVec = npc.Center - Projectile.Center;
			float targetDist = targetVec.Length();

			float moveSpeed = 40f;
			// If more than 60 pixels away, move toward the target
			if (targetDist > 60f && Behavior == 0f)
			{
				targetVec.Normalize();
				targetVec *= moveSpeed;
				Projectile.velocity = (Projectile.velocity * 1f + targetVec) / 2f;
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
			}
			else
			{
				if (Behavior == 0f)
				{
					if (Projectile.scale == 1f)
					{
						Projectile.scale = 3f;
						Projectile.ExpandHitboxBy(Projectile.scale);
					}
					Projectile.rotation = MathHelper.PiOver4 + MathHelper.ToRadians(20f) * (targetVec.X > 0 ? 1f : -6f);
					Projectile.ai[1] = MathHelper.PiOver4 + MathHelper.ToRadians(20f) * (targetVec.X > 0 ? 5f : -1f);
					PivotPointX = Projectile.Bottom.X;
					PivotPointY = Projectile.Bottom.Y;
					Behavior = 1f;
				}
				if (Behavior == 1f)
				{
					Behavior = targetVec.X > 0 ? 2f : 3f;
				}
				if (Behavior == 2f || Behavior == 3f)
				{
					float swingTime = 20f;
					Projectile.ai[1] += MathHelper.ToRadians(200f / swingTime) * (Behavior == 2f ? 1f : -1f);
					counter++;
					float outwardPosition = 50f;
					Vector2 pivot = new Vector2(PivotPointX, PivotPointY);
					Projectile.Center = pivot + Projectile.ai[1].ToRotationVector2() * outwardPosition;
					Projectile.rotation = Projectile.ai[1] + MathHelper.PiOver4;
					if (counter > swingTime)
					{
						Projectile.ai[1] = MathHelper.PiOver4;
						counter = 0;
						Behavior = 4f;
					}
				}
				if (Behavior == 4f)
					MoveBackToPlayer();
			}
		}

		private void MoveBackToPlayer()
		{
			Player player = Main.player[Projectile.owner];

			const float outwardPosition = 180f;
			Vector2 returnPos = player.Center + Projectile.ai[0].ToRotationVector2() * outwardPosition;

			// Player distance calculations
			Vector2 playerVec = returnPos - Projectile.Center;
			float playerDist = playerVec.Length();

			float playerHomeSpeed = 40f;
			// If more than 60 pixels away, move toward the player
			if (playerDist > 60f)
			{
				playerVec.Normalize();
				playerVec *= playerHomeSpeed;
				Projectile.velocity = (Projectile.velocity * 1f + playerVec) / 2f;
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
			}
			else
			{
				Behavior = 0f;
			}
			if (Projectile.scale != 1f)
			{
				Projectile.scale = 1f;
				Projectile.ExpandHitboxBy(projSize);
			}
		}

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 56, 0, Projectile.alpha);

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(Projectile.Center, 1, 1, 67, dspeed.X, dspeed.Y, 50, default, 1.2f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
