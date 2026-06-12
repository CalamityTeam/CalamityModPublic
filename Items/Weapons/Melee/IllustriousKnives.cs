using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("RoyalKnives", "RoyalKnivesMelee", "RoyalKnivesRogue")]
    public class IllustriousKnives : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 62;
            Item.damage = 525;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 8;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = true;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;

            Item.shoot = ModContent.ProjectileType<IllustriousKnife>();
            Item.shootSpeed = 9f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int knifeAmt = 4;
            if (Main.rand.NextBool()) knifeAmt++;
            if (Main.rand.NextBool(4)) knifeAmt++;
            if (Main.rand.NextBool(6)) knifeAmt++;
            if (Main.rand.NextBool(8)) knifeAmt++;

            for (int i = 0; i < knifeAmt; i++)
            {
                Vector2 knifeVel = velocity.RotatedByRandom(MathHelper.Pi / 6f);
                Projectile.NewProjectile(source, position, knifeVel, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EmpyreanKnives>().
                AddIngredient<ShadowspecBar>(5).
                AddIngredient<CoreofCalamity>(2).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
