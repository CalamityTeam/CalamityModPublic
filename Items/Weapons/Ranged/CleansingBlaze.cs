using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class CleansingBlaze : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cleansing Blaze");
            Tooltip.SetDefault("90% chance to not consume gel");
        }

        public override void SetDefaults()
        {
            Item.damage = 130;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 64;
            Item.height = 32;
            Item.useTime = 3;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item34;
            Item.value = Item.buyPrice(1, 80, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EssenceFire>();
            Item.shootSpeed = 14f;
            Item.useAmmo = AmmoID.Gel;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/CleansingBlazeGlow"));
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int num6 = Main.rand.Next(2, 4);
            for (int index = 0; index < num6; ++index)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-15, 16) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-15, 16) * 0.05f;
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 90)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 12).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
