using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SpearofDestiny : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear of Destiny");
            Tooltip.SetDefault("Throws three spears with the outer two having homing capabilities\n" +
            "Stealth strikes cause all three spears to home in, ignore tiles, and pierce more");
        }

        public override void SafeSetDefaults()
        {
            item.width = 52;
            item.damage = 26;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 52;
            item.value = CalamityGlobalItem.Rarity6BuyPrice;
            item.rare = ItemRarityID.LightPurple;
            item.shoot = ModContent.ProjectileType<SpearofDestinyProjectile>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int index = 7;
            for (int i = -index; i <= index; i += index)
            {
                int projType = (i != 0 || player.Calamity().StealthStrikeAvailable()) ? type : ModContent.ProjectileType<IchorSpearProj>();
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(i));
                int spear = Projectile.NewProjectile(position, perturbedSpeed, projType, damage, knockBack, player.whoAmI);
                if (spear.WithinBounds(Main.maxProjectiles))
                    Main.projectile[spear].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HallowedBar, 7);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ItemID.SoulofFright, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
