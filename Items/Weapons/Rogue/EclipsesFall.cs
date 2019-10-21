using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class EclipsesFall : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eclipse's Fall");
            Tooltip.SetDefault("When the sun goes dark, you will know judgment\n" +
            "Summons spears from the sky on hit\n" +
            "Stealth strikes impale enemies and summon a constant barrage of spears");
        }

        public override void SafeSetDefaults()
        {
            item.width = 72;
            item.damage = 1300;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.useTime = 20;
            item.knockBack = 3.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 72;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 14;
            item.shoot = ModContent.ProjectileType<EclipsesFallMain>();
            item.shootSpeed = 15f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<EclipsesStealth>(), damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[p].Calamity().stealthStrike = true;
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DayBreak, 1);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 15);
            recipe.AddIngredient(ModContent.ItemType<CoreofCinder>(), 6);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
