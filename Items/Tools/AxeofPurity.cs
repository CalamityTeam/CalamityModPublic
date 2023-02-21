using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    [LegacyName("PurityAxe")]
    public class AxeofPurity : ModItem
    {
        private static int AxePower = 125 / 5;
        private static float PowderSpeed = 21f;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Axe of Purity");
            Tooltip.SetDefault("Left click to use as a tool\n" +
                "Right click to cleanse evil");
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.knockBack = 5f;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.axe = AxePower;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.DamageType = DamageClass.Melee;
            Item.width = 58;
            Item.height = 54;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = PowderSpeed;
        }

        public override bool CanShoot(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                return false;
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int powderDamage = (int)(0.85f * damage);
            int idx = Projectile.NewProjectile(source, position, velocity, type, powderDamage, knockback, player.whoAmI);
            Main.projectile[idx].DamageType = DamageClass.Melee;
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void UseAnimation(Player player)
        {
            //Done here because no other override fits neatly enough in the pipeline.
            Item.axe = AxePower;

            if (player.altFunctionUse == 2)
                Item.axe = 0;
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
