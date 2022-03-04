using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AegisBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aegis Blade");
            Tooltip.SetDefault("Striking an enemy with the blade causes an earthen eruption\n" +
                "Right click to fire an aegis bolt");
        }

        public override void SetDefaults()
        {
            item.width = 72;
			item.height = 72;
			item.scale = 1.2f;
			item.damage = 108;
            item.melee = true;
            item.useAnimation = item.useTime = 15;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shootSpeed = 14f;

            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Yellow;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.noMelee = true;
                item.UseSound = SoundID.Item73;
                item.shoot = ModContent.ProjectileType<AegisBeam>();
            }
            else
            {
                item.noMelee = false;
                item.UseSound = SoundID.Item1;
                item.shoot = ProjectileID.None;
            }
            return base.CanUseItem(player);
        }

		public override float UseTimeMultiplier	(Player player)
		{
			if (player.altFunctionUse != 2)
				return 1f;
			return 0.75f;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<AegisBeam>(), (int)(damage * 0.3), knockBack, player.whoAmI);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 246, 0f, 0f, 0, new Color(255, Main.DiscoG, 53));
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
			if (crit)
				damage /= 2;

			Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<AegisBlast>(), damage, knockback, Main.myPlayer);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
			if (crit)
				damage /= 2;

			Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<AegisBlast>(), damage, item.knockBack, Main.myPlayer);
        }
    }
}
