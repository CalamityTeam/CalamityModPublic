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
            Item.width = 58;
            Item.damage = 123;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 36;
            Item.useTurn = true;
            Item.knockBack = 5;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 60;
            Item.scale = 1.5f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<DarkBeam>();
            Item.shootSpeed = 25f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = Main.rand.NextBool(2) ? type : ModContent.ProjectileType<LightBeam>();
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)(damage * 0.8), knockBack, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 12).AddIngredient(ItemID.FallenStar, 5).AddIngredient(ItemID.SoulofNight).AddIngredient(ItemID.SoulofLight).AddTile(TileID.MythrilAnvil).Register();
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
