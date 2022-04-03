using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AquaticDissolution : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Dissolution");
            Tooltip.SetDefault("Fires whaling spears from the sky that bounce off tiles");
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

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld;
            for (int x = 0; x < 3; x++)
            {
                Projectile.NewProjectile(position.X + (float)Main.rand.Next(-30, 31), position.Y - 600f, 0f, 8f, type, damage, knockBack, Main.myPlayer, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<Mariana>()).AddIngredient(ModContent.ItemType<UeliaceBar>(), 7).AddIngredient(ModContent.ItemType<BarofLife>(), 2).AddIngredient(ModContent.ItemType<Lumenite>(), 20).AddIngredient(ModContent.ItemType<Tenebris>(), 5).AddTile(TileID.LunarCraftingStation).Register();
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
    }
}
