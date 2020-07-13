using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
	public class MassivePlasmaExplosion : ModProjectile
	{
		public float Time
		{
			get => projectile.ai[0];
			set => projectile.ai[0] = value;
		}
		public const int Lifetime = 25;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Laser Rifle");
		}

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 840;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.timeLeft = 37;
			projectile.tileCollide = false;
			projectile.Calamity().rogue = true;
			projectile.usesIDStaticNPCImmunity = true;
			projectile.idStaticNPCHitCooldown = 10;
		}

		public override void AI()
		{
			Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 4f * projectile.scale);
			if (projectile.localAI[0] == 0f)
			{
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/FlareSound"), projectile.Center);
				projectile.localAI[0] = 1f;
			}
			projectile.scale = (float)Math.Sin(Time / 37 * MathHelper.Pi) * 2f;
			if (projectile.scale > 1f)
				projectile.scale = 1f;
			Time++;
		}
	}
}
