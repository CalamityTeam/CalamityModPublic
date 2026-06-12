using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CrescentMoon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.damage = 1000;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item82;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.shootSpeed = 24f;
            Item.shoot = ModContent.ProjectileType<CrescentMoonFlail>();
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().StratusStarburstResetTimer = (int)MathHelper.Max(player.Calamity().StratusStarburstResetTimer, 600);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float ai3 = (Main.rand.NextFloat() - 0.5f) * 0.7853982f; //0.5
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, ai3);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BlueMoon).
                AddIngredient<Lumenyl>(8).
                AddIngredient<RuinousSoul>(3).
                AddIngredient<ExodiumCluster>(16).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
