using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class PalladiumJavelin : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 54;
            Item.damage = 81;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<PalladiumJavelinProjectile>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override float StealthDamageMultiplier => 0.9f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float aiVal = player.Calamity().StealthStrikeAvailable() ? -12f : 0f;
            int javelin = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: aiVal);
            if (javelin.WithinBounds(Main.maxProjectiles))
                Main.projectile[javelin].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PalladiumBar, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
