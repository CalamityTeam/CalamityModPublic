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
            Item.width = 48;
            Item.height = 48;
            Item.damage = 43;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 22;
            Item.useTime = 22;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<GreenDonkeyKongReference>();
            Item.shootSpeed = 14f;
            Item.Calamity().rogue = true;
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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<BlastBarrel>()).AddIngredient(ModContent.ItemType<ContaminatedBile>()).AddIngredient(ModContent.ItemType<CorrodedFossil>(), 15).AddTile(TileID.Anvils).Register();
        }
    }
}
