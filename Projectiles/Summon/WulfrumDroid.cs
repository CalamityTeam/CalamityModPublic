using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class WulfrumDroid : ModProjectile
    {
        public float dust = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Droid");
            Main.projFrames[projectile.type] = 12;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 32;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (dust == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 36;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    Vector2 source = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 dustVel = source - projectile.Center;
                    int green = Dust.NewDust(source + dustVel, 0, 0, 61, dustVel.X * 1.1f, dustVel.Y * 1.1f, 100, default, 1.4f);
                    Main.dust[green].noGravity = true;
                    Main.dust[green].noLight = true;
                    Main.dust[green].velocity = dustVel;
                }
                dust += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = damage2;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            bool correctMinion = projectile.type == ModContent.ProjectileType<WulfrumDroid>();
            player.AddBuff(ModContent.BuffType<WulfrumDroidBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.wDroid = false;
                }
                if (modPlayer.wDroid)
                {
                    projectile.timeLeft = 2;
                }
            }
			projectile.MinionAntiClump();
			Vector2 targetVector = projectile.position;
			float range = 450f;
			bool foundTarget = false;
			int targetIndex = -1;
			if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
				if (npc.CanBeChasedBy((object) this, false))
				{
					float targetDist = Vector2.Distance(npc.Center, projectile.Center);
					if ((!foundTarget && targetDist < range) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
					{
						range = targetDist;
						targetVector = npc.Center;
						foundTarget = true;
						targetIndex = npc.whoAmI;
					}
				}
			}
			if (!foundTarget)
			{
				for (int npcIndex = 0; npcIndex < Main.npc.Length; ++npcIndex)
				{
					NPC npc = Main.npc[npcIndex];
					if (npc.CanBeChasedBy((object) this, false))
					{
						float targetDist = Vector2.Distance(npc.Center, projectile.Center);
						if ((!foundTarget && targetDist < range) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
						{
							range = targetDist;
							targetVector = npc.Center;
							foundTarget = true;
							targetIndex = npcIndex;
						}
					}
				}
			}
			float separationAnxietyDist = 500f;
			if (foundTarget)
				separationAnxietyDist = 1000f;
			if (Vector2.Distance(player.Center, projectile.Center) > separationAnxietyDist)
			{
				projectile.ai[0] = 1f;
				projectile.netUpdate = true;
			}
			if (projectile.ai[0] == 1f)
				projectile.tileCollide = false;
			if (foundTarget && projectile.ai[0] == 0f)
			{
				Vector2 targetPos = targetVector - projectile.Center;
				float targetDist = targetPos.Length();
				targetPos.Normalize();
				if (targetDist > 200f)
				{
					float speed = 6f;
					Vector2 goToTarget = targetPos * speed;
					projectile.velocity.X = (projectile.velocity.X * 40f + goToTarget.X) / 41f;
					projectile.velocity.Y = (projectile.velocity.Y * 40f + goToTarget.Y) / 41f;
				}
				else if (projectile.velocity.Y > -1f)
					projectile.velocity.Y -= 0.1f;
			}
			else
			{
				if (!Collision.CanHitLine(projectile.Center, 1, 1, player.Center, 1, 1))
					projectile.ai[0] = 1f;
				float returnSpeed = 6f;
				if (projectile.ai[0] == 1f)
					returnSpeed = 15f;
				Vector2 playerVec = player.Center - projectile.Center + new Vector2(0f, -60f);
				float playerDist = playerVec.Length();
				if (playerDist > 200f && returnSpeed < 9f)
					returnSpeed = 9f;
				if (playerDist < 100f && projectile.ai[0] == 1f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
				{
					projectile.ai[0] = 0f;
					projectile.netUpdate = true;
				}
				if (playerDist > 2000f)
				{
					projectile.position.X = player.Center.X - (float) (projectile.width / 2);
					projectile.position.Y = player.Center.Y - (float) (projectile.width / 2);
					projectile.netUpdate = true;
				}
				else if (playerDist > 70f)
				{
					playerVec.Normalize();
					projectile.velocity = (projectile.velocity * 20f + playerVec * returnSpeed) / 21f;
				}
				else
				{
					if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
					{
						projectile.velocity.X = -0.15f;
						projectile.velocity.Y = -0.05f;
					}
					projectile.velocity *= 1.01f;
				}
			}
			projectile.rotation = projectile.velocity.X * 0.05f;
			if (projectile.velocity.X > 0f)
				projectile.spriteDirection = projectile.direction = 1;
			else if (projectile.velocity.X < 0f)
				projectile.spriteDirection = projectile.direction = -1;
			if (projectile.ai[1] > 0f)
				projectile.ai[1] += (float) Main.rand.Next(1, 4);
			if (projectile.ai[1] > 90f)
			{
				projectile.ai[1] = 0.0f;
				projectile.netUpdate = true;
			}
			if (projectile.ai[1] != 0f)
				return;
			++projectile.ai[1];
			if (Main.myPlayer != projectile.owner)
				return;
			if (!foundTarget)
				return;
			Vector2 velocity = targetVector - projectile.Center;
			velocity.Normalize();
			velocity *= 10f;
			int bolt = Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<WulfrumBoltMinion>(), projectile.damage, projectile.knockBack, projectile.owner);
			Main.projectile[bolt].netUpdate = true;
			projectile.netUpdate = true;
        }

        public override bool CanDamage() => false;
    }
}
