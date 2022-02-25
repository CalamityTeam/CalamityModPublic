using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class PurityAxe : ModItem
    {
        private static int AxePower = 125 / 5;
        private static float PowderSpeed = 21f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Axe of Purity");
            Tooltip.SetDefault("Left click to use as a tool\n" +
                "Right click to cleanse evil");
        }

        public override void SetDefaults()
        {
            item.damage = 75;
            item.knockBack = 5f;
            item.useTime = 15;
            item.useAnimation = 15;
            item.axe = AxePower;

            item.melee = true;
            item.width = 58;
            item.height = 54;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int powderDamage = (int)(0.85f * damage);
            int idx = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, powderDamage, knockBack, player.whoAmI, 0f, 0f);
            Main.projectile[idx].melee = true;
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.axe = 0;
                item.shoot = ProjectileID.PurificationPowder;
                item.shootSpeed = PowderSpeed;
            }
            else
            {
                item.axe = AxePower;
                item.shoot = ProjectileID.None;
                item.shootSpeed = 0f;
            }
            return base.CanUseItem(player);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 58);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FellerofEvergreens>());
            recipe.AddIngredient(ItemID.PurificationPowder, 20);
            recipe.AddIngredient(ItemID.PixieDust, 20);
            recipe.AddIngredient(ItemID.CrystalShard, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
