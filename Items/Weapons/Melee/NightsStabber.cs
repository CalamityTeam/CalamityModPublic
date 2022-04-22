using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Melee
{
    public class NightsStabber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Night's Stabber");
            Tooltip.SetDefault("Don't underestimate the power of stabby knives\n" +
                "Enemies release homing dark energy on hit");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTurn = false;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.width = 28;
            Item.height = 34;
            Item.damage = 60;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 6.5f;
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
            var source = player.GetSource_ItemUse(Item);
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<NightStabber>(), (int)(Item.damage * (player.GetDamage<GenericDamageClass>().Base + player.GetDamage(DamageClass.Melee).Base - 1f)), knockback, Main.myPlayer);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            var source = player.GetSource_ItemUse(Item);
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<NightStabber>(), (int)(Item.damage * (player.GetDamage<GenericDamageClass>().Base + player.GetDamage(DamageClass.Melee).Base - 1f)), Item.knockBack, Main.myPlayer);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LeechingDagger>().
                AddIngredient<AncientShiv>().
                AddIngredient<SporeKnife>().
                AddIngredient<FlameburstShortsword>().
                AddTile(TileID.DemonAltar).
                Register();
            CreateRecipe().
                AddIngredient<BloodyRupture>().
                AddIngredient<AncientShiv>().
                AddIngredient<SporeKnife>().
                AddIngredient<FlameburstShortsword>().
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
