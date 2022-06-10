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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTurn = false;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.width = 32;
            Item.height = 32;
            Item.damage = 33;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 5.75f;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 2);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            var source = player.GetSource_ItemUse(Item);
            int sporeDamage = (int)player.GetDamage<MeleeDamageClass>().ApplyTo(Item.damage * 0.5f);
            int proj = Projectile.NewProjectile(source, target.Center, Vector2.Zero, Main.rand.Next(569, 572), sporeDamage, knockback, Main.myPlayer);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().forceMelee = true;
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            var source = player.GetSource_ItemUse(Item);
            int sporeDamage = (int)player.GetDamage<MeleeDamageClass>().ApplyTo(Item.damage * 0.5f);
            int proj = Projectile.NewProjectile(source, target.Center, Vector2.Zero, Main.rand.Next(569, 572), sporeDamage, Item.knockBack, Main.myPlayer);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().forceMelee = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.JungleSpores, 10).
                AddIngredient(ItemID.Stinger, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
