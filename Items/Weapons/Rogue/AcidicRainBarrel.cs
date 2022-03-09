using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class AcidicRainBarrel : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acidic Rain Barrel");
            Tooltip.SetDefault("Throws a rolling barrel that explodes on wall collision\n" +
                               "Stealth strikes make it rain on collision");
        }

        public override void SafeSetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 43;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 22;
            item.useTime = 22;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<GreenDonkeyKongReference>();
            item.shootSpeed = 14f;
            item.Calamity().rogue = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            speedY *= 0.667f;
            Vector2 initialVelocity = new Vector2(speedX, speedY);

            int p = Projectile.NewProjectile(position - Vector2.UnitY * 12f, initialVelocity, type, damage, knockBack, player.whoAmI);
			if (p.WithinBounds(Main.maxProjectiles))
				Main.projectile[p].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BlastBarrel>());
            recipe.AddIngredient(ModContent.ItemType<ContaminatedBile>());
            recipe.AddIngredient(ModContent.ItemType<CorrodedFossil>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
