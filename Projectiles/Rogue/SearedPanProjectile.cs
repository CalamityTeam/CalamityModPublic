using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.NPCs.NormalNPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class SearedPanProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pan");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 20;
            projectile.friendly = true;
			projectile.ignoreWater = true;
            projectile.timeLeft = 420;
            projectile.Calamity().rogue = true;
			projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 5, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 180);
			// Don't increment the Seared Pan counter when hitting dummies
            bool dummy = target.type != NPCID.TargetDummy && target.type != ModContent.NPCType<SuperDummyNPC>();
			OnHitEffects(target.whoAmI, target.life, dummy);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			Player player = Main.player[projectile.owner];
            target.AddBuff(BuffID.Bleeding, 300);
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 180);
			OnHitEffects(-1, target.statLife, true);
        }

		private void OnHitEffects(int targetIndex, int health, bool specialEffects)
		{
			Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.Calamity();

			// Don't spawn fireballs or increment the special effects counter if you can't even stealth strike
			bool playerCanStealthStrike = modPlayer.wearingRogueArmor && modPlayer.rogueStealthMax > 0;
			if (!playerCanStealthStrike)
				return;

			// Increment the seared pan counter and refill stealth after three consecutive hits
			// See CalamityPlayerMiscEffects.cs for code that resets the counter after 40 frames
			modPlayer.searedPanTimer = 0;
			if (!projectile.Calamity().stealthStrike && specialEffects)
				modPlayer.searedPanCounter++;
			if (modPlayer.searedPanCounter >= 3 && !projectile.Calamity().stealthStrike && specialEffects)
			{
				modPlayer.searedPanCounter = 0;
				modPlayer.rogueStealth = modPlayer.rogueStealthMax;
			}

			// Stealth strikes spawn six golden sparks on hit
			if (projectile.Calamity().stealthStrike)
			{
				modPlayer.searedPanCounter = 0;
				for (int t = 0; t < 6; t++)
				{
					Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
					Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<PanSpark>(), (int)(projectile.damage * 0.2), 0f, projectile.owner);
				}
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/SearedPanSmash"), (int)projectile.position.X, (int)projectile.position.Y);
				// Stealth strikes also cause any fireballs to home in on their targets
				FireballStuff(true);
			}
			else if (targetIndex != -1 && health > 0)
			{
				// Summon three fireballs to circle the hit enemy
				int projType = ModContent.ProjectileType<NiceCock>();
				for (int t = 0; t < 3; t++)
				{
					Projectile.NewProjectile(projectile.Center, Vector2.Zero, projType, (int)(projectile.damage * 0.2), 0f, projectile.owner, 0f, targetIndex);
				}
				int fireballCount = 0;
				// Count how many fireballs exist already around the given target
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					// Keep the loop as short as possible
					if (!Main.projectile[i].active || Main.projectile[i].owner != projectile.owner || !Main.projectile[i].Calamity().rogue || targetIndex != (int)Main.projectile[i].ai[1])
						continue;
					if (Main.projectile[i].type == projType)
					{
						if ((Main.projectile[i].modProjectile as NiceCock).homing)
							continue;
						fireballCount++;
					}
				}
				// Adjust the angle of the existing fireballs around a target
				float angleVariance = MathHelper.TwoPi / fireballCount;
				float angle = 0f;
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					if (!Main.projectile[i].active || Main.projectile[i].owner != projectile.owner || !Main.projectile[i].Calamity().rogue || targetIndex != (int)Main.projectile[i].ai[1])
						continue;
					if (Main.projectile[i].type == projType)
					{
						if ((Main.projectile[i].modProjectile as NiceCock).homing)
							continue;
						Main.projectile[i].ai[0] = angle;
						Main.projectile[i].netUpdate = true;
						angle += angleVariance;
					}
				}
			}
		}

		private void FireballStuff(bool activate)
		{
			if (projectile.owner == Main.myPlayer)
			{
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					if (!Main.projectile[i].active || Main.projectile[i].owner != projectile.owner)
						continue;
					if (Main.projectile[i].type == ModContent.ProjectileType<NiceCock>())
					{
						if (!activate)
							Main.projectile[i].Kill();
						else
						{
							(Main.projectile[i].modProjectile as NiceCock).homing = true;
							Main.projectile[i].extraUpdates += 2;
						}
					}
				}
			}
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			// Kill all fireballs if you miss a stealth strike
			if (projectile.Calamity().stealthStrike)
				FireballStuff(false);
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
		{
			// Stealth strikes are golden colored
			if (projectile.Calamity().stealthStrike)
				return new Color(255, 222, 0);
			return null;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
