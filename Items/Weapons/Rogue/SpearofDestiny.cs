using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SpearofDestiny : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear of Destiny");
            Tooltip.SetDefault("Throws three spears with the outer two having homing capabilities\n" +
            "Stealth strikes cause all three spears to home in, ignore tiles, and pierce more");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.damage = 26;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 52;
            Item.value = CalamityGlobalItem.Rarity6BuyPrice;
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<SpearofDestinyProjectile>();
            Item.shootSpeed = 20f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int index = 7;
            for (int i = -index; i <= index; i += index)
            {
                int projType = (i != 0 || player.Calamity().StealthStrikeAvailable()) ? type : ModContent.ProjectileType<IchorSpearProj>();
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.ToRadians(i));
                int spear = Projectile.NewProjectile(source, position, perturbedSpeed, projType, damage, knockback, player.whoAmI);
                if (spear.WithinBounds(Main.maxProjectiles))
                    Main.projectile[spear].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 7).
                AddIngredient(ItemID.SoulofLight, 5).
                AddIngredient(ItemID.SoulofFright, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
