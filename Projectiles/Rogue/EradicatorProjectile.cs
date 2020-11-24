using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class EradicatorProjectile : ModProjectile
	{
		public override string Texture => "CalamityMod/Items/Weapons/Rogue/Eradicator";
		// If you can read this, the string below does not apply because you have the original ;)
		internal const string HagingText = "If you can read this, you probably decompiled the source and therefore, you suck >:C ~Ben";
		// Aside, programming stealth strikes is somewhat a bore.  Also, my internet went out at this time. I need to entertain myself.
		internal float rotationDirection = 1f;
		internal float rotationAmt = 0f;
		internal float shootCounter = 0f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eradicator");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 58;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.aiStyle = 3;
			projectile.timeLeft = 180;
			aiType = ProjectileID.WoodenBoomerang;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 3;
			projectile.Calamity().rogue = true;
		}

		public override void AI()
		{
			Lighting.AddLight(projectile.Center, 0.35f, 0f, 0.25f);
			if (projectile.aiStyle == 3)
				CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 300f, 6f, 8f, 6, ModContent.ProjectileType<NebulaShot>(), 1D, true);
			else
				StealthStrikeAI();
		}

		private void StealthStrikeAI()
		{
			if (rotationAmt < 0.75f)
				rotationAmt += 0.005f;
            projectile.rotation += rotationAmt * rotationDirection;
			shootCounter += Main.rand.NextFloat(0f, 2f);

			//Fire lasers at random
			if (shootCounter >= 8f)
			{
				shootCounter = 0f;
				Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
				Projectile laser = Projectile.NewProjectileDirect(projectile.Center, velocity, ModContent.ProjectileType<NebulaShot>(), projectile.damage, 0f, projectile.owner);
				laser.Calamity().forceRogue = true;
				laser.aiStyle = Main.rand.NextBool() ? 1 : -1;
				laser.penetrate = -1;
				laser.usesLocalNPCImmunity = true;
				laser.localNPCHitCooldown = 20;
			}

			projectile.StickyProjAI(6, true);

			if (projectile.ai[0] != 0f)
				return;
			//Lazily homes in on the nearest NPC (boss preferential)
			const float turnSpeed = 30f;
			const float speedMult = 5f;
			const float homingRange = 600f;
            NPC uDie = projectile.Center.ClosestNPCAt(homingRange, true, true);
            if (uDie != null)
            {
				Vector2 distNorm = (uDie.Center - projectile.Center).SafeNormalize(Vector2.UnitX);
				projectile.velocity = (projectile.velocity * (turnSpeed - 1f) + distNorm * speedMult) / turnSpeed;
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 600);
			target.AddBuff(BuffID.Frostburn, 600);
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 600);
			target.AddBuff(BuffID.Frostburn, 600);
		}

		public override void ModifyHitPvp(Player target, ref int damage, ref bool crit) => StealthStrikeSetup();
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			StealthStrikeSetup();
            projectile.ModifyHitNPCSticky(3, true);
        }

		private void StealthStrikeSetup()
		{
			if (!projectile.Calamity().stealthStrike || projectile.aiStyle == -1)
				return;
			rotationDirection = projectile.direction;
			projectile.aiStyle = -1;
			projectile.velocity *= 0.1f;
			projectile.ai[0] = projectile.ai[1] = 0f;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 origin = new Vector2(31f, 29f);
			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/EradicatorGlow"), projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
		}
	}
}
