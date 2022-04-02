using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DarklightGreatsword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Darklight Greatsword");
            Tooltip.SetDefault("Fires darklight blades that split on death");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.damage = 123;
            item.melee = true;
            item.useAnimation = 36;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 36;
            item.useTurn = true;
            item.knockBack = 5;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 60;
            item.scale = 1.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<DarkBeam>();
            item.shootSpeed = 25f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = Main.rand.NextBool(2) ? type : ModContent.ProjectileType<LightBeam>();
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)(damage * 0.8), knockBack, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 12);
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddIngredient(ItemID.SoulofNight);
            recipe.AddIngredient(ItemID.SoulofLight);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 29);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}
