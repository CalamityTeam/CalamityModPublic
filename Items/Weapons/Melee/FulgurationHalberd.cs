using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class FulgurationHalberd : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fulguration Halberd");
            Tooltip.SetDefault("Inflicts burning blood on enemy hits\n" +
                "Right click to use as a spear");
        }

        public override void SetDefaults()
        {
            item.width = 60;
            item.damage = 53;
            item.melee = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.useTurn = true;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 62;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shootSpeed = 6f;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.noMelee = true;
                item.noUseGraphic = true;
                item.useStyle = 5;
                item.autoReuse = false;
                item.shoot = ModContent.ProjectileType<FulgurationHalberdProj>();
            }
            else
            {
                item.noMelee = false;
                item.noUseGraphic = false;
                item.useStyle = 1;
                item.autoReuse = true;
                item.shoot = 0;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<FulgurationHalberdProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 300);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrystalShard, 20);
            recipe.AddIngredient(ItemID.OrichalcumHalberd);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrystalShard, 20);
            recipe.AddIngredient(ItemID.MythrilHalberd);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
