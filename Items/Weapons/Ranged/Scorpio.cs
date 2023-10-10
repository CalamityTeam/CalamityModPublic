using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("Scorpion")]
    public class Scorpio : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 58;
            Item.height = 26;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6.5f;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<MiniRocket>();
            Item.useAmmo = AmmoID.Rocket;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20, 10);

        public override bool AltFunctionUse(Player player) => true;

        public override float UseSpeedMultiplier(Player player) => player.altFunctionUse == 2 ? 0.3333f : 1f;

        // Figure out which rocket is used
        public int RocketType;
        public override void OnConsumeAmmo(Item ammo, Player player) => RocketType = ammo.type;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BigNuke>(), (int)(damage * 1.85), knockback * 2f, player.whoAmI, RocketType);
            else
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<MiniRocket>(), damage, knockback, player.whoAmI, RocketType);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SnowmanCannon).
                AddIngredient(ItemID.FragmentNebula, 6).
                AddIngredient(ItemID.Nanites, 100).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
