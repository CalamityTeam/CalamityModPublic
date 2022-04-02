using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DesecratedWater : RogueWeapon
    {
        public const int BaseDamage = 55;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desecrated Water");
            Tooltip.SetDefault(@"Throws an unholy flask of water that explodes into an explosion of bubbles on death
Stealth strikes spawn additional bubbles that inflict Ichor and Cursed Inferno");
        }

        public override void SafeSetDefaults()
        {
            item.damage = BaseDamage;
            item.width = 22;
            item.height = 24;
            item.useAnimation = 29;
            item.useTime = 29;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4.5f;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DesecratedWaterProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnyEvilWater", 100);
            recipe.AddRecipeGroup("AnyAdamantiteBar", 5);
            recipe.AddRecipeGroup("CursedFlameIchor", 5);
            recipe.AddIngredient(ItemID.SoulofNight, 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
