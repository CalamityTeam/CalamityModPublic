using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class PiercingBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Piercing Blow");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.light = 0.5f;
            projectile.alpha = 255;
            projectile.extraUpdates = 10;
            projectile.scale = 1.18f;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = ProjectileID.BulletHighVelocity;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			//Avoid touching things that you probably aren't meant to damage
			if (target.defense > 999 || target.Calamity().DR >= 0.95f || target.Calamity().unbreakableDR)
				return;

			//DR applies after defense, so undo it first
			damage = (int)(damage * (1 / (1 - target.Calamity().DR)));

			//Then proceed to ignore all defense
			int penetratableDefense = Math.Max(target.defense - Main.player[projectile.owner].armorPenetration, 0);
			int penetratedDefense = Math.Min(penetratableDefense, target.defense);
			damage += (int)(0.5f * penetratedDefense);
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(0, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
            {
				int bulletCount = 10;
                for (int x = 0; x < bulletCount; x++)
                {
					float speed = 12f;
                    float xPos = projectile.Center.X + (x < bulletCount / 2 ? 500f : -500f);
					float yPos = projectile.Center.Y + Main.rand.Next(-500, 501);
                    Vector2 origin = new Vector2(xPos, yPos);
					Vector2 velocity = new Vector2(target.Center.X - origin.X, target.Center.Y - origin.Y);
					velocity.Normalize();
					velocity *= speed;
                    if (projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(origin, velocity, ModContent.ProjectileType<AMR2>(), (int)(projectile.damage * 0.2), projectile.knockBack, projectile.owner);
                    }
                }
            }
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 900);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (crit)
            {
				int bulletCount = 10;
                for (int x = 0; x < bulletCount; x++)
                {
					float speed = 12f;
                    float xPos = projectile.Center.X + (x < bulletCount / 2 ? 500f : -500f);
					float yPos = projectile.Center.Y + Main.rand.Next(-500, 501);
                    Vector2 origin = new Vector2(xPos, yPos);
					Vector2 velocity = new Vector2(target.Center.X - origin.X, target.Center.Y - origin.Y);
					velocity.Normalize();
					velocity *= speed;
                    if (projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(origin, velocity, ModContent.ProjectileType<AMR2>(), (int)(projectile.damage * 0.2), projectile.knockBack, projectile.owner);
                    }
                }
            }
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 900);
        }
    }
}
