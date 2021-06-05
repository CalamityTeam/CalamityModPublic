using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheLastMourning : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Last Mourning");
            Tooltip.SetDefault("Summons flaming pumpkins and mourning skulls that split into fire orbs on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 94;
            item.height = 94;
            item.scale = 1.5f;
            item.melee = true;
            item.damage = 360;
            item.knockBack = 8.5f;
            item.useAnimation = 20;
            item.useTime = 20;
            item.autoReuse = true;
            item.useTurn = true;

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;

            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.Calamity().donorItem = true;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            int projDamage = (int)(item.damage * player.MeleeDamage());
            bool isDoGSegment = target.type == ModContent.NPCType<DevourerofGodsBody>() || 
                target.type == ModContent.NPCType<DevourerofGodsBody2>() || 
                target.type == ModContent.NPCType<DevourerofGodsBodyS>();
            if (!isDoGSegment || Main.rand.NextBool(3))
            {
                CalamityPlayerOnHit.HorsemansBladeOnHit(player, target.whoAmI, projDamage, knockback, 0, ModContent.ProjectileType<MourningSkull>());
                CalamityPlayerOnHit.HorsemansBladeOnHit(player, target.whoAmI, projDamage, knockback, 1);
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            int projDamage = (int)(item.damage * player.MeleeDamage());
            CalamityPlayerOnHit.HorsemansBladeOnHit(player, -1, projDamage, item.knockBack, 0, ModContent.ProjectileType<MourningSkull>());
            CalamityPlayerOnHit.HorsemansBladeOnHit(player, -1, projDamage, item.knockBack, 1);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dustType = 5;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        dustType = 5;
                        break;
                    case 1:
                        dustType = 6;
                        break;
                    case 2:
                        dustType = 174;
                        break;
                    default:
                        break;
                }
                int dust = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, dustType, (float)(player.direction * 2), 0f, 150, default, 1.3f);
                Main.dust[dust].velocity *= 0.2f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<BalefulHarvester>());
            r.AddIngredient(ItemID.SoulofNight, 30);
            r.AddIngredient(ModContent.ItemType<ReaperTooth>(), 5);
            r.AddIngredient(ModContent.ItemType<RuinousSoul>(), 3);
            r.AddTile(TileID.LunarCraftingStation);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
