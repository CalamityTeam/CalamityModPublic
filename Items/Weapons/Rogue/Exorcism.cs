using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Exorcism : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exorcism");
            Tooltip.SetDefault("Throws a hallowed cross which explodes into a flash of light that damages nearby enemies, closer enemies receiving more damage\n" +
                               "As the cross travels downwards, the damage inflicted by both the cross and flash increases constantly\n" +
                               "Stealth strikes cause the cross to be thrown with full damage immediately. Hallowed stars fall when the cross explodes");
        }

        public override void SafeSetDefaults()
        {
            item.width = 34;
            item.damage = 55;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 42;
            item.maxStack = 1;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<ExorcismProj>();
            item.shootSpeed = 10f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)(damage * 1.15f), knockBack, player.whoAmI, 2f, damage);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
            }
            else
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 1f, damage);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HolyWater, 10);
            recipe.AddIngredient(ItemID.HallowedBar, 12);
            recipe.AddIngredient(ItemID.SoulofMight, 6);
            recipe.AddIngredient(ItemID.SoulofSight, 6);
            recipe.AddIngredient(ItemID.SoulofFright, 6);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
