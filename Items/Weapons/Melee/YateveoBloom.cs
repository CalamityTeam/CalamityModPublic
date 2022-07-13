using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Melee.Spears;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class YateveoBloom : ModItem
    {
        public static int BaseDamage = 30; //Spear is 20 damage, Flail is 30 damage
        public static float ShootSpeed = 12f;
        public static float SpearSpeed = 4.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yateveo Bloom");
            Tooltip.SetDefault("A synthesis of jungle flora\n" +
                "Throws a powerful rose flail\n" +
                "Right click to stab with a flower spear");
            SacrificeTotal = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 62;
            Item.damage = BaseDamage;
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

            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<YateveoBloomProj>();
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
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X * speedMult, velocity.Y * speedMult, ModContent.ProjectileType<YateveoBloomSpear>(), (int)(damage * 0.666666f), knockback, player.whoAmI);
            else
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
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
