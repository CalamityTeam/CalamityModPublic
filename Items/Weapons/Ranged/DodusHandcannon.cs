using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class DodusHandcannon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 34;
            Item.damage = 1020;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;

            // Reduce volume to 30% so it stops destroying people's ears.
            Item.UseSound = CommonCalamitySounds.LargeWeaponFireSound with { Volume = 0.3f };

            Item.shoot = ModContent.ProjectileType<HighExplosivePeanutShell>();
            Item.shootSpeed = 13f;
            Item.useAmmo = AmmoID.Bullet;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.Calamity().donorItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Item.shoot;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-17, 5);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PearlGod>().
                AddIngredient<RuinousSoul>(5).
                AddIngredient(ItemID.LunarBar, 15).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
