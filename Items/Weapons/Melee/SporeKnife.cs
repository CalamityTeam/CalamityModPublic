using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SporeKnife : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spore Knife");
            Tooltip.SetDefault("Enemies release spore clouds on hit");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.Stabbing;
            item.useTurn = false;
            item.useAnimation = 12;
            item.useTime = 12;
            item.width = 32;
            item.height = 32;
            item.damage = 33;
            item.melee = true;
            item.knockBack = 5.75f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.JungleSpores, 10);
            recipe.AddIngredient(ItemID.Stinger, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 2);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            int proj = Projectile.NewProjectile(target.Center, Vector2.Zero, Main.rand.Next(569, 572), (int)(item.damage * 0.5f * player.MeleeDamage()), knockback, Main.myPlayer);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().forceMelee = true;
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            int proj = Projectile.NewProjectile(target.Center, Vector2.Zero, Main.rand.Next(569, 572), (int)(item.damage * 0.5f * player.MeleeDamage()), item.knockBack, Main.myPlayer);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().forceMelee = true;
        }
    }
}
