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
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 62;
            item.damage = BaseDamage;
            item.knockBack = 5f;
            item.useAnimation = item.useTime = 22;

            item.noUseGraphic = true;
            item.melee = true;
            item.noMelee = true;
            item.channel = true;
            item.autoReuse = true;
            item.useTurn = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;

            item.value = CalamityGlobalItem.Rarity2BuyPrice;
            item.rare = ItemRarityID.Green;
            item.Calamity().donorItem = true;

            item.shoot = ModContent.ProjectileType<YateveoBloomProj>();
            item.shootSpeed = ShootSpeed;
            item.Calamity().trueMelee = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override float UseTimeMultiplier    (Player player)
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
                item.channel = false;
                item.autoReuse = true;
            }
            // Flail
            else
            {
                item.channel = true;
                item.autoReuse = false;
            }
            return player.ownedProjectileCounts[item.shoot] + player.ownedProjectileCounts[ModContent.ProjectileType<YateveoBloomSpear>()] <= 0;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float speedMult = SpearSpeed / ShootSpeed;
            if (player.altFunctionUse == 2)
                Projectile.NewProjectile(position.X, position.Y, speedX * speedMult, speedY * speedMult, ModContent.ProjectileType<YateveoBloomSpear>(), (int)(damage * 0.666666f), knockBack, player.whoAmI);
            else
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.JungleRose);
            recipe.AddIngredient(ItemID.RichMahogany, 15);
            recipe.AddIngredient(ItemID.JungleSpores, 12);
            recipe.AddIngredient(ItemID.Stinger, 4);
            recipe.AddIngredient(ItemID.Vine, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
