using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TerraShiv : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Shiv");
            Tooltip.SetDefault("Don't underestimate the power of shivs");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTurn = false;
            Item.useAnimation = 13;
            Item.useTime = 13;
            Item.width = 42;
            Item.height = 42;
            Item.damage = 140;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 6.5f;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ShortTerraBeam>();
            Item.shootSpeed = 12f;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, Item.shootSpeed * player.direction, 0f, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 107);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TrueNightsStabber>().
                AddIngredient<TrueExcaliburShortsword>().
                AddIngredient<LivingShard>(7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
