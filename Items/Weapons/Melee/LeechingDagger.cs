using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class LeechingDagger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leeching Dagger");
            Tooltip.SetDefault("Enemies release homing leech orbs on hit");
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTurn = false;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.width = 26;
            Item.height = 26;
            Item.damage = 33;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 5.25f;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 14);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            var source = player.GetProjectileSource_Item(Item);
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<Leech>(), (int)(Item.damage * 0.5f * (player.GetDamage<GenericDamageClass>().Additive + player.GetDamage(DamageClass.Melee) - 1f)), knockback, Main.myPlayer);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            var source = player.GetProjectileSource_Item(Item);
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<Leech>(), (int)(Item.damage * 0.5f * (player.GetDamage<GenericDamageClass>().Additive + player.GetDamage(DamageClass.Melee) - 1f)), Item.knockBack, Main.myPlayer);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RottenChunk, 2).
                AddIngredient(ItemID.DemoniteBar, 5).
                AddIngredient<TrueShadowScale>(4).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
