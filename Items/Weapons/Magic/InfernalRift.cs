using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class InfernalRift : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernal Rift");
            Tooltip.SetDefault("Summons infernal blades that spawn additional blades on enemy hits");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 40;
            item.magic = true;
            item.mana = 15;
            item.width = 16;
            item.height = 16;
            item.useAnimation = 16;
            item.useTime = 4;
            item.reuseDelay = item.useAnimation + 6;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item9;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<InfernalBlade>();
            item.shootSpeed = 16f;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 25;

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SkyFracture);
            recipe.AddIngredient(ModContent.ItemType<EssenceofChaos>(), 3);
            recipe.AddIngredient(ItemID.SoulofFright, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num72 = item.shootSpeed;
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            float f = Main.rand.NextFloat() * 6.28318548f;
            float value12 = 20f;
            float value13 = 60f;
            Vector2 vector13 = vector2 + f.ToRotationVector2() * MathHelper.Lerp(value12, value13, Main.rand.NextFloat());
            for (int num202 = 0; num202 < 50; num202++)
            {
                vector13 = vector2 + f.ToRotationVector2() * MathHelper.Lerp(value12, value13, Main.rand.NextFloat());
                if (Collision.CanHit(vector2, 0, 0, vector13 + (vector13 - vector2).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                {
                    break;
                }
                f = Main.rand.NextFloat() * 6.28318548f;
            }
            Vector2 mouseWorld = Main.MouseWorld;
            Vector2 vector14 = mouseWorld - vector13;
            Vector2 vector15 = new Vector2(num78, num79).SafeNormalize(Vector2.UnitY) * num72;
            vector14 = vector14.SafeNormalize(vector15) * num72;
            vector14 = Vector2.Lerp(vector14, vector15, 0.25f);
            Projectile.NewProjectile(vector13, vector14, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
