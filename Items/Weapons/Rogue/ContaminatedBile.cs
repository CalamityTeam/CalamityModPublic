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
            Item.damage = 9;
            Item.width = Item.height = 24;
            Item.useAnimation = Item.useTime = 31;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4.5f;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.UseSound = SoundID.Item106;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ContaminatedBileFlask>();
            Item.shootSpeed = 15f;
            Item.Calamity().rogue = true;
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
            CreateRecipe(1).AddIngredient(ItemID.BottledWater).AddIngredient(ModContent.ItemType<SulfuricScale>(), 10).AddTile(TileID.Bottles).Register();
        }
    }
}
