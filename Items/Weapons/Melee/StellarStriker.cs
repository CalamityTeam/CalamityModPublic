using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class StellarStriker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stellar Striker");
            Tooltip.SetDefault("Summons a swarm of lunar flares from the sky on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 84;
            item.damage = 640;
            item.melee = true;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 25;
            item.useTurn = true;
            item.knockBack = 7.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 90;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shootSpeed = 12f;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy)
            {
                return;
            }
            if (player.whoAmI == Main.myPlayer)
                SpawnFlares(player, knockback);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (player.whoAmI == Main.myPlayer)
                SpawnFlares(player, item.knockBack);
        }

        private void SpawnFlares(Player player, float knockBack)
        {
            Main.PlaySound(SoundID.Item88, player.Center);
			Projectile flare = CalamityUtils.ProjectileToMouse(player, 2, item.shootSpeed, 0.03f, 80f, ProjectileID.LunarFlare, (int)(item.damage * player.MeleeDamage() * 0.5), knockBack, player.whoAmI, true, 40f);
			flare.Calamity().forceMelee = true;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 229);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CometQuasher>());
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
