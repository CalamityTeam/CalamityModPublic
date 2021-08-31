using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class CobaltKunai : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cobalt Kunai");
            Tooltip.SetDefault("Stealth strikes fire three homing cobalt energy bolts");
        }

        public override void SafeSetDefaults()
        {
            item.width = 18;
            item.damage = 36;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 12;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 40;
            item.maxStack = 999;
            item.value = 900;
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<CobaltKunaiProjectile>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
			{
				for (int i = -6; i <= 6; i += 6)
				{
					Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(i));
					int stealth = Projectile.NewProjectile(position, perturbedSpeed, ModContent.ProjectileType<CobaltEnergy>(), damage, knockBack, player.whoAmI);
					if (stealth.WithinBounds(Main.maxProjectiles))
						Main.projectile[stealth].Calamity().stealthStrike = true;
				}
				return false;
			}
			return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CobaltBar);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 30);
            recipe.AddRecipe();
        }
    }
}
