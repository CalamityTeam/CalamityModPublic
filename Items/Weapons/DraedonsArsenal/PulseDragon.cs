using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class PulseDragon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse Dragon");
            Tooltip.SetDefault("Heavy duty flails, each containing a powerful generator which is activated upon launch.\n" +
            "Throws two dragon heads that emit electrical fields\n" +
            "Especially effective against inorganic targets");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.damage = 300;
            item.melee = true;
            item.width = 30;
            item.height = 10;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 8f;
            item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.channel = true;
            item.shoot = ModContent.ProjectileType<PulseDragonProjectile>();
            item.shootSpeed = 20f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 190f;
            modItem.ChargePerUse = 0.32f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            float offsetAngle = Main.rand.NextFloat(0.2f, 0.4f);
            velocity += player.velocity;
            float velocityAngle = velocity.ToRotation();
            Projectile projectile = Projectile.NewProjectileDirect(position, velocity, type, damage, knockBack, player.whoAmI, velocityAngle, Main.rand.NextFloat(30f, PulseDragonProjectile.MaximumPossibleOutwardness));
            projectile.localAI[0] = 1f;
            projectile = Projectile.NewProjectileDirect(position, velocity.RotatedBy(offsetAngle), type, damage, knockBack, player.whoAmI, velocityAngle + offsetAngle, Main.rand.NextFloat(30f, PulseDragonProjectile.MaximumPossibleOutwardness));
            projectile.localAI[0] = -1f;
            return false;
        }

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 4);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 12);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 18);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 8);
            recipe.AddIngredient(ItemID.LunarBar, 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
