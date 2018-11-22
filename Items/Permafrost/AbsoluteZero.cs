using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Permafrost
{
	public class AbsoluteZero : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Absolute Zero");
			Tooltip.SetDefault("Ancient blade imbued with the Archmage of Ice's magic\nShoots dark ice crystals\nThe blade creates frost explosions on direct hits");
		}
		public override void SetDefaults()
		{
			item.damage = 56;
			item.melee = true;
			item.width = 58;
			item.height = 58;
			item.useTime = 20;
            item.useAnimation = 20;
			item.useStyle = 1;
			item.useTurn = false;
			item.knockBack = 4f;
			item.value = Item.buyPrice(0, 50, 0, 0);
            item.rare = 5;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("DarkIceZero");
            item.shootSpeed = 3f;
		}
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 600);
            target.AddBuff(mod.BuffType("GlacialState"), 300);

            int p = Projectile.NewProjectile(target.Center, Vector2.Zero, mod.ProjectileType("DarkIceZero"), damage / 3, knockBack * 3, player.whoAmI);
            Main.projectile[p].Kill();
        }
    }
}
