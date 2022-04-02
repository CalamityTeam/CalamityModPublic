using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BrimlashBuster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimlash Buster");
            Tooltip.SetDefault("50% chance to do triple damage on enemy hits\n" +
                "Fires a brimstone bolt that explodes into more bolts on death");
        }

        public override void SetDefaults()
        {
            item.width = item.height = 72;
            item.damage = 126;
            item.melee = true;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 8;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<BrimlashProj>();
            item.shootSpeed = 18f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Brimlash>());
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>(), 3);
            recipe.AddIngredient(ItemID.FragmentSolar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, (int)CalamityDusts.Brimstone);
            }
        }

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            float damageMult = 0f;
            if (player.Calamity().brimlashBusterBoost)
                damageMult = 2f;
            mult += damageMult;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            player.Calamity().brimlashBusterBoost = Main.rand.NextBool(3);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            player.Calamity().brimlashBusterBoost = Main.rand.NextBool(3);
        }
    }
}
