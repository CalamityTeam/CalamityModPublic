using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AquaticDissolution : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Dissolution");
            Tooltip.SetDefault("Fires whaling spears from the sky that bounce off tiles");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.damage = 125;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item60;
            Item.autoReuse = true;
            Item.height = 72;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.shoot = ModContent.ProjectileType<OceanBeam>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position = Main.MouseWorld;
            for (int x = 0; x < 3; x++)
            {
                Projectile.NewProjectile(source, new Vector2(position.X + (float)Main.rand.Next(-30, 31), position.Y - 600f), new Vector2(0f, 8f), type, damage, knockback, Main.myPlayer, 0f, 0f);
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int num250 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 33, (float)(player.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
                Main.dust[num250].velocity *= 0.2f;
                Main.dust[num250].noGravity = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Mariana>().
                AddIngredient<UeliaceBar>(7).
                AddIngredient<BarofLife>(2).
                AddIngredient<Lumenite>(20).
                AddIngredient<Tenebris>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
