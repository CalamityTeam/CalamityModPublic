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
            Item.width = 26;
            Item.damage = 28;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 18;
            Item.knockBack = 6.5f;
            Item.UseSound = SoundID.Item1;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<BrimbladeProj>();
            Item.shootSpeed = 12f;
            Item.Calamity().rogue = true;
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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<UnholyCore>(), 4).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
