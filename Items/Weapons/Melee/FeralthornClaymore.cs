using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class FeralthornClaymore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Feralthorn Claymore");
            Tooltip.SetDefault("Summons thorns on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 68;
            item.damage = 80;
            item.melee = true;
            item.useAnimation = 13;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 13;
            item.useTurn = true;
            item.knockBack = 7.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 66;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 44);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 300);

            if (crit)
                damage /= 2;

            Projectile.NewProjectile(player.position.X + 40f + Main.rand.Next(0, 151), player.position.Y + 36f, 0f, -18f, ModContent.ProjectileType<ThornBase>(), (int)(damage * 0.25), 0f, Main.myPlayer);
            Projectile.NewProjectile(player.position.X - 40f + Main.rand.Next(-150, 1), player.position.Y + 36f, 0f, -18f, ModContent.ProjectileType<ThornBase>(), (int)(damage * 0.25), 0f, Main.myPlayer);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 300);

            if (crit)
                damage /= 2;

            Projectile.NewProjectile(player.position.X + 40f + Main.rand.Next(0, 151), player.position.Y + 36f, 0f, -18f, ModContent.ProjectileType<ThornBase>(), (int)(damage * 0.25), 0f, Main.myPlayer);
            Projectile.NewProjectile(player.position.X - 40f + Main.rand.Next(-150, 1), player.position.Y + 36f, 0f, -18f, ModContent.ProjectileType<ThornBase>(), (int)(damage * 0.25), 0f, Main.myPlayer);
        }
    }
}
