using CalamityMod.Projectiles.Melee.MaceFlails;
using CalamityMod.Projectiles.Melee.Spears;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class YateveoBloom : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static float ShootSpeed = 12f;
        public static float SpearSpeed = 4.5f;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Poisoned];
            // Flail already does half damage. No tooltip mutliplier required.
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 62;
            Item.damage = 30;
            Item.knockBack = 5f;
            Item.useAnimation = Item.useTime = 22;

            Item.noUseGraphic = true;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;

            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<YateveoBloomMace>();
            Item.shootSpeed = ShootSpeed;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse != 2)
                return 1f;
            return 0.66f;
        }

        public override bool CanUseItem(Player player)
        {
            // Spear
            if (player.altFunctionUse == 2)
            {
                Item.channel = false;
                Item.autoReuse = true;
            }
            // Flail
            else
            {
                Item.channel = true;
                Item.autoReuse = false;
            }
            return player.ownedProjectileCounts[Item.shoot] + player.ownedProjectileCounts[ModContent.ProjectileType<YateveoBloomSpear>()] <= 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float speedMult = SpearSpeed / ShootSpeed;
            if (player.altFunctionUse == 2)
                Projectile.NewProjectile(source, position, velocity * speedMult, ModContent.ProjectileType<YateveoBloomSpear>(), damage, knockback, player.whoAmI);
            else
                Projectile.NewProjectile(source, position, velocity, type, (int)(damage * 0.5f), knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.JungleRose).
                AddIngredient(ItemID.RichMahogany, 15).
                AddIngredient(ItemID.JungleSpores, 12).
                AddIngredient(ItemID.Stinger, 4).
                AddIngredient(ItemID.Vine, 2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
