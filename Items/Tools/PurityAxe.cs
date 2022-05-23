using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Axe of Purity");
            Tooltip.SetDefault("Left click to use as a tool\n" +
                "Right click to cleanse evil");
        }

        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.knockBack = 5f;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.axe = AxePower;

            Item.DamageType = DamageClass.Melee;
            Item.width = 58;
            Item.height = 54;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int powderDamage = (int)(0.85f * damage);
            int idx = Projectile.NewProjectile(source, position, velocity, type, powderDamage, knockback, player.whoAmI, 0f, 0f);
            Main.projectile[idx].DamageType = DamageClass.Melee;
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.axe = 0;
                Item.shoot = ProjectileID.PurificationPowder;
                Item.shootSpeed = PowderSpeed;
            }
            else
            {
                Item.axe = AxePower;
                Item.shoot = ProjectileID.None;
                Item.shootSpeed = 0f;
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
            CreateRecipe().
                AddIngredient<FellerofEvergreens>().
                AddIngredient(ItemID.PurificationPowder, 20).
                AddIngredient(ItemID.PixieDust, 20).
                AddIngredient(ItemID.CrystalShard, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
