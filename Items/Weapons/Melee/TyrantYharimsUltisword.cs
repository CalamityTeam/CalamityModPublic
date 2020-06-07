using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TyrantYharimsUltisword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tyrant Yharim's Ultisword");
            Tooltip.SetDefault("Fires homing blazing blades\n" +
                "Gives the player the tyrant's fury buff on enemy hits\n" +
                "This buff increases melee damage by 30% and melee crit chance by 10%");
        }

        public override void SetDefaults()
        {
            item.width = 88;
            item.damage = 64;
            item.melee = true;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 26;
            item.useTurn = true;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 88;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<BlazingPhantomBlade>();
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<TrueCausticEdge>());
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddIngredient(ItemID.FlaskofVenom, 5);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 106);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            player.AddBuff(ModContent.BuffType<TyrantsFury>(), 180);
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.Venom, 180);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<TyrantsFury>(), 180);
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.Venom, 180);
        }
    }
}
