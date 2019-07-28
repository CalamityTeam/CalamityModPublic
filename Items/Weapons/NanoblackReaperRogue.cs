using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Weapons
{
	public class NanoblackReaperRogue : CalamityDamageItem
	{
		public static int BaseDamage = 800;
		public static float Knockback = 9f;
		public static float Speed = 16f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nanoblack Reaper");
			Tooltip.SetDefault("Unleashes a storm of nanoblack energy blades\nBlades target bosses whenever possible\n'She smothered them in Her hatred'");
		}

		public override void SafeSetDefaults()
		{
			item.width = 78;
			item.height = 64;
			item.damage = BaseDamage;
			item.knockBack = Knockback;
			item.useTime = 6;
			item.useAnimation = 6;
			item.autoReuse = true;
			item.noMelee = true;
			item.noUseGraphic = true;

			item.useStyle = 1;
			item.UseSound = SoundID.Item18;

			item.rare = 10;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 16;
			item.value = Item.buyPrice(5, 0, 0, 0);

			item.shoot = mod.ProjectileType("NanoblackMain");
			item.shootSpeed = Speed;
		}

        // ai[0] = 1f so that the projectile is rogue.
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, Main.myPlayer, 1f);
            return false;
        }

        public override void AddRecipes()
		{
			ModRecipe r = new ModRecipe(mod);
			r.SetResult(this);
			r.AddTile(null, "DraedonsForge");
			r.AddIngredient(null, "GhoulishGouger");
			r.AddIngredient(null, "SoulHarvester");
			r.AddIngredient(null, "EssenceFlayer");
			r.AddIngredient(null, "ShadowspecBar", 5);
			r.AddIngredient(null, "EndothermicEnergy", 40);
			r.AddIngredient(null, "DarkPlasma", 10);
			r.AddIngredient(ItemID.Nanites, 400);
			r.AddRecipe();
		}
	}
}
