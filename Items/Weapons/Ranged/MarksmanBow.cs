using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MarksmanBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marksman Bow");
            Tooltip.SetDefault("Fires three arrows at once\n" +
            "Wooden arrows are converted into Jester's arrows");
        }

        public override void SetDefaults()
        {
            item.damage = 30;
            item.ranged = true;
            item.width = 36;
            item.height = 110;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ProjectileID.JestersArrow;
            item.shootSpeed = 10f;
            item.useAmmo = AmmoID.Arrow;

            item.value = Item.buyPrice(gold: 80); // crafted out of nothing but 31 ectoplasm so it has unique pricing
            item.rare = ItemRarityID.Yellow;
            item.Calamity().donorItem = true;
            item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 20;

        public override Vector2? HoldoutOffset() => new Vector2(-4, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            //Convert wooden arrows to Jester's Arrows
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ProjectileID.JestersArrow;

            for (int i = 0; i < 3; i++)
            {
                int randomExtraUpdates = Main.rand.Next(3);
                float SpeedX = speedX + Main.rand.NextFloat(-10f, 10f) * 0.05f;
                float SpeedY = speedY + Main.rand.NextFloat(-10f, 10f) * 0.05f;
                int arrow = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI);
                Main.projectile[arrow].noDropItem = true;
                Main.projectile[arrow].extraUpdates += randomExtraUpdates; //0 to 2 extra updates
                if (type == ProjectileID.JestersArrow)
                {
                    Main.projectile[arrow].localNPCHitCooldown = 10 * (randomExtraUpdates + 1);
                    Main.projectile[arrow].usesLocalNPCImmunity = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Ectoplasm, 31);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
