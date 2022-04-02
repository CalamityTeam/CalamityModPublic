using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class PoisonPack : RogueWeapon
    {
        private static int baseDamage = 20;
        private static float baseKnockback = 1.8f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poison Pack");
            Tooltip.SetDefault("Throws a poisonous spiky ball. Stacks up to 3.\n" +
                "Stealth strikes cause the balls to release spore clouds\n" +
                "Right click to delete all existing spiky balls");
        }

        public override void SafeSetDefaults()
        {
            item.damage = baseDamage;
            item.Calamity().rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 14;
            item.height = 14;
            item.useTime = 19;
            item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = baseKnockback;
            item.value = Item.buyPrice(0, 0, 33, 0);
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item1;
            item.maxStack = 3;

            item.shootSpeed = 7f;
            item.shoot = ModContent.ProjectileType<PoisonBol>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 4;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.shoot = ProjectileID.None;
                item.shootSpeed = 0f;
                return player.ownedProjectileCounts[ModContent.ProjectileType<PoisonBol>()] > 0;
            }
            else
            {
                item.shoot = ModContent.ProjectileType<PoisonBol>();
                item.shootSpeed = 7f;
                int UseMax = item.stack;
                return player.ownedProjectileCounts[ModContent.ProjectileType<PoisonBol>()] < UseMax;
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.killSpikyBalls = false;
            if (modPlayer.StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.killSpikyBalls = true;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.SpikyBall, 50);
            recipe.AddIngredient(ItemID.JungleSpores, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
