using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SolsticeClaymore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solstice Claymore");
            Tooltip.SetDefault("Changes projectile color based on the time of year\n" +
                               "Inflicts daybroken during the day and nightwither during the night");
        }

        public override void SetDefaults()
        {
            item.width = 86;
            item.damage = 300;
            item.melee = true;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 16;
            item.useTurn = true;
            item.knockBack = 6.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 86;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<SolsticeBeam>();
            item.shootSpeed = 16f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BeamSword);
            recipe.AddIngredient(ModContent.ItemType<AstralBar>(), 20);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            int dustType = Main.dayTime ?
            Utils.SelectRandom(Main.rand, new int[]
            {
            6,
            259,
            158
            }) :
            Utils.SelectRandom(Main.rand, new int[]
            {
            173,
            27,
            234
            });
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, dustType);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.dayTime)
            {
                target.AddBuff(BuffID.Daybreak, 300);
            }
            else
            {
                target.AddBuff(ModContent.BuffType<Nightwither>(), 300);
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (!Main.dayTime)
            {
                target.AddBuff(ModContent.BuffType<Nightwither>(), 300);
            }
        }
    }
}
