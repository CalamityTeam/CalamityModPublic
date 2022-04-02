using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class XerocsGreatsword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Entropic Claymore");
            Tooltip.SetDefault("Fires a spread of homing plasma balls");
        }

        public override void SetDefaults()
        {
            item.width = 130;
            item.height = 106;
            item.damage = 90;
            item.melee = true;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 26;
            item.useTurn = true;
            item.knockBack = 5.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = ItemRarityID.Cyan;
            item.shoot = ModContent.ProjectileType<MeldGreatswordSmallProjectile>();
            item.shootSpeed = 12f;
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            hitbox = CalamityUtils.FixSwingHitbox(118, 118);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int num6 = Main.rand.Next(4, 6);
            for (int index = 0; index < num6; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
                float damageMult = 0.5f;
                switch (index)
                {
                    case 0:
                        type = ModContent.ProjectileType<MeldGreatswordSmallProjectile>();
                        break;
                    case 1:
                        type = ModContent.ProjectileType<MeldGreatswordMediumProjectile>();
                        damageMult = 0.65f;
                        break;
                    case 2:
                        type = ModContent.ProjectileType<MeldGreatswordBigProjectile>();
                        damageMult = 0.8f;
                        break;
                    default:
                        break;
                }
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)(damage * damageMult), knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MeldiateBar>(), 15);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 27);
            }
        }
    }
}
