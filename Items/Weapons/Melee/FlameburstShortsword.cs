using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class FlameburstShortsword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flameburst Shortsword");
            Tooltip.SetDefault("Enemies explode on hit");
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTurn = false;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.width = 30;
            Item.height = 30;
            Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.HellstoneBar, 7).AddTile(TileID.Anvils).Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 6);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            int boom = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), (int)(Item.damage * 0.75f * player.MeleeDamage()), knockback, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            if (boom.WithinBounds(Main.maxProjectiles))
                Main.projectile[boom].Calamity().forceMelee = true;
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            int boom = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), (int)(Item.damage * 0.75f * player.MeleeDamage()), Item.knockBack, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            if (boom.WithinBounds(Main.maxProjectiles))
                Main.projectile[boom].Calamity().forceMelee = true;
        }
    }
}
