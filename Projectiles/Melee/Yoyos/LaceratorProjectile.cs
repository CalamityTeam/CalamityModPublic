using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class LaceratorProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lacerator");
		}
    	
        public override void SetDefaults()
        {
        	projectile.CloneDefaults(ProjectileID.TheEyeOfCthulhu);
            projectile.width = 16;
            projectile.scale = 1.1f;
            projectile.height = 16;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            aiType = 555;
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 5;
        }

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (target.type == NPCID.TargetDummy || !target.canGhostHeal)
			{
				return;
			}
			Player player = Main.player[projectile.owner];
			if (Main.rand.Next(5) == 0)
			{
				player.statLife += 1;
				player.HealEffect(1);
			}
		}
	}
}