using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ContaminatedBile : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Contaminated Bile");
            Tooltip.SetDefault("Throws a flask of sickly green, irradiated bile which explodes on collision\n" +
                               "Stealth strikes make the explosion much more violent and powerful");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 9;
            item.width = item.height = 24;
            item.useAnimation = item.useTime = 31;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4.5f;
            item.rare = ItemRarityID.Green;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ContaminatedBileFlask>();
            item.shootSpeed = 15f;
            item.Calamity().rogue = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            int p = Projectile.NewProjectile(position, velocity, type, damage, knockBack, player.whoAmI);
            if (p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<SulfuricScale>(), 10);
            recipe.AddTile(TileID.Bottles);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
