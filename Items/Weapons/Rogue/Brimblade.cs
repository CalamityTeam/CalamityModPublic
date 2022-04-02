using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Brimblade : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimblade");
            Tooltip.SetDefault("Throws a blade that splits on enemy hits\n" +
            "Stealth strikes split further and cause the player to launch a barrage of brimstone darts");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.damage = 28;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 18;
            item.knockBack = 6.5f;
            item.UseSound = SoundID.Item1;
            item.height = 26;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<BrimbladeProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int blade = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (blade.WithinBounds(Main.maxProjectiles))
                    Main.projectile[blade].Calamity().stealthStrike = true;

                for (int i = -6; i <= 6; i += 4)
                {
                    Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(i));
                    int dart = Projectile.NewProjectile(position, perturbedSpeed, ModContent.ProjectileType<SeethingDischargeBrimstoneBarrage>(), damage, knockBack * 0.5f, player.whoAmI);
                    if (dart.WithinBounds(Main.maxProjectiles))
                        Main.projectile[dart].Calamity().forceRogue = true;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UnholyCore>(), 4);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
